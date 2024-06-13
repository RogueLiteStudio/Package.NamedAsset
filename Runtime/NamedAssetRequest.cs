using UnityEngine;

namespace NamedAsset
{
    public enum AssetLoadState
    {
        None,
        Loading,
        NoneExist,
        LoadFailed,
        Loaded,
    }

    public class NamedAssetRequest : CustomYieldInstruction
    {
        internal Object asset;
#if UNITY_EDITOR
        internal bool isPrefab;
#endif
        private System.Action<Object> onComplete;
        public AssetLoadState State { get; internal set; }
        public override bool keepWaiting => State <= AssetLoadState.Loading;

        public event System.Action<Object> OnComplete
        {
            add
            {
                if (State > AssetLoadState.Loading )
                {
                    value(asset);
                }
                else
                {
                    onComplete += value;
                }
            }
            remove
            {
                onComplete -= value;
            }
        }

        internal void SetAsset(Object asset)
        {
            this.asset = asset;
            State = asset ? AssetLoadState.Loaded : AssetLoadState.LoadFailed;
            var action = onComplete;
            onComplete = null;
            action?.Invoke(asset);

        }

        public GameObject Instantiate(Transform parent)
        {
#if UNITY_EDITOR
            if (isPrefab)
            {
                return UnityEditor.PrefabUtility.InstantiatePrefab(asset as GameObject, parent) as GameObject;
            }
#endif
            if (asset is GameObject go)
            {
                return Object.Instantiate(go, parent);
            }
            return null;
        }

        public T GetAsset<T>() where T : Object
        {
            return asset as T;
        }

        public static readonly NamedAssetRequest NoneExist = new NamedAssetRequest { State = AssetLoadState.NoneExist };
    }
}
