namespace PlethoraV2.UI
{
    using Boom.Patterns.Broadcasts;
    using Boom.UI;
    using PlethoraV2.Addressables;

    public class FailureWindow : Window
    {
        protected override void Awake()
        {
            base.Awake();

            BroadcastState.Register<SceneManager.OnLoadingSteteChange>(SceneLoadingStateChangeHandler);
        }


        private void OnDestroy()
        {
            BroadcastState.Unregister<SceneManager.OnLoadingSteteChange>(SceneLoadingStateChangeHandler);
        }
        public override bool RequireUnlockCursor()
        {
            return true;
        }

        public override void Setup(object data)
        {

        }

        private void SceneLoadingStateChangeHandler(SceneManager.OnLoadingSteteChange change)
        {
            if (change.state == SceneManager.SceneLoadState.TransitionInitiated)
            {
                Close();
            }
        }
    }
}