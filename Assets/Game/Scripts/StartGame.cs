using LuaInterface;
using PigPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour {

    private LuaState lua = null;

    private void Awake()
    {
        //初始化FileReader
        FileReader reader = FileReader.Instance;
        reader.InitPath();
        reader.PreLoadAssetBundle();

        //设置lua读取路径
        LuaFileUtils fileutils = new LuaFileUtils();
        fileutils.beZip = PackDefine.PACK_TEXT_FILE;
        if (PackDefine.PACK_TEXT_FILE)
        {
            var luaab = reader.GetAllPreLoadABByRoot("lua");
            foreach (var ab in luaab)
            {
                string abname = ab.abname;
                if (abname.EndsWith(PackDefine.AB_EXT))
                {
                    abname = abname.Substring(0, abname.Length-PackDefine.AB_EXT.Length);
                }
                fileutils.AddSearchBundle(abname, ab.ab);
            }
        }
        else
        {
            var pathes = FileReader.TXT_FILES["lua"];
            foreach (var path in pathes)
            {
                fileutils.AddSearchPath(path);
            }
        }

        //初始化AssetManager
        AssetManager assetMgr = AssetManager.Instance;
        //GameObject can = GameObject.Find("Canvas");
        //GameObject uistart = assetMgr.LoadPrefab("ui_prefab", "PanelHelloWorld");
        //GameObject.Instantiate<GameObject>(uistart, can.transform);

        //上面的设置完毕了，启动lua状态机
        lua = new LuaState();
        lua.Start();
        LuaBinder.Bind(lua);

        //lua入口文件
        lua.DoFile("Main.lua");
        Debug.Log("After dofile main.lua");
    }

    private void OnDestroy()
    {
        lua.Dispose();
        lua = null;
    }

}
