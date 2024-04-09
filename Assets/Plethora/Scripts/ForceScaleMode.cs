using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class ForceScaleMode : MonoBehaviour
{
    [SerializeField] CanvasScaler.ScaleMode scaleMode;
    // Start is called before the first frame update
    void Awake()
    {
        var canvasScaler = GetComponent<CanvasScaler>();

        canvasScaler.uiScaleMode = scaleMode;
    }
}
