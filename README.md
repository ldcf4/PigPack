# PigPack
PigPack用于Unity的资源打包读取工具,在编辑器模式的时候可以直接读取文件,而在游戏发布后从AssetBundle中读取文件.(默认是根据Tolua配置的)这个工具只是提供一个思路.

## 目录
### Game
Game目录为例子游戏资源目录,其中StartGame脚本中有怎么初始化本工具的代码,以及和Tolua结合使用的方式
### PigPack
PigPack目录为本工具所需要的资源以及代码.
#### FileReader
其中FileReader为读取文本类文件用.
#### AssetManager
AssetManager为读取美术音效等文件用.
#### PackDefine
PackDefine为配置是否从编辑器读取资源.
### 与Tolua结合
需要把FileReader,AssetManager,AssetPack放到添加到Tolua的ConstomSetting里面导出给LUA环境使用.