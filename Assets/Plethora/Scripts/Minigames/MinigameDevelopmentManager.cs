using Cysharp.Threading.Tasks;
using PlethoraV2.Patterns.RuntimeSet;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class MinigameDevelopmentManager : MonoBehaviour
{
    [SerializeField] bool enableTemplateWindows;

    [SerializeField] bool disableGameplay;

    [SerializeField] bool forceEnablePlayerControl;


    [SerializeField] AssetReferenceGameObject EnemySkinOverride;

    public static bool DisableGameplay { get; private set; }
    public static bool ForceEnablePlayerControl { get; private set; }

    private void Awake()
    {
        Boom.UI.WindowManager.EnableTemplateWindows = enableTemplateWindows;

        DisableGameplay = disableGameplay;
        ForceEnablePlayerControl = forceEnablePlayerControl;

        RuntimeSet.AddListenerToOnAddEvent(RuntimeSet.Group.Enemy, RuntimeSet.Channel.A, OnMeleEnemySpawnHandler);
    }
    private void OnDestroy()
    {
        RuntimeSet.RemoveListenerToOnRemoveEvent(RuntimeSet.Group.Enemy, RuntimeSet.Channel.A, OnMeleEnemySpawnHandler);
    }
    private void OnMeleEnemySpawnHandler(Transform arg0)
    {
        OverrideMeleEnemySkin(arg0).Forget();
    }

    private async UniTask OverrideMeleEnemySkin(Transform arg0)
    {
        GameObject newSkin = null;

        if (EnemySkinOverride.RuntimeKeyIsValid() == false) return;

        newSkin = await Addressables.InstantiateAsync(EnemySkinOverride);

        newSkin.transform.position = arg0.position;
        newSkin.transform.rotation = arg0.rotation;
        if (arg0.TryGetComponent<ConfigurableCharacter>(out var configurableCharacter) == false)
        {
            Debug.LogError($"Character doesn't not have compoment of type {nameof(ConfigurableCharacter)}");
            return;
        }

        await UniTask.Delay(750);

        configurableCharacter.SetupSkin(newSkin);
    }

    private void Update()
    {
        Boom.UI.WindowManager.EnableTemplateWindows = enableTemplateWindows;

        DisableGameplay = disableGameplay;
        ForceEnablePlayerControl = forceEnablePlayerControl;
    }
}
