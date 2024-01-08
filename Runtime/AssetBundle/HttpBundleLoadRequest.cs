using UnityEngine;
using UnityEngine.Networking;

namespace NamedAsset
{
    internal class HttpBundleLoadRequest : BundleLoadRequest
    {
        private readonly UnityWebRequest webRequest;
        public override bool IsDone => webRequest.isDone;

        public override AssetBundle GetAssetBundle()
        {
            return DownloadHandlerAssetBundle.GetContent(webRequest);
        }
        public HttpBundleLoadRequest(string url, AssetBundleInfo info)
        {
            Info = info;
            webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url, info.Hash, 0);
            webRequest.SendWebRequest();
        }
    }
}
