using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Diagnostics;
using System.IO;

namespace SpadeAce
{
    internal class ExternToolExtension
    {
        private static ExternToolAsset _asset;
        internal static ExternToolAsset asset
        {
            get
            {
                if (_asset == null)
                    _asset = ExternToolAsset.Load("Assets/ExternToolExt.asset");
                return _asset;
            }
        }
        [OnOpenAsset(2)]
        public static bool OpenFile(int instanceID, int line, int column)
        {
            if (asset == null)
                return false;
            string filePath = AssetDatabase.GetAssetPath(instanceID);
            string extension = Path.GetExtension(filePath);
            extension = extension.TrimStart('.');
            string exePath = asset.GetPath(extension);
            if(!string.IsNullOrWhiteSpace(exePath))
            {
                filePath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, filePath);
                Process.Start(exePath, $"\"{filePath}\"");
                return true;
            }

            return false;
        }
    }
}