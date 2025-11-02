using UnityEngine;

namespace Entin.StaticData.DownloadSheets.Editor
{
    [CreateAssetMenu(fileName = "StaticDataGoogleSheetSettings", menuName = "Entin/Static Data/StaticDataGoogleSheetSettings", order = 1)]
    public class DownloadSheetsSettings : ScriptableObject
    {
        public string TableId;
        public string IgnoreSymbol = "_";
        public Sheet[] Sheets;
    }
}
