using Boom.Utility;
using PlethoraV2.Mono;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOverTime : MonoBehaviour
{
    [SerializeField] List<string> validTags = new();
    [SerializeField] float damageFreq = 1.5f;
    [SerializeField] float damageAmount = 2;
    Dictionary<Transform, long> targets;

    private void Awake()
    {
        targets = new();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out HealthComponent health) == false) return;

        bool skip = true;
        foreach (var t in validTags)
        {
            if (other.CompareTag(t))
            {
                skip = false;
                break;
            }
        }

        if (skip && validTags.Count > 0) return;

        targets.TryAdd(other.transform, 0);
    }

    private void OnTriggerExit(Collider other)
    {
        targets.Remove(other.transform);
    }


    private void OnTriggerStay(Collider other)
    {
        if (targets.TryGetValue(other.transform, out long value) == false) return;
        
        if (value > MainUtil.Now()) return;

        targets[other.transform] = MainUtil.Now() + (int)(damageFreq * 1000);

        other.TryGetComponent(out HealthComponent health);

        health.TakeDamage(damageAmount);
    }
}
