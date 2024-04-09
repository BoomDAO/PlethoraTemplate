namespace PlethoraV2.Utility
{
    using UnityEngine;

    public static class GizmosUtil
    {
        public static void DrawFieldOfView(this Transform source, float range, float fovAngle, Color color)
        {
            Vector3 fovLine1 = Quaternion.Euler(0, fovAngle / 2, 0) * source.forward * range;
            Vector3 fovLine2 = Quaternion.Euler(0, -fovAngle / 2, 0) * source.forward * range;

            Gizmos.color = color;
            Gizmos.DrawRay(source.position, fovLine1);
            Gizmos.DrawRay(source.position, fovLine2);
        }
    }
}