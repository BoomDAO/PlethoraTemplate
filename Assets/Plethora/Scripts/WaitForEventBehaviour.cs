using System;
using UnityEngine;
using UnityEngine.Events;

public class WaitForEventBehaviour : MonoBehaviour
{
    [SerializeField] bool onStart;
    [SerializeField] float waitTimeSeconds = .5f;
    [SerializeField] UnityEvent onCompleted;

    private void Start()
    {
        if (onStart == false) return;

        Action a = Invoke_;
        a.DelayAction(waitTimeSeconds, transform);
    }

    public void Invoke()
    {
        if (onStart) return;


        Action a = Invoke_;
        a.DelayAction(waitTimeSeconds, transform);
    }

    private void Invoke_()
    {
        onCompleted.Invoke();
    }
}
