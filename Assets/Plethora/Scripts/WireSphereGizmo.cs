using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireSphereGizmo : MonoBehaviour
{
    [SerializeField] float radius = 1;
    [SerializeField] Color color = Color.red;

    private void OnDrawGizmos()
    {
        Gizmos.color = color;

        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
