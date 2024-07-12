using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SC.Editor.Helpers
{
    public class GuidChanger
    {
        private static Dictionary<string, string> guidMap = new();

        // [MenuItem("Tools/Change GUIDs in Selected Folder")]
        public static void ChangeGuidsInSelectedFolder()
        {
            var selectedAsset = Selection.activeObject;
            if (selectedAsset == null)
            {
                Debug.LogError("No asset selected. Please select a folder in the Project window.");
                return;
            }
        
            var folderPath = AssetDatabase.GetAssetPath(selectedAsset);
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                Debug.LogError("Selected asset is not a folder. Please select a folder in the Project window.");
                return;
            }
        
            ProcessDirectory(folderPath);
            AssetDatabase.Refresh();
        
            UpdateReferences();
        }

        private static void ProcessDirectory(string directoryPath)
        {
        
            var fileEntries = Directory.GetFiles(directoryPath);
            foreach (var filePath in fileEntries)
            {
                if (!filePath.EndsWith(".meta")) continue;

                ProcessFile(filePath);
            }
        
            var subdirectoryEntries = Directory.GetDirectories(directoryPath);
            foreach (string subdirectoryPath in subdirectoryEntries)
            {
                ProcessDirectory(subdirectoryPath);
            }
        }

        private static void ProcessFile(string metaFilePath)
        {
            var metaFileLines = File.ReadAllLines(metaFilePath);
            for (int i = 0; i < metaFileLines.Length; i++)
            {
                if (metaFileLines[i].StartsWith("guid:"))
                {
                    var oldGuid = metaFileLines[i].Split(' ')[1];
                    var newGuid = System.Guid.NewGuid().ToString("N");
                    metaFileLines[i] = "guid: " + newGuid;
                
                    guidMap[oldGuid] = newGuid;

                    Debug.Log($"GUID changed in {metaFilePath} from {oldGuid} to {newGuid}");
                    break;
                }
            }
            File.WriteAllLines(metaFilePath, metaFileLines);
        }

        private static void UpdateReferences()
        {
            var allAssets = AssetDatabase.GetAllAssetPaths();
            foreach (string assetPath in allAssets)
            {
                if (assetPath.EndsWith(".unity") || assetPath.EndsWith(".prefab"))
                {
                    var fileContent = File.ReadAllText(assetPath);
                    foreach (var kvp in guidMap)
                    {
                        fileContent = fileContent.Replace(kvp.Key, kvp.Value);
                    }
                    File.WriteAllText(assetPath, fileContent);
                }
            }

            AssetDatabase.Refresh();
        }
    }
}
