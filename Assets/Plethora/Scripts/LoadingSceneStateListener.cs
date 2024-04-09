using Boom.Patterns.Broadcasts;
using PlethoraV2.Addressables;
using UnityEngine;
using UnityEngine.Events;
using static PlethoraV2.Addressables.SceneManager;

public class LoadingSceneStateListener : MonoBehaviour
{
    [SerializeField] SceneLoadState sceneLoadState;
    [SerializeField] UnityEvent onConditionMet;


    private void Awake()
    {
        BroadcastState.Register<SceneManager.OnLoadingSteteChange>(OnStateChangeHandler, new BroadcastState.BroadcastSetting() { invokeOnRegistration = true });
    }

    private void OnDestroy()
    {
        BroadcastState.Unregister<SceneManager.OnLoadingSteteChange>(OnStateChangeHandler);
    }

    private void OnStateChangeHandler(SceneManager.OnLoadingSteteChange change)
    {
        if (sceneLoadState == change.state)
        {
            onConditionMet.Invoke();
        }
    }
}
