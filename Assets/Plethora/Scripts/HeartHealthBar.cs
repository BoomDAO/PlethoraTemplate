using Boom.Utility;
using PlethoraV2.Mono;
using PlethoraV2.Patterns.RuntimeSet;
using PlethoraV2.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartHealthBar : MonoBehaviour
{
    [SerializeField] Image[] hearts = new Image[0];

    [SerializeField, Range(0,1)] float testHealthController;

    private HealthComponent healthComponent;

    private void Awake()
    {
        UpdateHealthBar(1);

        var player = RuntimeSet.FindFirst(RuntimeSet.Group.Player, RuntimeSet.Channel.A);

        if (player == null)
        {
            $"Player could not be found".Error(GetType().Name);
            return;
        }

        if(player.TryGetComponent<HealthComponent>(out healthComponent) == false)
        {
            $"Player's health component could not be found".Error(GetType().Name);
            return;
        }


        healthComponent.OnHealthChange.AddListener(OnHealthChangeHandler);
    }
    private void OnDestroy()
    {
        healthComponent.OnHealthChange.RemoveListener(OnHealthChangeHandler);
    }

    private void OnHealthChangeHandler(HealthComponent.HealthChangeInfo arg0)
    {
        float perc = arg0.newHealthValue / arg0.maxHealth;
        UpdateHealthBar(perc);
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying) return;
        UpdateHealthBar(testHealthController);
    }
    public void UpdateHealthBar(float perc)
    {
        float fraction = 1f / hearts.Length;

        for (int i = 0; i < hearts.Length; i++)
        {
            float heartFillAmount = perc > fraction? fraction: perc;
            perc -= fraction;
            if (heartFillAmount < 0) heartFillAmount = 0;
            if (hearts[i] != null) hearts[i].fillAmount = Mathf.Clamp01(MathUtil.MapUnclamped(heartFillAmount, 0, fraction, 0, 1));
        }
    }
}
