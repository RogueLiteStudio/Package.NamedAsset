using System.Collections;

namespace NamedAsset
{
    public class AssetManager
    {

#if UNITY_EDITOR
        public static IAssetPackageInfoProvider PackageInfo;
#endif
        private static IAssetProvider assetProvider;

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void SetEditorModeMaxLoadCount(int count)
        {
            AssetDatabaseProvider.MaxLoadAssetCount = count;
        }

        public static IEnumerable Initialize(IPathProvider pathProvider)
        {

#if UNITY_EDITOR
            if (assetProvider == null)
            {
                bool forceBundle = UnityEditor.EditorPrefs.GetBool("_forceBundle_");
                if (!forceBundle)
                {
                    assetProvider = new AssetDatabaseProvider();
                }
            }
#endif
            assetProvider ??= new AssetBundleProvider(pathProvider);
            yield return assetProvider.Initialize();
        }

        public static NamedAssetRequest Load(string name)
        {
            return assetProvider?.LoadAsset(name);
        }

        public static void Destroy()
        {
            assetProvider?.Destroy();
        }
    }
}
