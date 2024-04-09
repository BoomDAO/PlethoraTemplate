using Boom.Patterns.Broadcasts;
using PlethoraV2.Minigames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncrementCurrentMissionScore : MonoBehaviour
{
    [SerializeField] Vector2 incrementRange;
    public void Increment()
    {
        Broadcast.Invoke(new BaseMinigameManager.AddScore((int)Random.Range(incrementRange.x, incrementRange.y)));
    }
}
