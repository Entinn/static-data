using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Entin.StaticData.CsvReader
{
    public static class Reader
    {
        public static string CleanIgnoreSymbols(string text, string ignoreSymbol)
        {
            var stringBuilder = new StringBuilder();

            string[,] grid = SplitCsvGrid(text, false);
            int columns = grid.GetLength(0);
            int rows = grid.GetLength(1);

            List<int> ignoredColumns = new List<int>();

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    if (r == 0 && !string.IsNullOrEmpty(grid[c, 0]) && grid[c, 0].StartsWith(ignoreSymbol))
                        ignoredColumns.Add(c);

                    if (ignoredColumns.Contains(c) || (!string.IsNullOrEmpty(grid[c, r]) && grid[c, r].StartsWith(ignoreSymbol)))
                        grid[c, r] = string.Empty;

                    if (!string.IsNullOrEmpty(grid[c, r]) && (grid[c, r].Contains(',') || grid[c, r].Contains('"')))
                        grid[c, r] = $"\"{grid[c, r]}\"";

                    stringBuilder.Append(grid[c, r]);
                    if (c != columns - 1)
                    {
                        stringBuilder.Append(',');
                    }
                }

                stringBuilder.Append(Environment.NewLine);
            }

            return stringBuilder.ToString();
        }

        public static T[] Parse<T>(string data)
        {
            string[,] grid = SplitCsvGrid(data, true);

            List<T> valueList = new List<T>();
            List<Dictionary<string, string>> list = ReadToDict(grid);
            for (int i = 0; i < list.Count; i++)
            {
                Dictionary<string, string> dict = list[i];
                FillValues(valueList, dict, i);
            }

            return valueList.ToArray();
        }

        public static T ParseKeyValue<T>(string data)
        {
            string[,] grid = SplitCsvGrid(data, true);

            Dictionary<string, string> dict = GetKeyValueDict(grid);
            return FillValue<T>(dict);
        }

        private static void FillValues<T>(List<T> valueList, Dictionary<string, string> dict, int lineNumber)
        {
            T info = Activator.CreateInstance<T>();
            FillFromDictionary(info, dict, lineNumber);
            valueList.Add(info);
        }

        private static T FillValue<T>(Dictionary<string, string> dict)
        {
            T info = Activator.CreateInstance<T>();
            FillFromDictionary(info, dict, 0);
            return info;
        }

        private static void FillFromDictionary<T>(T target, Dictionary<string, string> valuesDict, int lineNumber)
        {
            var allProperties = target.GetType().GetProperties();

            foreach (var property in allProperties)
            {
                var name = property.Name.ToLower();
                if (valuesDict.ContainsKey(name))
                {
                    try
                    {
                        string valueStr = valuesDict[name];
                        FillValue(target, property, valueStr, lineNumber);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"[CsvReader.FillFromDictionary] <{typeof(T)}> fail fo set value `{valuesDict[name]}` for key `{name}`", ex);
                    }
                }
                else
                {
                    throw new Exception($"[CsvReader.FillFromDictionary] <{typeof(T)}> storage csv dont have column `{name}`", null);
                }
            }
        }

        private static object ParseValue(Type type, string valueStr, string tableName, int lineNumber)
        {
            return ParseValueUnsafe(type, valueStr, tableName, lineNumber);
        }

        private static object ParseValueUnsafe(Type type, string valueStr, string tableName, int lineNumber)
        {
            if (type.IsEnum)
            {
                try
                {
                    object value = Enum.Parse(type, valueStr, true);
                    return value;
                }
                catch
                {
                    throw new Exception($"Can't parse enum {valueStr} to {type} in table {tableName}. Line number {lineNumber}");
                }
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IReadOnlyList<>))
            {
                Type genericType = type.GetGenericArguments().Single();

                Type genericListType = typeof(List<>);
                Type concreteListType = genericListType.MakeGenericType(genericType);
                IList list = Activator.CreateInstance(concreteListType) as IList;
                string[] values = valueStr.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string t in values)
                {
                    list.Add(ParseValueUnsafe(genericType, t, tableName, lineNumber));
                }

                return list;
            }

            if (type == typeof(Vector2))
            {
                string[] temp = valueStr.Split(';', ',', '.');
                return new Vector2(float.Parse(temp[0]), float.Parse(temp[1]));
            }

            if (type == typeof(Vector2Int))
            {
                string[] temp = valueStr.Split(';', ',', '.');
                return new Vector2(int.Parse(temp[0]), int.Parse(temp[1]));
            }

            if (string.IsNullOrEmpty(valueStr) && (type == typeof(int) || type == typeof(float) || type == typeof(decimal)))
                valueStr = "0";
            if (string.IsNullOrEmpty(valueStr) && type == typeof(string))
                valueStr = "";

            try
            {
                return Convert.ChangeType(valueStr, type, CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                throw new Exception($"Can't parse {valueStr} to {type} in table {tableName}. Line number {lineNumber + 1}. Exception: {e.Message}");
            }
        }

        public static void FillValue<T>(T target, PropertyInfo property, string valueStr, int lineNumber)
        {
            object value = ParseValue(property.PropertyType, valueStr, target.ToString(), lineNumber);

            MethodInfo setter = property.GetSetMethod(nonPublic: true);
            if (setter is not null)
            {
                setter.Invoke(target, new[] { value });
                return;
            }

            FieldInfo backingField = property.DeclaringType?.GetField($"<{property.Name}>k__BackingField", DeclaredOnlyLookup);
            if (backingField is not null)
            {
                backingField.SetValue(target, value);
                return;
            }

            throw new InvalidOperationException($"Could not find a way to set {property.DeclaringType?.FullName}.{property.Name}. Try adding a private setter.");
        }

        public const BindingFlags DeclaredOnlyLookup = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

        private static List<Dictionary<string, string>> ReadToDict(string[,] grid)
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            ReadToList(grid, (keys, row) =>
            {
                if (row.All(string.IsNullOrEmpty))
                    return;

                Dictionary<string, string> dict = new Dictionary<string, string>();
                for (int i = 0; i < keys.Count; i++)
                {
                    string key = keys[i].ToLower();
                    string value = row[i];
                    dict[key] = value;
                }

                list.Add(dict);
            });
            return list;
        }

        private static void ReadToList(string[,] grid, Action<List<string>, List<string>> rowAction)
        {
            List<string> keys = new List<string>();
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                string value = grid[x, 0];
                if (value != null)
                    keys.Add(value);
                else
                    keys.Add("");
            }

            for (int y = 1; y < grid.GetLength(1); y++)
            {
                List<string> row = Enumerable.Range(0, grid.GetLength(0)).Select(o => string.IsNullOrEmpty(grid[o, y]) ? "" : grid[o, y]).ToList();
                rowAction(keys, row);
            }
        }

        private static Dictionary<string, string> GetKeyValueDict(string[,] grid)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            for (int y = 1; y < grid.GetLength(1); y++)
            {
                string key = grid[0, y];
                if (string.IsNullOrEmpty(key))
                    continue;

                string value = grid[1, y];
                value = string.IsNullOrEmpty(value) ? "" : value;
                dict.Add(key.ToLower(), value);
            }

            return dict;
        }

        private static string[,] SplitCsvGrid(string csvText, bool replaceDoubleQuotation)
        {
            string[] lines = csvText.Split("\n"[0]);

            // finds the max width of row
            int width = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                List<string> row = SplitCsvLine(lines[i]);
                for (int j = row.Count - 1; j >= 0; j--)
                {
                    if (string.IsNullOrEmpty(row[j]))
                        row.RemoveAt(j);
                    else
                        break;
                }

                width = Math.Max(width, row.Count);
            }

            // creates new 2D string grid to output to
            string[,] outputGrid = new string[width, lines.Length];
            for (int y = 0; y < lines.Length; y++)
            {
                List<string> row = SplitCsvLine(lines[y]);
                for (int x = 0; x < row.Count; x++)
                {
                    if (x >= width)
                        continue;

                    outputGrid[x, y] = row[x];

                    // This line was to replace "" with " in my output.
                    // Include or edit it as you wish.
                    if (replaceDoubleQuotation)
                        outputGrid[x, y] = outputGrid[x, y].Replace("\"\"", "\"");
                }
            }

            return outputGrid;
        }

        private static List<string> SplitCsvLine(string line)
        {
            return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
            @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)",
            System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
                    select m.Groups[1].Value).ToList();
        }
    }
}
