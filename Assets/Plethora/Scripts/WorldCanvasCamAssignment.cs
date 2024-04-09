namespace ItsJackAnton.UI
{
    using Boom.Patterns.Broadcasts;
    using PlethoraV2.Addressables;
    using UnityEngine;

    [RequireComponent(typeof(Canvas))]
    public class WorldCanvasCamAssignment : MonoBehaviour
    {
        protected void Awake()
        {
            AssignCamera();
            BroadcastState.Register<SceneManager.OnLoadingSteteChange>(SceneLoadingStateChangeHandler);
        }

        private void SceneLoadingStateChangeHandler(SceneManager.OnLoadingSteteChange change)
        {
            if(change.state == SceneManager.SceneLoadState.LoadCompleted) AssignCamera();
        }

        private void AssignCamera()
        {
            Canvas _canvas = GetComponent<Canvas>();
            if (_canvas)
            {
                _canvas.worldCamera = Camera.main;
            }
        }

        private void OnDestroy()
        {
            BroadcastState.Unregister<SceneManager.OnLoadingSteteChange>(SceneLoadingStateChangeHandler);
        }
    }
}