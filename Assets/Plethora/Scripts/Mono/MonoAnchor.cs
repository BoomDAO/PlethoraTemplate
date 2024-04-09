using Boom.Patterns.Broadcasts;
using PlethoraV2.Input;
using PlethoraV2.Minigames;
using PlethoraV2.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoAnchor : MonoBehaviour
{
    public enum UpdateType { Update, FixedUpdate, LateUpdate }

    [SerializeField] Transform source;
    [SerializeField] Vector3 offset;
    [SerializeField] UpdateType updateType = UpdateType.Update;

    [SerializeField] bool smoothing;
    [SerializeField] float smoothSpeed = 0.125f;

    BaseMinigameManager.GameState gameState;

    private void Awake()
    {
        BroadcastState.Register<BaseMinigameManager.MinigameState>(StateChangeHandler, new BroadcastState.BroadcastSetting() { invokeOnRegistration = true });
    }

    private void OnDestroy()
    {
        BroadcastState.Unregister<BaseMinigameManager.MinigameState>(StateChangeHandler);
    }


    private void StateChangeHandler(BaseMinigameManager.MinigameState state)
    {
        gameState = state.gameState;
    }
    private void Update()
    {
        if (updateType != UpdateType.Update) return;
        MovementHandler();
    }
    private void FixedUpdate()
    {
        if (updateType != UpdateType.FixedUpdate) return;
        MovementHandler();
    }
    private void LateUpdate()
    {
        if (updateType != UpdateType.LateUpdate) return;
        MovementHandler();
    }

    private void MovementHandler()
    {
        if (gameState != BaseMinigameManager.GameState.Playing) return;

        if (source)
        {
            var nextPosition = source.position + offset;
            var movementInputSqrMagnitud = Vector3.Distance(transform.position.Y(0), nextPosition.Y(0));
            var moveTarget = movementInputSqrMagnitud > 0.01f;

            if (smoothing)
            {
                transform.position = moveTarget ? Vector3.Lerp(transform.position, nextPosition, smoothSpeed) : transform.position;
            }
            else transform.position = moveTarget ? nextPosition : transform.position;
        }
    }
}
