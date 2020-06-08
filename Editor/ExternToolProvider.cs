using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using UnityEngine.Events;

namespace SpadeAce
{
    internal class ExternToolProvider : SettingsProvider
    {
        private const string assetPath = "Assets/ExternToolExt.asset";

        private ExternToolAsset asset;
        private SerializedObject serializedObject;
        private SerializedProperty items;
        private ReorderableList list;

        public ExternToolProvider()
            : base("Preferences/外部工具扩展", SettingsScope.User)
        {
            if (asset == null)
                asset = ExternToolAsset.Load(assetPath);
            serializedObject = new SerializedObject(asset);
            items = serializedObject.FindProperty("items");
            if (list == null)
            {
                list = new ReorderableList(serializedObject, items, true, true, true, true);
                list.elementHeight = 48;
                list.drawElementCallback = OnDrawReorderableList;
                list.drawHeaderCallback = OnDrawHeader;
            }
        }

        private void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "扩展名列表");
        }

        private void OnDrawReorderableList(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty item = items.GetArrayElementAtIndex(index);
            SerializedProperty ext = item.FindPropertyRelative("extension");
            SerializedProperty extPath = item.FindPropertyRelative("path");
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y += 2;
            Rect extRect = rect;
            extRect.width = 48;
            EditorGUI.LabelField(extRect, "扩展名");
            extRect.x += extRect.width;
            extRect.width = 100;
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(extRect, ext, GUIContent.none);
            if (EditorGUI.EndChangeCheck())
                ext.stringValue = ReognizeExtInput(ext.stringValue);
            Rect extPathRect = rect;
            extPathRect.width = 48;
            extPathRect.y += extPathRect.height + 2;
            EditorGUI.LabelField(extPathRect, "程序路径");
            extPathRect.x += extPathRect.width;
            extPathRect.width = 300;
            GUI.enabled = false;
            EditorGUI.PropertyField(extPathRect, extPath, GUIContent.none);
            GUI.enabled = true;
            extPathRect.x += extPathRect.width + 8;
            extPathRect.width = 48;
            if (GUI.Button(extPathRect, "浏览..."))
            {
                string path = EditorUtility
                    .OpenFilePanelWithFilters(
                    "选择程序文件", "",
                    new string[] { "可执行文件", "exe" }
                );
                if (!string.IsNullOrWhiteSpace(path))
                    extPath.stringValue = path;
            }
            extPathRect.x += extPathRect.width + 4;
            if (GUI.Button(extPathRect, "默认"))
                extPath.stringValue = "";
        }

        [SettingsProvider]
        private static SettingsProvider ProviderGUI()
        {
            if (!IsAvailable())
            {
                ExternToolAsset asset = ScriptableObject.CreateInstance<ExternToolAsset>();
                asset.Create(assetPath);
                Debug.Log($"创建asset文件: {assetPath}");
            }
            return new ExternToolProvider();
        }

        private static bool IsAvailable()
        {
            return File.Exists(assetPath);
        }

        public override void OnGUI(string searchContext)
        {
            serializedObject.Update();
            list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private string ReognizeExtInput(string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                string[] inputs = input.Split(';');
                for (int i = 0; i < inputs.Length; ++i)
                    inputs[i] = inputs[i].Trim();
                return string.Join(";", inputs);
            }
            return input;
        }

        private void ForEachItem(SerializedProperty items, UnityAction<int, SerializedProperty, SerializedProperty> onForEach)
        {
            int size = items.arraySize;
            for (int i = 0; i < size; ++i)
            {
                SerializedProperty item = this.items.GetArrayElementAtIndex(i);
                SerializedProperty extension = item.FindPropertyRelative("extension");
                SerializedProperty extPath = item.FindPropertyRelative("path");
                onForEach(i, extension, extPath);
            }
        }

        public override void OnDeactivate()
        {
            ForEachItem(items, (index, extension, exPath) =>
            {
                if (string.IsNullOrWhiteSpace(extension.stringValue))
                    items.DeleteArrayElementAtIndex(index);
            });
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
        }
    }
}