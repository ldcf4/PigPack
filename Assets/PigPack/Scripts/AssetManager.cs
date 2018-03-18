using System;
using UnityEngine;

namespace PigPack
{
    public class AssetManager
    {
        private static AssetManager _instence = null;
        public static AssetManager Instance
        {
            get
            {
                if (_instence == null)
                {
                    _instence = new AssetManager();
                }
                return _instence;
            }
        }

        private AssetPackManager m_pack_manager;

        public AssetManager()
        {
            if (PackDefine.PACK_ASSETBUNDLE)
            {
                m_pack_manager = new AssetPackABManager();
            }
            else
            {
                m_pack_manager = new AssetPackFileManager();
            }
            m_pack_manager.LoadConfig();
        }

        public T LoadAsset<T>(string abname, string assetname) where T : UnityEngine.Object
        {
            var ap = LoadPack(abname);
            return ap.LoadAsset<T>(assetname);
        }

        public UnityEngine.Object LoadAsset(string abname, string assetname, Type type)
        {
            var ap = LoadPack(abname);
            return ap.LoadAsset(assetname,type);
        }

        public GameObject LoadPrefab(string abname,string assetname)
        {
            return LoadAsset<GameObject>(abname, assetname);
        }

        public AssetPack LoadPack(string abname)
        {
            return m_pack_manager.LoadPack(abname);
        }

        public void UnloadPack(string abname)
        {
            m_pack_manager.UnloadPack(abname);
        }
    }
}


