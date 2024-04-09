namespace PlethoraV2.Utility
{
    using UnityEngine;

    public static class MathUtil
    {
        public static float LerpUnclamped(float a, float b, float p)
        {
            return (1f - p) * a + b * p;
        }
        public static float MapUnclamped(this float val, float iMin, float iMax, float oMin, float oMax)
        {
            return (val - iMin) * (oMax - oMin) / (iMax - iMin) + oMin;
        }

        public static ulong AddElementTo(this ulong components, ulong element)
        {
            components |= element;

            return components;
        }
        public static ulong RemoveElementTo(this ulong components, ulong element)
        {
            components &= ~element;

            return components;
        }
        public static bool ContainElement(this ulong components, ulong elementName)
        {
            return (components & elementName) != 0;
        }

        public static Vector3 XY3(this Vector2 vector)
        {
            return new Vector3(vector.x, vector.y);
        }
        public static Vector3 XZ3(this Vector2 vector)
        {
            return new Vector3(vector.x, 0, vector.y);
        }
        public static Vector3 Y(this Vector3 vector, float newVal)
        {
            return new Vector3(vector.x, newVal, vector.z);
        }
        public static Vector3 Y0(this Vector3 vector)
        {
            return new Vector3(vector.x, 0, vector.z);
        }

        public static Vector3 AddY(this Vector3 vector, float value)
        {
            return new Vector3(vector.x, vector.y + value, vector.z);
        }
        public static Vector3 MulY(this Vector3 vector, float value)
        {
            return new Vector3(vector.x, vector.y * value, vector.z);
        }

        public static Quaternion RotateSlerp(this Quaternion currentRot, Vector3 desiredDir, float speed = 1)
        {
            Quaternion lookRot = Quaternion.LookRotation(desiredDir);
            Vector3 rotation = Quaternion.Slerp(currentRot, lookRot, Time.deltaTime * speed).eulerAngles;
            return Quaternion.Euler(rotation);
        }
    }
}
