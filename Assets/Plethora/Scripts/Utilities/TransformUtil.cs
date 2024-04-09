namespace PlethoraV2.Utility
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public static class TransformUtil
    {
        public static bool IsInView(this Transform target, Transform source, float radius = float.PositiveInfinity, float fovAngle = 360)
        {
            if(!target) return false;
            Vector3 targetPosition = target.position;

            float dist = Vector3.Distance(targetPosition, source.position);

            //CHECK IF TARGET IS IN RANGE
            if (dist > radius) return false;

            //CHECK FOV ANGLE
            if (fovAngle < 360)
            {
                Vector3 dirToTarget = (targetPosition - source.position).normalized;

                float angleToTarget = Vector3.Angle(source.forward, dirToTarget);

                if (angleToTarget <= fovAngle / 2f == false) return false;
            }

            return true;
        }
        public static Transform FindClosest(this LinkedList<Transform> list, Vector3 position, float radius = float.PositiveInfinity, Predicate<Transform> filter = null)
        {
            LinkedListNode<Transform> runner = list.First;

            Transform closest = null;
            float closestDist = Mathf.Infinity;

            if (filter == null)
            {

                while (runner != null)
                {
                    Vector3 targetPosition = runner.Value.position;

                    float dist = Vector3.Distance(targetPosition, position);

                    //CHECK IF TARGET IS IN RANGE
                    if (dist > radius) goto end;

                    //CHECK IF TARGET IS NEW CLOSEST
                    if (dist > closestDist) goto end;

                    //SET CLOSEST
                    closestDist = dist;
                    closest = runner.Value;

                end:

                    runner = runner.Next;
                }
            }
            else
            {

                while (runner != null)
                {
                    Vector3 targetPosition = runner.Value.position;

                    float dist = Vector3.Distance(targetPosition, position);

                    //CHECK IF TARGET WILL PASS THE FILTER
                    if (filter(runner.Value) == false) goto end;

                    //CHECK IF TARGET IS IN RANGE
                    if (dist > radius) goto end;

                    //CHECK IF TARGET IS NEW CLOSEST
                    if (dist > closestDist) goto end;

                    //SET CLOSEST
                    closestDist = dist;
                    closest = runner.Value;

                end:

                    runner = runner.Next;
                }
            }


            return closest;
        }
        public static LinkedList<Transform> FindAllClosestTransform(this LinkedList<Transform> list, Vector3 position, float radius = float.PositiveInfinity, Predicate<Transform> filter = null)
        {
            LinkedListNode<Transform> runner = list.First;

            LinkedList<Transform> found = new();

            if (filter == null)
            {

                while (runner != null)
                {
                    Vector3 targetPosition = runner.Value.position;

                    float dist = Vector3.Distance(targetPosition, position);

                    //CHECK IF TARGET IS IN RANGE
                    if (dist > radius) goto end;

                    //ADD TARGET TO LIST
                    found.AddLast(runner.Value);

                end:

                    runner = runner.Next;
                }
            }
            else
            {
                while (runner != null)
                {
                    Vector3 targetPosition = runner.Value.position;

                    float dist = Vector3.Distance(targetPosition, position);

                    //CHECK IF TARGET WILL PASS THE FILTER
                    if (filter(runner.Value) == false) goto end;

                    //CHECK IF TARGET IS IN RANGE
                    if (dist > radius) goto end;

                    //ADD TARGET TO LIST
                    found.AddLast(runner.Value);

                    end:

                    runner = runner.Next;
                }
            }


            return found;
        }
        public static Transform FindClosestTransform(this LinkedList<Transform> list, Transform source, float radius = float.PositiveInfinity, float fovAngle = 360, Predicate<Transform> filter = null)
        {
            LinkedListNode<Transform> runner = list.First;

            Transform closest = null;
            float closestDist = Mathf.Infinity;

            if (filter == null)
            {
                while (runner != null)
                {
                    Vector3 targetPosition = runner.Value.position;

                    float dist = Vector3.Distance(targetPosition, source.position);

                    //CHECK IF TARGET IS IN RANGE
                    if (dist > radius) goto end;

                    //CHECK FOV ANGLE
                    if (fovAngle < 360)
                    {
                        Vector3 dirToTarget = (targetPosition - source.position).normalized;

                        float angleToTarget = Vector3.Angle(source.forward, dirToTarget);

                        if (angleToTarget <= fovAngle / 2f == false) goto end;
                    }

                    //CHECK IF TARGET IS NEW CLOSEST
                    if (dist > closestDist) goto end;

                    //SET CLOSEST
                    closestDist = dist;
                    closest = runner.Value;

                    end:

                    runner = runner.Next;
                }
            }
            else
            {
                while (runner != null)
                {
                    Vector3 targetPosition = runner.Value.position;

                    float dist = Vector3.Distance(targetPosition, source.position);

                    //CHECK IF TARGET WILL PASS THE FILTER
                    if (filter(runner.Value) == false) goto end;

                    //CHECK IF TARGET IS IN RANGE
                    if (dist > radius) goto end;

                    //CHECK FOV ANGLE
                    if (fovAngle < 360)
                    {
                        Vector3 dirToTarget = (targetPosition - source.position).normalized;

                        float angleToTarget = Vector3.Angle(source.forward, dirToTarget);

                        if (angleToTarget <= fovAngle / 2f == false) goto end;
                    }

                    //CHECK IF TARGET IS NEW CLOSEST
                    if (dist > closestDist) goto end;

                    //SET CLOSEST
                    closestDist = dist;
                    closest = runner.Value;

                    end:

                    runner = runner.Next;
                }
            }


            return closest;
        }
        public static LinkedList<Transform> FindAllClosestTransform(this LinkedList<Transform> list, Transform source, float radius = float.PositiveInfinity, float fovAngle = 1, Predicate<Transform> filter = null)
        {
            LinkedListNode<Transform> runner = list.First;

            LinkedList<Transform> found = new();

            if(filter == null)
            {
                while (runner != null)
                {
                    Vector3 targetPosition = runner.Value.transform.position;

                    float dist = Vector3.Distance(targetPosition, source.position);

                    //CHECK IF TARGET IS IN RANGE
                    if (dist > radius) goto end;

                    //CHECK FOV ANGLE
                    if (fovAngle < 360)
                    {
                        Vector3 dirToTarget = (targetPosition - source.position).normalized;

                        float angleToTarget = Vector3.Angle(source.forward, dirToTarget);

                        if (angleToTarget <= fovAngle / 2f == false) goto end;
                    }

                    //ADD TARGET TO LIST
                    found.AddLast(runner.Value);

                    end:

                    runner = runner.Next;
                }
            }
            else
            {
                while (runner != null)
                {
                    Vector3 targetPosition = runner.Value.transform.position;

                    float dist = Vector3.Distance(targetPosition, source.position);

                    //CHECK IF TARGET WILL PASS THE FILTER
                    if (filter(runner.Value) == false) goto end;

                    //CHECK IF TARGET IS IN RANGE
                    if (dist > radius) goto end;

                    //CHECK FOV ANGLE
                    if (fovAngle < 360)
                    {
                        Vector3 dirToTarget = (targetPosition - source.position).normalized;

                        float angleToTarget = Vector3.Angle(source.forward, dirToTarget);

                        if (angleToTarget <= fovAngle / 2f == false) goto end;
                    }

                    //ADD TARGET TO LIST
                    found.AddLast(runner.Value);

                    end:

                    runner = runner.Next;
                }
            }

            return found;
        }
    }
}