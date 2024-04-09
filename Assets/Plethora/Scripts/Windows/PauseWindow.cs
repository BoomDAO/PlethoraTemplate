namespace PlethoraV2.UI
{
    using Boom.Patterns.Broadcasts;
    using Boom.UI;
    using Boom.Utility;
    using PlethoraV2.Addressables;
    using System.Collections.Generic;
    using UnityEngine;

    public class PauseWindow : Window
    {
        [SerializeField] private List<TransformTweening> closeAnimation;
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
                //Close();
            }
        }

        public override void Close()
        {
            base.Close();
            Time.timeScale = 1f;
        }

        public void PlayCloseAnimation()
        {
            closeAnimation.Iterate(e => e.PlayBackward());
        }
    }
}