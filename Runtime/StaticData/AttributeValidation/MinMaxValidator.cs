using System;
using System.Reflection;
using Entin.StaticData.CsvReader;
using Entin.StaticData.Sheet;

namespace Entin.StaticData.Attributes
{
    public class MinMaxValidator : IAttributeValidator
    {
        public void Validate<TSheet>(StaticData staticData, Action<string> onError)
            where TSheet : BaseSheet
        {
            foreach (PropertyInfo propertyInfo in typeof(TSheet).GetProperties())
            {
                Attribute[] minAttributes = Attribute.GetCustomAttributes(propertyInfo, typeof(MinMaxAttribute), true);

                foreach (var attribute in minAttributes)
                {
                    ValidateMinMax<TSheet>(staticData, propertyInfo, attribute, onError);
                }
            }
        }

        private void ValidateMinMax<TSheet>(StaticData staticData, PropertyInfo propertyInfo, Attribute attribute, Action<string> onError)
            where TSheet : BaseSheet
        {
            foreach (TSheet baseSheet in staticData.Get<TSheet>())
            {
                object value1 = propertyInfo.GetValue(baseSheet);

                bool isMinInt = attribute is MinIntAttribute;
                bool isMaxInt = attribute is MaxIntAttribute;

                if (isMinInt || isMaxInt)
                {
                    if (value1 is not int intValue)
                    {
                        onError($"Property with key {propertyInfo.Name} is not int");
                        return;
                    }

                    MinIntAttribute minInt = attribute as MinIntAttribute;
                    MaxIntAttribute maxInt = attribute as MaxIntAttribute;

                    if (isMinInt && intValue < minInt.Min)
                    {
                        onError($"{propertyInfo.Name} less than {minInt.Min}");
                    }
                    else if (isMaxInt && intValue > maxInt.Max)
                    {
                        onError($"{propertyInfo.Name} more than {maxInt.Max}");
                    }
                }

                bool isMinFloat = attribute is MinFloatAttribute;
                bool isMaxFloat = attribute is MaxFloatAttribute;

                if (isMinFloat || isMaxFloat)
                {
                    if (value1 is not float floatValue)
                    {
                        onError($"Property with key {propertyInfo.Name} is not float");
                        return;
                    }

                    MinFloatAttribute minFloat = attribute as MinFloatAttribute;
                    MaxFloatAttribute maxFloat = attribute as MaxFloatAttribute;

                    if (isMinFloat && floatValue < minFloat.Min)
                    {
                        onError($"{propertyInfo.Name} less than {minFloat.Min}");
                    }
                    else if (isMaxFloat && floatValue > maxFloat.Max)
                    {
                        onError($"{propertyInfo.Name} more than {maxFloat.Max}");
                    }
                }
            }
        }
    }
}