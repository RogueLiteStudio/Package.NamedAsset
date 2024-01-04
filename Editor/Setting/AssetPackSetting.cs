using System.Collections.Generic;
using UnityEditor;
namespace NamedAsset.Editor
{
    [FilePath("ProjectSettings/AssetPackSetting.asset", FilePathAttribute.Location.ProjectFolder)]
    public class AssetPackSetting : ScriptableSingleton<AssetPackSetting>
    {
        //允许不同的Package重名，但是不能有相同的资源名
        //重复的资源会被过滤掉
        public List<AssetPackage> Packages = new List<AssetPackage>();

        public void Save()
        {
            Save(true);
        }
    }
}