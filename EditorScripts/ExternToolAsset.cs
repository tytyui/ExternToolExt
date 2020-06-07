using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

internal class ExternToolAsset : ScriptableObject
{
    public List<ExternToolItem> items = new List<ExternToolItem>();

    public void Create(string assetPath)
    {
        AssetDatabase.CreateAsset(this, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
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
