using UnityEngine;

namespace NamedAsset
{
    internal abstract class BundleLoadRequest
    {
        public AssetBundleInfo Info;
        public abstract bool IsDone { get; }
        public abstract AssetBundle GetAssetBundle();
    }
}
