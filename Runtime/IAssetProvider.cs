using System.Collections;

namespace NamedAsset
{
    public interface IAssetProvider
    {
        IEnumerable Initialize(IPathProvider pathProvider);
        NamedAssetRequest LoadAsset(string name);

        void Destroy();
    }
}
