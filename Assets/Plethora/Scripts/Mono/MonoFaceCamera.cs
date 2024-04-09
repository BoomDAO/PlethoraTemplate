using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoFaceCamera : MonoBehaviour
{
    Camera mainCamera = null;
    [SerializeField] bool invertLookAt;
    [SerializeField] Vector3 scale = Vector3.one;
    private void Start()
    {
        mainCamera = Camera.main;
    }
    void Update()
    {
        // Find the main camera in the scene

        if (mainCamera != null)
        {
            // Make the object face the main camera
            Vector3 target = mainCamera.transform.position;
            if (scale.x == 0) target.x = transform.position.x;
            if (scale.y == 0) target.y = transform.position.y;
            if (scale.z == 0) target.z = transform.position.z;

            transform.LookAt(invertLookAt? -target:target);
        }
        else
        {
            Debug.LogWarning("Main camera not found in the scene!");
        }
    }
}
