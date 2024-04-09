using Boom;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[System.Serializable]
public class AssetReferenceMaterial : AssetReferenceT<Material>
{
    public AssetReferenceMaterial(string guid) : base(guid)
    {
    }
}
public class ConfigurableMaterial : MonoBehaviour, IAddressableSwap
{
    public static int SwapProcessCount { get; private set; }

    private static readonly Dictionary<int, Material> loadedMaterials = new(); //addressable's HashCode -> material


    [SerializeField] string assetConfigKey;
    [SerializeField] string assetConfigFieldKey;

    [field: SerializeField] public AssetReferenceMaterial DefautAssetReference { get; private set; }
    [SerializeField] Material plainAsset;
    [SerializeField] MeshRenderer meshRenderer;

    private async void Start()
    {
        var key = DefautAssetReference.GetHashCode();
        object assetReference = DefautAssetReference;

        if (ConfigUtil.TryGetConfigFieldAs<string>(BoomManager.Instance.WORLD_CANISTER_ID, assetConfigKey, assetConfigFieldKey, out var config))
        {
            key = Animator.StringToHash($"{assetConfigKey}{assetConfigFieldKey}");
            assetReference = config;
        }

        if (loadedMaterials.TryGetValue(key, out var material) == false)
        {
            ++SwapProcessCount;
            material = await Addressables.LoadAssetAsync<Material>(assetReference);
            --SwapProcessCount;
        }

        if(material != null)
        {
            meshRenderer.material = material;
        }
    }

    private void OnDestroy()
    {
        if(loadedMaterials.Count > 0)
        {
            foreach(var material in loadedMaterials.Values)
            {
                Addressables.Release(material);
            }
            loadedMaterials.Clear();
        }
    }

    public async void SwapToRealAddressable()
    {
        meshRenderer.material = await Addressables.LoadAssetAsync<Material>(DefautAssetReference);

    }

    public void SwapToPlainAsset()
    {
        meshRenderer.material = plainAsset;

    }
}
