using System.Collections;
using System.Collections.Generic;

namespace NamedAsset
{
    internal class AssetBundleProvider : IAssetProvider, ITickable
    {
        public static int MaxLoadBundleCount = 10;

        private readonly List<AssetBundleInfo> bundleInfos = new List<AssetBundleInfo>();
        private AssetManifest.AssetInfo[] assets;
        private readonly List<LoadBundleAssetTask> assetTasks = new List<LoadBundleAssetTask>();
        private readonly Queue<AssetBundleInfo> bundleQueue = new Queue<AssetBundleInfo>();
        private readonly List<BundleLoadTask> bundleTasks = new List<BundleLoadTask>();
        private readonly IPathProvider pathProvider;

        public AssetBundleProvider(IPathProvider pathProvider)
        {
            this.pathProvider = pathProvider;
        }

        public IEnumerable Initialize()
        {
            var file = pathProvider.GetAssetManifestPath();
            yield return AsyncFileUtil.ReadAssetManifest(file, (manifest) =>
            {
                if (manifest == null)
                {
                    throw new System.Exception($"load AssetManifest fail : {file.Path} => empty data");
                }
                assets = manifest.Assets.ToArray();
                BuildBundleInfo(manifest);
            });
        }

        public NamedAssetRequest LoadAsset(string name)
        {
            for (int i=0; i<assets.Length; ++i)
            {
                if (assets[i].Name == name)
                {
                    int location = assets[i].Location;
                    return LocationToRequest(location);
                }
            }
            return NamedAssetRequest.NoneExist;
        }

        private NamedAssetRequest LocationToRequest(int location)
        {
            int bundleIdx = location >> 16;
            int assetIdx = location & 0xFFFF;
            var info = bundleInfos[bundleIdx];
            CheckAndLoadBundle(info);
            var request = info.RequestList[assetIdx];
            if (request == null)
            {
                request = new NamedAssetRequest();
                info.RequestList[assetIdx] = request;
            }
            if (CreateLoadAssetTask(info, assetIdx))
            {
                AssetUpdateLoop.Instance.AddLoadTick(this);
            }
            return info.RequestList[assetIdx];
        }

        private void CheckAndLoadBundle(AssetBundleInfo info)
        {
            if (info.State < BundleLoadState.InQueue)
            {
                info.State = BundleLoadState.InQueue;
                for (int i=0; i<info.DependenceIdx.Length; ++i)
                {
                    var dep = bundleInfos[info.DependenceIdx[i]];
                    if (dep.State < BundleLoadState.InQueue)
                    {
                        bundleQueue.Enqueue(dep);
                    }
                }
                bundleQueue.Enqueue(info);
                AssetUpdateLoop.Instance.AddLoadTick(this);
            }
        }

        private void BuildBundleInfo(AssetManifest manifest)
        {
            for (int i=0; i< manifest.Bundles.Count; ++i)
            {
                var bundle = manifest.Bundles[i];
                AssetBundleInfo info = new AssetBundleInfo
                {
                    Index = i,
                    Path = bundle.Name,
                    Hash = bundle.Hash,
                    Crc = bundle.Crc
                };
                int depCount = 0;
                if (bundle.Dependencies != null && bundle.Dependencies.Length > 0)
                {
                    depCount = bundle.Dependencies.Length;
                }
                info.DependenceIdx = new int[depCount];
                if (bundle.Assets != null && bundle.Assets.Length > 0)
                {
                    info.AssetNames = bundle.Assets;
                    info.RequestList = new NamedAssetRequest[bundle.Assets.Length];
                }
                bundleInfos.Add(info);
            }
            for (int i=0; i<bundleInfos.Count; ++i)
            {
                var info = bundleInfos[i];
                var bundle = manifest.Bundles[i];
                for (int j = 0; j < bundle.Dependencies.Length; ++j)
                {
                    var idx = bundleInfos.FindIndex(it => it.Path == bundle.Dependencies[j]);
                    info.DependenceIdx[j] = idx;
                }
            }
        }

        private bool CreateLoadAssetTask(AssetBundleInfo bundleInfo, int idx)
        {
            var request = bundleInfo.RequestList[idx];
            if (request != null && !request.keepWaiting 
                && bundleInfo.IsDone)
            {
                if (bundleInfo.Bundle)
                {
                    var task = LoadBundleAssetTask.Create(request, bundleInfo.Bundle, bundleInfo.AssetNames[idx]);
                    assetTasks.Add(task);
                    return true;
                }
                else
                {
                    //如果bundle加载失败，做加载失败处理
                    //防止卡加载
                    request.SetAsset(null);
                }
            }
            return false;
        }

        private void OnBundleLoadComplete(AssetBundleInfo info)
        {
            foreach (var bundle in bundleInfos)
            {
                if (bundle == info)
                    continue;
                for (int i = 0; i < bundle.DependenceIdx.Length; ++i)
                {
                    if (bundle.DependenceIdx[i] == info.Index)
                    {
                        ++bundle.DepnedenceComplateCount;
                        if (bundle.IsDone)
                        {
                            OnBundleDone(bundle);
                        }
                        break;
                    }
                }
            }
            if (info.IsDone)
            {
                OnBundleDone(info);
            }
        }
        private void OnBundleDone(AssetBundleInfo info)
        {
            for (int i = 0; i < info.RequestList.Length; ++i)
            {
                if (info.RequestList[i] != null)
                {
                    CreateLoadAssetTask(info, i);
                }
            }
        }

        public bool OnTick()
        {
            for (int i=bundleTasks.Count-1; i>=0; --i)
            {
                var task = bundleTasks[i];
                if (task.IsDone)
                {
                    var bundle = task.GetAssetBundle();
                    task.Info.Bundle = bundle;
                    task.Info.State = bundle ? BundleLoadState.Loaded : BundleLoadState.LoadFailed;
                    OnBundleLoadComplete(task.Info);
                    bundleTasks.RemoveAt(i);
                }
            }
            while (bundleTasks.Count < MaxLoadBundleCount && bundleQueue.Count > 0)
            {
                var info = bundleQueue.Dequeue();
                info.State = BundleLoadState.Loading;
                var task = AsyncFileUtil.LoadAssetBundle(pathProvider.GetAssetBundlePath(info.Path), info);
                if (task == null)
                {
                    bundleTasks.Add(task);
                }
                else
                {
                    task.Info.State = BundleLoadState.LoadFailed;
                    OnBundleLoadComplete(info);
                }
            }

            for (int i=assetTasks.Count-1; i>=0; --i)
            {
                var task = assetTasks[i];
                if (task.IsComplete)
                {
                    task.OnFinish();
                    assetTasks.RemoveAt(i);
                }
            }

            return assetTasks.Count == 0 && bundleTasks.Count == 0;
        }

        public void Destroy()
        {
            AssetUpdateLoop.RemoveTick(this);
            foreach (var bundle in bundleInfos)
            {
                bundle.Destroy();
            }
            bundleInfos.Clear();
            bundleQueue.Clear();
            bundleTasks.Clear();
            assetTasks.Clear();
        }
    }
}
