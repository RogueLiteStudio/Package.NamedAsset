using System.Collections.Generic;
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
        public AssetBundle Bundle;
        public BundleLoadState State;
        public int DepnedenceComplateCount;
        public List<AssetBundleInfo> Dependence = new List<AssetBundleInfo>();
        public string[] AssetNames;
        public NamedAssetRequest[] RequestList;

        public bool IsDone => State > BundleLoadState.Loading && Dependence.Count == DepnedenceComplateCount;

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
