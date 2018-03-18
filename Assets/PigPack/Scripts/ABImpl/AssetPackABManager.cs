using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;

namespace PigPack
{
    public class AssetPackABManager : AssetPackManager
    {
        private AssetBundleManifest m_manifest = null;
        private Dictionary<string, AssetPackABImpl> m_ab_map = new Dictionary<string, AssetPackABImpl>();

        public static string FileArtPath
        {
            get
            {
                if (Application.isEditor)
                {
                    return PackDefine.StreamingAssets + '/' + PackDefine.FILE_ART_NAME;
                }
                else
                {
                    return PackDefine.SandBoxPath + '/' + PackDefine.FILE_ART_NAME;
                }
            }
        }

        //读取AssetBundle的manifest
        public override void LoadConfig()
        {
            string manifestPath = FileArtPath + '/' + PackDefine.FILE_ART_NAME;
            AssetBundle ab = AssetBundle.LoadFromFile(manifestPath);
            m_manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        private string GetRealABName(string abname)
        {
            if (!abname.EndsWith(PackDefine.AB_EXT))
            {
                return abname + PackDefine.AB_EXT;
            }
            return abname;
        }

        public override AssetPack LoadPack(string abname)
        {
            AssetPackABImpl ret = GetAssetBundle(GetRealABName(abname));
            return ret;
        }

        public override void UnloadPack(string abname)
        {
            string realname = GetRealABName(abname);
            //减去引用计数，当计数为0时卸载AB
            AssetPackABImpl ap = null;
            if (m_ab_map.TryGetValue(realname,out ap))
            {
                if (ap.Unlock())
                {
                    ap.Unload();
                    string[] depNames = m_manifest.GetDirectDependencies(realname);
                    foreach (var depName in depNames)
                    {
                        UnloadPack(depName);
                    }
                }
            }
        }

        private AssetPackABImpl GetAssetBundle(string abname)
        {
            AssetPackABImpl ap = null;
            if (!m_ab_map.TryGetValue(abname,out ap))
            {
                ap = LoadAssetPackAuto(abname);
                m_ab_map.Add(abname, ap);
            }
            ap.Lock();
            return ap;
        }

        private AssetPackABImpl LoadAssetPackAuto(string abname)
        {
            string[] deps = m_manifest.GetDirectDependencies(abname);
            foreach (string depAbname in deps)
            {
                AssetPackABImpl depAp = GetAssetBundle(depAbname);
            }
            string abpath = Path.Combine(FileArtPath, abname);
            AssetBundle ab = AssetBundle.LoadFromFile(abpath);
            return new AssetPackABImpl(ab);
        }
    }
}