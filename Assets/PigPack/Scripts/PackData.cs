using UnityEngine;
using System.Collections;
using System;

namespace PigPack
{
    [Serializable()]
    public struct PackData
    {
        //AssetBundle的名字
        public string abname;
        //对应打包的资源文件
        public string[] assetfiles;
    }

    [Serializable()]
    public struct PackDataList
    {
        public PackData[] list;
    }
}


