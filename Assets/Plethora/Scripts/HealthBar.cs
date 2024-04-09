namespace PlethoraV2.UI
{
    using PlethoraV2.Mono;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class HealthBar : MonoBehaviour
    {
        [SerializeField] HealthComponent healthComponent;
        [SerializeField] Image progressBar;
        [SerializeField] float progressChangeSpeed = 500;
        [SerializeField, ShowOnly] float currentProgressChangeSpeed;
        [SerializeField] Gradient gradient;
        Coroutine progressCoroutine;

        private void Awake()
        {
            healthComponent.OnHealthChange.AddListener(OnHealthChangeHandler);
        }

        private void Start()
        {
            progressBar.fillAmount = healthComponent.currentHealth / healthComponent.maxHealth;

            if(HealthBarCanvas.Instance)
            {
                transform.SetParent(HealthBarCanvas.Instance.transform);
            }
        }

        private void OnDestroy()
        {
            healthComponent.OnHealthChange.RemoveListener(OnHealthChangeHandler);
        }

        private void OnHealthChangeHandler(HealthComponent.HealthChangeInfo arg0)
        {
            float progress = arg0.newHealthValue / arg0.maxHealth;
            float diff = Mathf.Abs(progressBar.fillAmount - progress);

            currentProgressChangeSpeed = progressChangeSpeed * (diff * .1f);
            if (progressCoroutine != null)
            {
                StopCoroutine(progressCoroutine);
            }
            progressCoroutine = StartCoroutine(AnimateProgress(progress, currentProgressChangeSpeed));
        }

        IEnumerator AnimateProgress(float progress, float speed)
        {
            float time = 0;
            float initialProgress = progressBar.fillAmount;

            while (time < 1)
            {
                progressBar.fillAmount = Mathf.Lerp(initialProgress, progress, time);
                time += Time.deltaTime * speed;

                progressBar.color = gradient.Evaluate(1 - progress);

                yield return null;
            }

            progressBar.fillAmount = progress;
            progressBar.color = gradient.Evaluate(1 - progress);
        }
    }
}