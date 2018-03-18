using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

namespace PigPack
{
    public class AssetPackFileManager:AssetPackManager
    {
        private Dictionary<string, AssetPackFileImpl> m_packdata_map = new Dictionary<string, AssetPackFileImpl>();

        public static string PACK_DATA_FILE
        {
            get
            {
                return Path.Combine(PackDefine.PACK_DATA_JSON_PATH, PackDefine.ART_PACK_DATA_LIST);
            }
        }


        public override AssetPack LoadPack(string abname)
        {
            AssetPackFileImpl ap = null;
            m_packdata_map.TryGetValue(abname, out ap);
            return ap;
        }

        //从json中读取PackData
        public override void LoadConfig()
        {
            string path = PACK_DATA_FILE;
            if (File.Exists(path))
            {
                string buff = File.ReadAllText(path);
                PackDataList list = JsonUtility.FromJson<PackDataList>(buff);
                foreach (PackData item in list.list)
                {
                    m_packdata_map.Add(item.abname, new AssetPackFileImpl(item));
                }
            }
        }

        public override void UnloadPack(string abname)
        {
            //文件方式加载不需要卸载
        }
    }
}