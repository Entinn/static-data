using System;
using System.Text;
using System.Threading.Tasks;
using Entin.StaticData.CsvReader;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Entin.StaticData.DownloadSheets.Editor
{
    internal sealed class UpdateDataEditorWindow : EditorWindow
    {
        private static DownloadSheetsSettings settings;

        private const string UrlPattern = "https://docs.google.com/spreadsheets/d/{0}/export?format=csv&gid={1}";
        private const string SettingsFilePath = "StaticDataGoogleSheetSettings.asset";

        private SerializedObject _so;

        [MenuItem("Entin/Static Data/Open window")]
        private static void Init()
        {
            TryLoadSettings();
            UpdateDataEditorWindow window = (UpdateDataEditorWindow)GetWindow(typeof(UpdateDataEditorWindow));
            window.Show();
        }

        private static bool TryLoadSettings()
        {
            DownloadSheetsSettings loadedSettings = (DownloadSheetsSettings)EditorGUIUtility.Load(SettingsFilePath);
            if (loadedSettings == null)
                return false;

            settings = loadedSettings;
            return true;
        }

        private void OnGUI()
        {
            GUILayout.Space(20);

            if (settings == null)
            {
                EditorGUILayout.LabelField("Create 'StaticDataGoogleSheetSettings.asset' in 'Editor Default Resources' folder");
                return;
            }

            DrawSheetsSettings();

            if (GUILayout.Button(nameof(UpdateFiles)))
            {
                UpdateFiles();
            }
        }

        private void OnEnable()
        {
            _so = new SerializedObject(settings);
        }

        private void DrawSheetsSettings()
        {
            settings.TableId = EditorGUILayout.TextField("Table Id", settings.TableId);
            settings.IgnoreSymbol = EditorGUILayout.TextField("Ignore symbol", settings.IgnoreSymbol);
            _so.Update();
            SerializedProperty stringsProperty = _so.FindProperty("Sheets");
            EditorGUILayout.PropertyField(stringsProperty, true);
            _so.ApplyModifiedProperties();
        }

        [MenuItem("Entin/Static Data/Update files")]
        private static void UpdateFiles()
        {
            if (TryLoadSettings())
                UpdateFiles(settings.TableId, settings.Sheets);
        }

        private static void UpdateFiles(string tableId, Sheet[] sheets)
        {
            var folder = Application.dataPath + "/Data/Resources/Files";
            var sheetsCount = sheets.Length;

            Debug.Log("<color=yellow>Sync started.</color>");

            foreach (var sheet in sheets)
            {
                var url = string.Format(UrlPattern, tableId, sheet.Id);

                UpdateFiles(url, www =>
                {
                    var path = System.IO.Path.Combine(folder, sheet.Name + ".csv");

                    string data = ModifyData(www.downloadHandler.data);
                    Debug.Log(data);

                    System.IO.File.WriteAllText(path, data);
                    Debug.LogFormat("Sheet {0} saved to {1}", sheet.Id, path);

                    AssetDatabase.Refresh();

                    if (--sheetsCount == 0)
                    {
                        Debug.Log("<color=green>Files successfully synced!</color>");
                    }
                });
            }
        }

        private static string ModifyData(byte[] bytes)
        {
            string text = Encoding.UTF8.GetString(bytes);
            return Reader.CleanIgnoreSymbols(text, settings.IgnoreSymbol);
        }

        private static async void UpdateFiles(string url, Action<UnityWebRequest> callback)
        {
            Debug.LogFormat("downloading {0}", url);

            var www = UnityWebRequest.Get(url);

            var operation = www.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (www.result == UnityWebRequest.Result.Success)
            {
                callback(www);
            }
            else
            {
                Debug.Log($"Failed: {url} with error: {www.error}");
            }
        }
    }
}