using UnityEngine;

namespace NamedAsset
{
    public enum AssetLoadState
    {
        None,
        Loading,
        LoadFailed,
        Loaded,
    }

    public class NamedAssetRequest : CustomYieldInstruction
    {
        internal Object asset;
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
            State = AssetLoadState.Loaded;
            var action = onComplete;
            onComplete = null;
            action?.Invoke(asset);

        }

        public T GetAsset<T>() where T : Object
        {
            return asset as T;
        }
    }
}
