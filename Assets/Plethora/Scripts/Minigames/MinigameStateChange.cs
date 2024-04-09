namespace PlethoraV2.Minigames
{
    using Boom.Patterns.Broadcasts;
    using UnityEngine;
    using UnityEngine.Events;

    public class MinigameStateChange : MonoBehaviour
    {

        [SerializeField] BaseMinigameManager.GameState gameState = BaseMinigameManager.GameState.GameOverFailure;
        [SerializeField] UnityEvent onConditionMet;

        private void Awake()
        {
            BroadcastState.Register<BaseMinigameManager.MinigameState>(StateChangeHandler);
        }
        private void OnDestroy()
        {
            BroadcastState.Unregister<BaseMinigameManager.MinigameState>(StateChangeHandler);
        }
        private void StateChangeHandler(BaseMinigameManager.MinigameState state)
        {
            if(state.gameState == gameState)
            {
                onConditionMet.Invoke();
            }
        }
    }
}