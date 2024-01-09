using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace NamedAsset
{
    internal class AssetDatabaseProvider : IAssetProvider, ITickable
    {
        public static int MaxLoadAssetCount = 10;
        private Dictionary<string, string> assetPaths = new Dictionary<string, string>();
        private Dictionary<string, NamedAssetRequest> assetRequests = new Dictionary<string, NamedAssetRequest>();
        private Queue<string> assetLoadQueue = new Queue<string>();

        public AssetDatabaseProvider()
        {

        }

        public IEnumerable Initialize()
        {
            yield return new WaitForEndOfFrame();
        }

        public NamedAssetRequest LoadAsset(string name)
        {
#if UNITY_EDITOR
            if (!assetRequests.TryGetValue(name, out var request))
            {
                if (assetPaths.TryGetValue(name, out var path))
                {
                    request = new NamedAssetRequest();
                    request.isPrefab = path.EndsWith(".prefab");
                    assetRequests.Add(name, request);
                    assetLoadQueue.Enqueue(name);
                }
                else
                {
                    request = NamedAssetRequest.NoneExist;
                }
            }
            return request;
#else
            return NamedAssetRequest.NoneExist;
#endif
        }
        public void Destroy()
        {
#if UNITY_EDITOR
            foreach (var kv in assetRequests)
            {
                kv.Value.asset = null;
                kv.Value.State = AssetLoadState.None;
            }
            assetRequests.Clear();
#endif
        }

        public bool OnTick()
        {
#if UNITY_EDITOR
            int count = 0;
            while (count < MaxLoadAssetCount && assetLoadQueue.Count > 0)
            {
                var name = assetLoadQueue.Dequeue();
                if (assetRequests.TryGetValue(name, out var request))
                {
                    var asset = UnityEditor.AssetDatabase.LoadMainAssetAtPath(assetPaths[name]);
                    request.SetAsset(asset);
                    ++count;
                }
            }
            return false;
#else
            return true;
#endif
        }
    }
}
