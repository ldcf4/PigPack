using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PigPack
{
    public struct FileAB
    {
        public AssetBundle ab;
        public string abname;
        public string root;
    }


    public class FileReader
    {
        //文本文件目录
        //key:类型枚举
        //value:路径数组，源目录从Assets开始，打包的时候会拷贝到SteamingAssets目录中去
        public static readonly Dictionary<string, string[]> TXT_FILES = new Dictionary<string, string[]>
    {
        {"lua",new string[]{"Lua", "ToLua/Lua" } },
        {"data",new string[]{"Data"} },
    };
        //需要提前预加载ab的root（就是TXT_FILES中需要提前加载的key）
        private static readonly string[] pres = new string[] { "lua" };

        private static FileReader _instence = null;
        public static FileReader Instance
        {
            get
            {
                if (_instence==null)
                {
                    _instence = new FileReader();
                }
                return _instence;
            }
        }

        private Dictionary<string, string[]> m_root_map = new Dictionary<string, string[]>();
        private Dictionary<string, FileAB> m_ab_map = new Dictionary<string, FileAB>();

        public static string FileABPath
        {
            get
            {
                string filedata = null;
                if (Application.isEditor)
                {
                    filedata = PackDefine.StreamingAssets + '/' + PackDefine.FILE_DATA_NAME;
                }
                else
                {
                    filedata = PackDefine.SandBoxPath + '/' + PackDefine.FILE_DATA_NAME;
                }
                return filedata;
            }
        }

        public static string FileFilePath
        {
            get
            {
                string filepath = null;
                if (Application.isEditor)
                {
                    filepath = PackDefine.Assets;
                }
                else
                {
                    filepath = PackDefine.SandBoxPath + "/" + PackDefine.FILE_DATA_NAME;
                }
                return filepath;
            }
        }

        public void PreLoadAssetBundle()
        {
            if (PackDefine.PACK_TEXT_FILE)
            {
                string path = FileABPath;
                foreach (string pre_root in pres)
                {
                    string abpath = path + '/' + pre_root;
                    string manifestpath = abpath + '/' + pre_root;
                    AssetBundle manifestAB = AssetBundle.LoadFromFile(manifestpath);
                    AssetBundleManifest manifest = manifestAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                    string[] allabs = manifest.GetAllAssetBundles();
                    foreach (string abname in allabs)
                    {
                        AssetBundle ab = AssetBundle.LoadFromFile(abpath + '/' + abname);
                        FileAB fab = new FileAB()
                        {
                            ab = ab,
                            abname = abname,
                            root = pre_root,
                        };
                        m_ab_map[abname] = fab;
                    }
                    manifestAB.Unload(false);
                }
            }
        }

        public List<FileAB> GetAllPreLoadABByRoot(string root)
        {
            List<FileAB> m_ret = new List<FileAB>();
            foreach (var item in m_ab_map)
            {
                if (item.Value.root==root)
                {
                    m_ret.Add(item.Value);
                }
            }
            return m_ret;
        }

        public void InitPath()
        {
            if (PackDefine.PACK_TEXT_FILE)
            {
                string head = FileABPath;
                foreach (var item in TXT_FILES)
                {
                    m_root_map[item.Key] = new string[] { Path.Combine(head, item.Key) };
                }
            }
            else
            {
                string head = FileFilePath;
                foreach (var item in TXT_FILES)
                {
                    string[] path_list = new string[item.Value.Length];
                    for (int i = 0; i < item.Value.Length; i++)
                    {
                        path_list[i] = Path.Combine(head, item.Value[i]);
                    }
                    m_root_map[item.Key] = path_list;
                }
            }
            
        }

        public string ReadText(string root, string path)
        {
            if (PackDefine.PACK_TEXT_FILE)
            {
                string abname = root;
                string assetname = path;
                if (!path.Contains("/"))
                {
                }
                else
                {
                    int pos = path.LastIndexOf('/');
                    string temp = path.Substring(0, pos);
                    abname = root + '_' + temp.ToLower().Replace('/','_');
                    assetname = path.Substring(pos + 1);
                }
                abname += PackDefine.AB_EXT;
                FileAB fab;
                if (m_ab_map.TryGetValue(abname,out fab))
                {//这个AB已经提前加载好了
                    Debug.Log("load pre ab");
                    TextAsset ret = fab.ab.LoadAsset<TextAsset>(assetname);
                    return ret.text;
                }
                else
                {//ab还没有加载
                    Debug.Log("load unpre ab");
                    string abpath = m_root_map[root][0];
                    string filename = abpath + '/' + abname;
                    var ab = AssetBundle.LoadFromFile(filename); //加载ab
                    TextAsset ret = ab.LoadAsset<TextAsset>(assetname);
                    string retstr = ret.text;
                    ab.Unload(false); //用完就卸载
                    return retstr;
                }
                return null;
            }
            else
            {
                string[] path_list = null;
                if (m_root_map.TryGetValue(root,out path_list))
                {
                    foreach (var search_path in path_list)
                    {
                        string temp = Path.Combine(search_path, path);
                        if (File.Exists(temp))
                        {
                            return File.ReadAllText(temp);
                        }
                    }
                }
                return null;
            }
        }

    }
}