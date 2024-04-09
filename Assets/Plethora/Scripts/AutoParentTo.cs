using System;
using System.Collections;
using System.Collections.Generic;
using PlethoraV2.Patterns.RuntimeSet;
using UnityEngine;

[ExecuteInEditMode]
public class AutoParentTo : MonoBehaviour
{
    [SerializeField] RuntimeSet.Group group = RuntimeSet.Group.Any;
    [SerializeField] Transform pivot;



    private void Awake()
    {
        if (Application.isPlaying == false) return;
        RuntimeSet.AddListenerToOnAddEvent(group, RuntimeSet.Channel.A, OnAddTargetHandler);
    }

    private void OnDestroy()
    {
        if (Application.isPlaying == false) return;

        RuntimeSet.RemoveListenerToOnRemoveEvent(group, RuntimeSet.Channel.A, OnAddTargetHandler);
    }

    private void OnAddTargetHandler(Transform arg0)
    {
        pivot = arg0;
    }

    private void Update()
    {
        HandleTransform();
    }

    private void HandleTransform()
    {
        if (pivot == null) return;

        transform.position = pivot.position;
        transform.rotation = pivot.rotation;
    }
}
