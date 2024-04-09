
using Boom;
using Cysharp.Threading.Tasks;
using PlethoraV2.Addressables;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public interface IAddressableSwap
{
    public void SwapToRealAddressable();
    public void SwapToPlainAsset();
}
public class ConfigurablePrefab : MonoBehaviour, IAddressableSwap
{
    public static int SwapProcessCount { get; private set; }

    [SerializeField] string assetConfigKey;
    [SerializeField] string assetConfigFieldKey;

    [field: SerializeField] public AssetReferenceGameObject DefautAssetReference { get; private set; }

    [SerializeField] Transform addressableHolder;
    [SerializeField] GameObject plainAssetHolder;


    private void Start()
    {
        object assetReference = DefautAssetReference;

        if (ConfigUtil.TryGetConfigFieldAs<string>(BoomManager.Instance.WORLD_CANISTER_ID, assetConfigKey, assetConfigFieldKey, out var config))
        {
            assetReference = config;
        }

        SceneManager.Instance.AddListenerToAddressableInitializationCompleted(() =>
        {
            LoadAsset(assetReference).Forget();
        });
    }

    private async UniTaskVoid LoadAsset(object assetReference)
    {
        if (Application.isPlaying == false)
            Debug.Log("Load addressable");

        Transform newGoTransform = null;

        ++SwapProcessCount;
        try
        {
            var e = await Addressables.InstantiateAsync(assetReference, null, true);
            newGoTransform = e.transform;
        }
        catch
        {
            Debug.LogError("Issue loading addressable on GameObject of name " + name);
        }
        --SwapProcessCount;

        if (newGoTransform == null) return;

        foreach (Transform child in addressableHolder)
        {
            Addressables.Release(child.gameObject);
        }

        if (Application.isPlaying == false)
            Debug.Log($"Assigning addressable to parent gameobject of name: {addressableHolder}");
        newGoTransform.parent = addressableHolder;
        newGoTransform.localPosition = Vector3.zero;
        newGoTransform.localRotation = default;
        newGoTransform.localScale = Vector3.one;

        plainAssetHolder.SetActive(false);
    }

    public void SwapToRealAddressable()
    {
        LoadAsset(DefautAssetReference);
    }

    public void SwapToPlainAsset()
    {
        foreach (Transform child in addressableHolder)
        {
            Addressables.Release(child.gameObject);
        }
        plainAssetHolder.SetActive(true);
    }
}
