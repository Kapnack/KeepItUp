using UnityEditor;
using UnityEngine;

namespace Systems
{
    public class VersionIncrementer : EditorWindow
    {
        [MenuItem("Tools/Version Incrementer")]
        public static void ShowWindow()
        {
            GetWindow<VersionIncrementer>("Version Incrementer");
        }

        private void OnGUI()
        {
            GUILayout.Label("Version Control", EditorStyles.boldLabel);

            GUILayout.Space(10);

            EditorGUILayout.TextField("Current Version:", PlayerSettings.bundleVersion);
            EditorGUILayout.IntField("Android Bundle Code:", PlayerSettings.Android.bundleVersionCode);

            GUILayout.Space(10);

            if (GUILayout.Button("Increment Version & Bundle Code"))
            {
                if (EditorUtility.DisplayDialog("Confirm Increment",
                        "Are you sure you want to increment the Project Version and Android Bundle Code?",
                        "Yes", "No"))
                {
                    IncrementAndroidBundleCode();
                    IncrementProjectVersion();
                    AssetDatabase.SaveAssets();
                }
            }
        }

        void IncrementAndroidBundleCode()
        {
            PlayerSettings.Android.bundleVersionCode++;
        }

        void IncrementProjectVersion()
        {
            string currentVersion = PlayerSettings.bundleVersion;
            string[] versionParts = currentVersion.Split('.');

            if (versionParts.Length > 0)
            {
                int lastNum;
                if (int.TryParse(versionParts[versionParts.Length - 1], out lastNum))
                {
                    lastNum++;
                    versionParts[versionParts.Length - 1] = lastNum.ToString();
                    PlayerSettings.bundleVersion = string.Join(".", versionParts);
                    Debug.Log($"Version updated to: {PlayerSettings.bundleVersion}");
                }
                else
                {
                    Debug.LogError("Could not parse the last part of the version string as an integer.");
                }
            }
        }
    }
}