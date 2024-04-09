namespace PlethoraV2.UI
{
    using System;
    using Boom;
    using Boom.Patterns.Broadcasts;
    using Boom.UI;
    using PlethoraV2.Addressables;
    using PlethoraV2.Minigames;
    using TMPro;
    using UnityEngine;

    public class SuccessWindow : Window
    {
        [SerializeField] TMP_Text scoreText;

        public override Type[] GetConflictWindow()
        {
            return new Type[] { typeof(GameplayWindow) };
        }
        protected override void Awake()
        {
            base.Awake();

            BroadcastState.Register<SceneManager.OnLoadingSteteChange>(SceneLoadingStateChangeHandler);
            BroadcastState.Register<BaseMinigameManager.MinigameState>(MinigameStateChangeHandler);
        }

        private void Start()
        {
        }
        private void OnDestroy()
        {
            BroadcastState.Unregister<SceneManager.OnLoadingSteteChange>(SceneLoadingStateChangeHandler);
            BroadcastState.Unregister<BaseMinigameManager.MinigameState>(MinigameStateChangeHandler);
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

        private void MinigameStateChangeHandler(BaseMinigameManager.MinigameState state)
        { 
            if(state.gameState == BaseMinigameManager.GameState.GameOverSuccess)
            {
                if (state.reward != null)
                {
                    scoreText.text = $"EXP +{state.reward.Value}";
                }
            }
        }
    }
}