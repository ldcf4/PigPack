using UnityEngine;
using System.Collections;
using System;

namespace PigPack
{
    public class AssetPackABImpl:AssetPack
    {
        private AssetBundle m_ab;
        private int m_ref;

        public AssetPackABImpl(AssetBundle ab)
        {
            m_ab = ab;
            m_ref = 0;
        }

        public void Lock()
        {
            m_ref++;
            Debug.LogFormat("{0} lock:{1}",m_ab.name, m_ref);
        }

        public bool Unlock()
        {
            m_ref--;
            Debug.LogFormat("{0} unlock:{1}",m_ab.name, m_ref);
            return m_ref <= 0;
        }

        public void Unload()
        {
            m_ab.Unload(false);
            m_ab = null;
        }

        public override UnityEngine.Object LoadAsset(string name, Type type)
        {
            return m_ab.LoadAsset(name, type);
        }

        public override T LoadAsset<T>(string name)
        {
            return m_ab.LoadAsset<T>(name);
        }
    }
}
