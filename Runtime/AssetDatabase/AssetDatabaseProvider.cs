using System.Collections;

namespace NamedAsset
{
    internal class AssetDatabaseProvider : IAssetProvider
    {
        public IEnumerable Initialize(IPathProvider pathProvider)
        {
            throw new System.NotImplementedException();
        }

        public NamedAssetRequest LoadAsset(string name)
        {
            throw new System.NotImplementedException();
        }
        public void Destroy()
        {
            throw new System.NotImplementedException();
        }
    }
}
