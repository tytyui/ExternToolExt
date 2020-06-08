using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace SpadeAce
{
    internal class ExternToolAsset : ScriptableObject
    {
        public List<ExternToolItem> items = new List<ExternToolItem>();

        public void Create(string assetPath)
        {
            AssetDatabase.CreateAsset(this, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        public string GetPath(string extension)
        {
            for (int i = 0; i < items.Count; ++i)
            {
                string[] exts = items[i].extension.Split(';');
                for (int j = 0; j < exts.Length; ++j)
                {
                    if (exts[j] == extension)
                        return items[i].path;
                }
            }
            return null;
        }

        public bool Contains(string extension)
        {
            return !string.IsNullOrWhiteSpace(GetPath(extension));
        }

        public static ExternToolAsset Load(string assetPath)
        {
            return AssetDatabase.LoadAssetAtPath<ExternToolAsset>(assetPath);
        }
    }

    [CustomEditor(typeof(ExternToolAsset))]
    internal class ExternToolAssetInspector : Editor
    {
        private ExternToolAsset asset;

        private void OnEnable()
        {
            if (asset == null)
                asset = target as ExternToolAsset;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();
            int count = asset.items.Count;
            GUILayout.Label($"数量: {count}");
            for (int i = 0; i < count; ++i)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                GUILayout.Label($"扩展名: {asset.items[i].extension}");
                GUILayout.Label($"程序路径: {asset.items[i].path}");
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }
    }

    [System.Serializable]
    internal class ExternToolItem
    {
        public string extension;
        public string path;
    }
}