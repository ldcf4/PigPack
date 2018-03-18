using UnityEngine;
using System.Collections;

namespace PigPack
{
    public abstract class AssetPackManager
    {
        public abstract void LoadConfig();
        public abstract AssetPack LoadPack(string abname);
        public abstract void UnloadPack(string abname);
    }
}