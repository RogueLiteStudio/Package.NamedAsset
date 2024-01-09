using System.Collections;

namespace NamedAsset
{
    public interface IAssetProvider
    {
        IEnumerable Initialize();
        NamedAssetRequest LoadAsset(string name);

        void Destroy();
    }
}
