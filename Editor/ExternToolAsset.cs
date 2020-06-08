using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
            string path = null;
            ForEach((ext, extPath) => {
                if(ext == extension)
                {
                    path = extPath;
                    return false;
                }
                return true;
            });
            return path;
        }

        public bool Contains(string extension)
        {
            return !string.IsNullOrWhiteSpace(GetPath(extension));
        }

        public void ForEach(Func<string, string, bool> onForEach)
        {
            for (int i = 0; i < items.Count; ++i)
            {
                string[] exts = items[i].extension.Split(';');
                for (int j = 0; j < exts.Length; ++j)
                {
                    if (!onForEach(exts[i], items[i].path))
                        return;
                }
            }
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

    [Serializable]
    internal class ExternToolItem
    {
        public string extension = null;
        public string path = null;
    }
}