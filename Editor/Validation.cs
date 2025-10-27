using UnityEditor;
using UnityEngine;

namespace Entin.StaticData.Editor
{
    public static class Validation
    {
        [MenuItem("Validation/Validate static data")]
        public static void ValidateStaticData()
        {
            StaticData staticData = new StaticData();

            if (staticData.Validate())
                Debug.Log("<color=green>Static data validation succeed</color>");
            else
                Debug.Log("<color=red>Static data has errors</color>");
        }
    }
}