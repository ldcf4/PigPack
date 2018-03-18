using UnityEngine;
using System.Collections;

namespace PigPack
{
    public static class PackDefine
    {
        //assetbundle模式
        public static bool PACK_ASSETBUNDLE
        {
            get
            {
                if (Application.isEditor)
                {//编辑器可以使用文件模式
                    return true;
                }
                else 
                {//非编辑器模式必须为AB模式
                    return true;
                }
            }
        }

        //文本类文件是否打包
        public static bool PACK_TEXT_FILE
        {
            get
            {
                if (Application.isEditor)
                {
                    return false;
                }
                else if (Application.isMobilePlatform)
                {//非编辑器的移动平台（应该就是手机真机的意思）
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        public const string PigPackName = "PigPack";
        public const string Assets = "Assets";
        public const string StreamingAssets = "Assets/StreamingAssets";
        //沙箱路径
        public static readonly string SandBoxPath = Application.persistentDataPath + "/SandBox";

        public const string PACK_FILE_EXT = ".bytes";
        public const string AB_EXT = ".unity3d";

        public const string MANIFEST_EXT = ".manifest";

        public const string FILE_DATA_NAME = "file_data";
        public const string FILE_ART_NAME = "file_art";

        public const string ART_PACK_DATA_LIST = "ArtPackDataList.json";
        public static readonly string PACK_DATA_JSON_PATH = Assets + "/" + PigPackName + "/JSON/";
    }
}
