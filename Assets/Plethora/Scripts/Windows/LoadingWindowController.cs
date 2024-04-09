namespace PlethoraV2.UI {

    using Boom.Patterns.Broadcasts;
    using Boom.UI;
    using PlethoraV2.Addressables;
    using UnityEngine;

    public class LoadingWindowController : MonoBehaviour
    {
        Window window;

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
            if(change.state == SceneManager.SceneLoadState.SceneFadeOut)
            {
                if(window == null)
                {
                    window = WindowManager.Instance.OpenWindow<LoadingWindow>(100);

                    if (window)
                    {
                        if(window.gameObject.TryGetComponent<TransformTweening>(out var tweenComponent))
                        {
                            tweenComponent.PlayFoward();
                        }
                    }
                }
            }
            else if (change.state == SceneManager.SceneLoadState.SceneFadeIn)
            {
                if (window)
                {
                    if (window.gameObject.TryGetComponent<TransformTweening>(out var tweenComponent))
                    {
                        tweenComponent.PlayBackward();
                    }
                }
            }
            else if (change.state == SceneManager.SceneLoadState.SceneFadeInCompleted)
            {
                if (window) window.Close();
            }
        }
    }
}