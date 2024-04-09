namespace PlethoraV2.Mono
{
    using PlethoraV2.Interfaces;
    using PlethoraV2.Structs;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public class HealthComponent : MonoBehaviour, IDamagable
    {
        public enum HealthState
        {
            Well, Hurt, Dead
        }
        public enum HealthChangeType { Damage, Heal, SettingUpdate }
        public struct HealthChangeInfo
        {
            public float maxHealth;
            public float oldHealthValue;
            public float newHealthValue;
            public float modifier;
            public HealthChangeType healthChangeType;

            public HealthChangeInfo(float maxHealth, float oldHealthValue, float newHealthValue, float modifier, HealthChangeType healthChangeType)
            {
                this.maxHealth = maxHealth;
                this.oldHealthValue = oldHealthValue;
                this.newHealthValue = newHealthValue;
                this.modifier = modifier;
                this.healthChangeType = healthChangeType;
            }
        }

        public struct HealthSetting
        {
            public float? maxHealth;
            public float? currentHealth;
            public float? invulnarabilityDuration;
        }

        public float maxHealth = 100;
        public float currentHealth = 0;
        public float invulnarabilityDuration = .3f;

        [SerializeField] bool startWithMaxHealth;

        [SerializeField, ShowOnly] float currentinvulnarabilityDuration = 0;
        [SerializeField, ShowOnly] bool isInvulnerable;

        public HealthState _HealthState {  get {
                if(currentHealth == maxHealth) return HealthState.Well;
                else if (currentHealth == 0) return HealthState.Dead;
                else return HealthState.Hurt;
            } 
        }

        [field: SerializeField] public UnityEvent<HealthChangeInfo> OnHealthChange { get; private set; }

        [field: SerializeField] public UnityEvent OnRevive { get; private set; }
        [field: SerializeField] public UnityEvent OnDeath { get; private set; }

        private void OnEnable()
        {
            if(startWithMaxHealth) Heal(maxHealth - currentHealth);
        }

        public bool TakeDamage(float damageAmount)
        {
            if (isInvulnerable) return false;

            if(_HealthState == HealthState.Dead) return false;

            float oldHealth = currentHealth;
            float newCurrentHealth = currentHealth - damageAmount;
            if (newCurrentHealth < 0) newCurrentHealth = 0;

            if(currentHealth != newCurrentHealth)
            {
                bool justDied = currentHealth > 0 & newCurrentHealth == 0;
                currentHealth = newCurrentHealth;

                OnHealthChange.Invoke(new (maxHealth, oldHealth, currentHealth, damageAmount, HealthChangeType.Damage));

                TryEnableInvulnerability();

                if (justDied)
                {
                    OnDeath.Invoke();
                }
            }

            return false;
        }

        public bool Heal(float healAmount)
        {
            float oldHealth = currentHealth;
            float newCurrentHealth = currentHealth + healAmount;
            if (newCurrentHealth > maxHealth) newCurrentHealth = maxHealth;

            if (currentHealth != newCurrentHealth)
            {
                bool revived = currentHealth == 0 & newCurrentHealth > 0;
                currentHealth = newCurrentHealth;

                OnHealthChange.Invoke(new(maxHealth, oldHealth, currentHealth, healAmount, HealthChangeType.Heal));

                if (revived) OnRevive.Invoke();
            }

            return false;
        }

        private void TryEnableInvulnerability()
        {
            if (isInvulnerable && invulnarabilityDuration == 0) return;
            isInvulnerable = true;
            currentinvulnarabilityDuration = invulnarabilityDuration;
            StartCoroutine(WaitForInvulterabilityToDeplete());
        }

        IEnumerator WaitForInvulterabilityToDeplete()
        {
            while (currentinvulnarabilityDuration > 0) 
            {
                currentinvulnarabilityDuration -= Time.deltaTime;
                yield return null;
            }    
            isInvulnerable = false;
        }

        public void UpdateSettings(HealthSetting settings)
        {
            if(settings.maxHealth.HasValue )
            {
                maxHealth = settings.maxHealth.Value;
            }
            if(settings.currentHealth.HasValue)
            {
                var oldHealthValue = currentHealth;
                currentHealth = settings.currentHealth.Value;
                OnHealthChange.Invoke(new(maxHealth, oldHealthValue, currentHealth, 0, HealthChangeType.SettingUpdate));
            }
            if (settings.invulnarabilityDuration.HasValue)
            {
                invulnarabilityDuration = settings.invulnarabilityDuration.Value;
                currentinvulnarabilityDuration = invulnarabilityDuration;
            }

        }
    }
}
