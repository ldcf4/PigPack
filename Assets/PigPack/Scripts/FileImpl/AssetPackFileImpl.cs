using System;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PigPack
{
    public class AssetPackFileImpl:AssetPack
    {
        private PackData m_data;

        public AssetPackFileImpl(PackData data)
        {
            m_data = data;
        }

        private string FindPath(string name)
        {
            foreach (var filepath in m_data.assetfiles)
            {
                string filename = Path.GetFileNameWithoutExtension(filepath);
                if (filename==name)
                {
                    return filepath;
                }
            }
            return null;
        }

        public override UnityEngine.Object LoadAsset(string name, Type type)
        {
#if UNITY_EDITOR
            string path = FindPath(name);
            return AssetDatabase.LoadAssetAtPath(path, type);
#else
            return null;
#endif
        }

        public override T LoadAsset<T>(string name)
        {
#if UNITY_EDITOR
            string path = FindPath(name);
            return AssetDatabase.LoadAssetAtPath<T>(path);
#else
            return null;
#endif
        }
    }
}