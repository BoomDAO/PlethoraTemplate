using Boom.Utility;
using Cysharp.Threading.Tasks;
using PlethoraV2.Patterns.RuntimeSet;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class DynamicAddressableSpawner : MonoBehaviour
{
    [SerializeField] Transform source;
    [SerializeField] Vector3 offset;
    private void Awake()
    {
        if (source == null) source = transform;
    }

    public void Spawn(string addressableKey)
    {
        Spawn_(addressableKey).Forget();
    }

    private async UniTaskVoid Spawn_(string addressableKey)
    {
        var go = await Addressables.InstantiateAsync(addressableKey);

        go.transform.SetParent(source);

        go.transform.localPosition = offset;
        go.transform.localScale = Vector3.one;
        go.transform.forward = source.forward;

    }
}
