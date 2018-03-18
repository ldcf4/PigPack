using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using PigPack;
using System.IO;

namespace PigPack
{
    public static class PigPacker
    {
        public const string __temp_txt__path = "Assets/__temp_txt__/";

        //把FileReader中定义的目录都打包成AssetBundle
        [MenuItem("PigPacker/BuildTxtAssetBundles")]
        public static void BuildTxtAssetBundles()
        {
            EmptyDir(__temp_txt__path);

            foreach (var item in FileReader.TXT_FILES)
            {
                string packpath = Path.Combine(__temp_txt__path, item.Key);

                foreach (var filepath in item.Value)
                {
                    string src = Path.Combine(PackDefine.Assets, filepath);
                    string dst = packpath;
                    CopyDir(src, dst, PackDefine.PACK_FILE_EXT);
                }
            }

            AssetDatabase.Refresh();

            foreach (var item in FileReader.TXT_FILES)
            {
                string packpath = Path.Combine(__temp_txt__path, item.Key).Replace('\\','/');

                string[] dirs = Directory.GetDirectories(packpath, "*.*", SearchOption.AllDirectories);
                List<string> build_dirs = new List<string>(dirs)
                {
                    packpath
                };
                List<AssetBundleBuild> m_build_list = new List<AssetBundleBuild>();
                foreach (string dir in build_dirs)
                {
                    string[] files = Directory.GetFiles(dir, "*" + PackDefine.PACK_FILE_EXT, SearchOption.TopDirectoryOnly);
                    string abname = dir.Replace('\\', '/').Replace(__temp_txt__path, "").Replace('/', '_').ToLower() + PackDefine.AB_EXT;
                    AssetBundleBuild builddata = new AssetBundleBuild();
                    builddata.assetBundleName = abname;
                    builddata.assetNames = files;
                    m_build_list.Add(builddata);
                }
                string outputpath = Path.Combine(PackDefine.StreamingAssets, PackDefine.FILE_DATA_NAME);
                outputpath = Path.Combine(outputpath, item.Key);
                EmptyDir(outputpath);
                BuildAssetBundleOptions opt = BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle;
                Debug.LogFormat("start build :{0}", outputpath);
                AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(outputpath, m_build_list.ToArray(), opt, EditorUserBuildSettings.activeBuildTarget);
                Debug.LogFormat("~~~~~~~~~~~~~~~~~~end build :{0}", outputpath);
                DeleteFile(outputpath, "*"+PackDefine.AB_EXT + PackDefine.MANIFEST_EXT, SearchOption.AllDirectories);
            }
            Debug.Log("==============BuildTxtAssetBundles over==================");
            Directory.Delete(__temp_txt__path,true);
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Pack Over", "BuildTxtAssetBundles finish", "OK");
        }

        //拷贝FileReader里面定义的文本文件目录到SteamingAssets目录中去
        [MenuItem("PigPacker/CopyTxtFiles")]
        public static void CopyTxtFiles()
        {
            string outputpath = Path.Combine(PackDefine.StreamingAssets, PackDefine.FILE_DATA_NAME);
            EmptyDir(outputpath);
            foreach (var item in FileReader.TXT_FILES)
            {
                foreach (var path_list in item.Value)
                {
                    CopyFileData(path_list);
                }
            }
        }

        public static void CopyFileData(string path)
        {
            string src = Path.Combine(PackDefine.Assets, path);
            string file_data = Path.Combine(PackDefine.StreamingAssets, PackDefine.FILE_DATA_NAME);
            string dst = Path.Combine(file_data, path);
            CopyDir(src, dst);
        }

        public static void CopyDir(string src, string dst, string ext)
        {
            string rep = src.Replace('\\', '/')+'/';
            string[] files = Directory.GetFiles(src, "*.*", SearchOption.AllDirectories);
            int index = 1;
            foreach (string file in files)
            {
                string extname = string.Empty;
                if (file.Contains("."))
                {
                    extname = file.Substring(file.LastIndexOf('.'));
                }
                if (extname != ".meta")
                {
                    string temp = file.Replace('\\', '/');
                    string filename = temp.Replace(rep,"")+ext;
                    string target = Path.Combine(dst, filename);
                    string dir = Path.GetDirectoryName(target);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    File.Copy(file, target, true);
                    EditorUtility.DisplayProgressBar("CopyDir", file + "\n" + target, index * 1f / file.Length);
                }
                index++;
            }
            EditorUtility.ClearProgressBar();
        }

        public static void CopyDir(string src, string dst)
        {
            CopyDir(src, dst, "");
        }

        //清空目录
        public static void EmptyDir(string dir)
        {
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
            Directory.CreateDirectory(dir);
        }

        public static void DeleteFile(string path, string ext, SearchOption so)
        {
            string[] files = Directory.GetFiles(path, ext, so);
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }
    }
}