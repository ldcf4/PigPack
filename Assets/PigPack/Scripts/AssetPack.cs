using UnityEngine;
using System;

namespace PigPack
{
    public abstract class AssetPack
    {

        public abstract UnityEngine.Object LoadAsset(string name, Type type);

        public abstract T LoadAsset<T>(string name) where T : UnityEngine.Object;

        public GameObject LoadPrefab(string name)
        {
            return LoadAsset<GameObject>(name);
        }
    }
}

