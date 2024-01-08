using UnityEngine;

namespace NamedAsset
{
    internal class FileBundleLoadRequest : BundleLoadRequest
    {
        private readonly AssetBundleCreateRequest createRequest;
        public override bool IsDone => createRequest.isDone;

        public override AssetBundle GetAssetBundle()
        {
            return createRequest.assetBundle;
        }

        public FileBundleLoadRequest(string path, AssetBundleInfo info)
        {
            Info = info;
            createRequest = AssetBundle.LoadFromFileAsync(path);
        }
    }
}
