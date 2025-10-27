using UnityEngine;

namespace Entin.StaticData.DownloadSheets.Editor
{
    [CreateAssetMenu(fileName = "StaticDataGoogleSheetSettings", menuName = "ScriptableObjects/DownloadSheetsSettings", order = 1)]
    public class DownloadSheetsSettings : ScriptableObject
    {
        public string TableId;
        public string IgnoreSymbol = "_";
        public Sheet[] Sheets;
    }
}
