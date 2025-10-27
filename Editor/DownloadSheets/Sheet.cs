using UnityEngine;

namespace Entin.StaticData.DownloadSheets.Editor
{
    [System.Serializable]
    public struct Sheet
    {
        [SerializeField]
        public string Name;
        [SerializeField]
        public long Id;

        public Sheet(string name, long id)
        {
            Name = name;
            Id = id;
        }
    }
}
