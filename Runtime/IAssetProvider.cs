namespace NamedAsset
{
    public interface IAssetProvider
    {
        NamedAssetRequest LoadAsset(string name);
    }
}
