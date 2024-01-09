using UnityEngine;

namespace NamedAsset
{
    public enum BundleLoadState
    {
        None,
        InQueue,
        Loading,
        LoadFailed,
        Loaded,
    }

    internal class AssetBundleInfo
    {
        public string Path;
        public Hash128 Hash;
        public int Index;
        public uint Crc;
        public int[] DependenceIdx;
        public string[] AssetNames;
        public int DepnedenceComplateCount;
        public BundleLoadState State;
        public AssetBundle Bundle;
        public NamedAssetRequest[] RequestList;

        public bool IsDone => State > BundleLoadState.Loading && DependenceIdx.Length == DepnedenceComplateCount;

        //重置操作，只是去掉引用，不做卸载操作
        public void Reset()
        {
            if (RequestList != null)
            {
                for (int i = 0; i < RequestList.Length; ++i)
                {
                    var r = RequestList[i];
                    if (r != null)
                    {
                        r.State = AssetLoadState.None;
                        r.asset = null;
                    }
                }
            }
        }

        public void Destroy()
        {
            Bundle.Unload(true);
            DepnedenceComplateCount = 0;
            Bundle = null;
            State = default;
            if (RequestList != null)
            {
                for (int i = 0; i < RequestList.Length; ++i)
                {
                    var r = RequestList[i];
                    if (r != null)
                    {
                        r.State = AssetLoadState.None;
                        r.asset = null;
                    }
                }
            }
            RequestList = null;
            AssetNames = null;
        }
    }
}
