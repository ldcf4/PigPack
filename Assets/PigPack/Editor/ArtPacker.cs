using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace PigPack
{
    public class ArtPacker
    {
        public const string PREFAB_EXT = "*.prefab";
        public const string PNG_EXT = "*.png";

        List<PackData> packList = new List<PackData>();

        [MenuItem("PigPacker/CreateArtPackData")]
        public static void CreateArtPackData()
        {
            ArtPacker packer = new ArtPacker();
            packer.PackUIPrefab();
            packer.PackUISprite();
            packer.WritePackData();
            Debug.Log("====CreateArtPackData over====");
        }

        [MenuItem("PigPacker/BuildArtAssetBundles")]
        public static void BuildArtAssetBundles()
        {
            string path = AssetPackFileManager.PACK_DATA_FILE;
            if (File.Exists(path))
            {
                string buff = File.ReadAllText(path);
                PackDataList list = JsonUtility.FromJson<PackDataList>(buff);
                List<AssetBundleBuild> buildlist = new List<AssetBundleBuild>();
                foreach (PackData pack in list.list)
                {
                    AssetBundleBuild builddata = new AssetBundleBuild()
                    {
                        assetBundleName = pack.abname+PackDefine.AB_EXT,
                        assetNames = pack.assetfiles,
                    };
                    buildlist.Add(builddata);
                }

                string output = PackDefine.StreamingAssets + "/" + PackDefine.FILE_ART_NAME;
                PigPacker.EmptyDir(output);
                BuildAssetBundleOptions opt = BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle;
                AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(output, buildlist.ToArray(), opt, EditorUserBuildSettings.activeBuildTarget);
                PigPacker.DeleteFile(output, "*" + PackDefine.AB_EXT + PackDefine.MANIFEST_EXT, SearchOption.AllDirectories);
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("Pack Over", "BuildArtAssetBundles finish", "OK");
            }
            else
            {
                Debug.LogErrorFormat("file not exist:{0}", path);
            }
        }

        private void WritePackData()
        {
            PackDataList list = new PackDataList();
            list.list = packList.ToArray();
            string buff = JsonUtility.ToJson(list);
            string wpath = PackDefine.PACK_DATA_JSON_PATH;
            if ( !Directory.Exists(wpath) )
            {
                Directory.CreateDirectory(wpath);
            }
            string path = AssetPackFileManager.PACK_DATA_FILE;
            File.WriteAllText(path, buff);
        }

        private void AddDir(string abname,string dir,string file_ext)
        {
            PackData data = new PackData()
            {
                abname = abname,
            };
            string[] fls = Directory.GetFiles(dir, file_ext, SearchOption.TopDirectoryOnly);
            data.assetfiles = new string[fls.Length];
            for (int i = 0; i < fls.Length; i++)
            {
                string fl = fls[i].Replace('\\', '/');
                data.assetfiles[i] = fl;
            }
            packList.Add(data);
        }

        private void PackUIPrefab()
        {
            string uiprefab = "Assets/Game/UI/Prefab";
            AddDir("ui_prefab", uiprefab, PREFAB_EXT);
        }

        private void PackUISprite()
        {
            string spritepath = "Assets/Game/UI/Sprite";
            AddDir("ui_sprite", spritepath, PNG_EXT);
        }
    }
}