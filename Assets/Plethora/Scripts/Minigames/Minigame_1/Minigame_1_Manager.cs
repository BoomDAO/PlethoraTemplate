namespace PlethoraV2.Minigames
{
    using Boom;
    using Boom.Utility;
    using Cysharp.Threading.Tasks;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.AI;
    using Gacha;
    using Patterns.RuntimeSet;
    using Utility;
    using Mono;
    using global::PlethoraV2.Values;
    using Newtonsoft.Json;

    public class Minigame_1_Manager : BaseMinigameManager
    {
        [field: SerializeField] public float PlayerAttackRange { get; private set; } = 4;
        [field: SerializeField] public float PlayerAttackFovAngle { get; private set; } = 100;


        [System.Serializable]
        public struct WaveSetting
        {
            public int enemyCount;
            public float waveDuration;

            public WaveSetting(int enemyCount, float waveDuration)
            {
                this.enemyCount = enemyCount;
                this.waveDuration = waveDuration;
            }
        }

        [SerializeField] LayerMask playerMask;

        [SerializeField] GameObject meleMonsterPrefab;

        List<WaveSetting> waves;
        [SerializeField, ShowOnly] int currentWaveCount = 0;
        [SerializeField] WaveSetting currentWave;


        Coroutine waveRoutine;

        private const float PLAYER_HEALTH_CONFIG_FIELD_VALUE = 100;

        private const float BASIC_ENEMY_POWER_CONFIG_FIELD_VALUE = 5;
        private const float SPECIAL_ENEMY_POWER_1_CONFIG_FIELD_VALUE = 10;
        private const float SPECIAL_ENEMY_POWER_2_CONFIG_FIELD_VALUE = 15;
        private const float BASIC_ENEMY_SPAWM_CHANCE_CONFIG_FIELD_VALUE = 70;
        private const float SPECIAL_ENEMY_SPAWM_CHANCE_1_CONFIG_FIELD_VALUE = 20;
        private const float SPECIAL_ENEMY_SPAWM_CHANCE_2_CONFIG_FIELD_VALUE = 10;


        private const string GAMEPLAY_WAVES_CONFIG_KEY = "minigame_1_system_wave";

        private static List<int> ENEMY_COUNT_PER_WAVE_CONFIG_FIELD_VALUES = new() { 3, 4, 5, 6, 6, 6 };


        private const string ENEMY_MELE_1_CONFIG_KEY = "enemy_chase_mele_1";

        private const string ENEMY_MELE_2_CONFIG_KEY = "enemy_chase_mele_2";


        List<(string skin, string effectSpawn, string effectDeath)> assetsEnemyMeleAddressablesKeys;
        List<float> enemiesSpawnChance;
        List<float> enemiesPower;
        List<float> enemySpeed;
        GachaRoll<(string skin, string effectSpawn, string effectDeath, float power, float speed)> enemiesToSpawn;

        protected override void Start()
        {
            base.Start();

            assetsEnemyMeleAddressablesKeys = new();


            ConfigurationHandler();
        }

        private void ConfigurationHandler()
        {
            GameplayWavesConfigHandler();

            AssetsEnemyMeleConfigHandler(
                ENEMY_MELE_1_CONFIG_KEY,
                ("SkinDoge", "EffectSpawnNormal", "EffectDeathDoge"), AssetOverride.Instance.EnemyChaseMeleModel1);

            AssetsEnemyMeleConfigHandler(
                ENEMY_MELE_2_CONFIG_KEY,
                ("SkinPepe", "EffectSpawnNormal", "EffectDeathPepe"), AssetOverride.Instance.EnemyChaseMeleModel2);

            List<GachaItem<(string skin, string effectSpawn, string effectDeath, float power, float speed)>> gachaRollItems = new();

            Utility.CollectionUtil.Iterate(assetsEnemyMeleAddressablesKeys, (e, i) =>
            {
                var chance = enemiesSpawnChance[i];
                var power = enemiesPower[i];
                var speed = enemySpeed[i];

                gachaRollItems.Add(new GachaItem<(string skin, string effectSpawn, string effectDeath, float power, float speed)>(new (e.skin, e.effectSpawn, e.effectDeath, power, speed), 1, chance));
            });

            enemiesToSpawn = new(gachaRollItems);
        }


        private void GameplayWavesConfigHandler()
        {
            List<int> enemiesCountPerWave = new();

            if (ConfigUtil.TryGetConfig(BoomManager.Instance.WORLD_CANISTER_ID, GAMEPLAY_WAVES_CONFIG_KEY, out var config))
            {
                var tempConfigId = "wave_enemy_count_";
                int waveCount = 0;
                while (true)
                {
                    ++waveCount;
                    if (config.fields.TryGetValue($"{tempConfigId}{waveCount}", out var waveEnemyCount) == false)
                    {
                        //$"Could not find field of id: {tempConfigId} in config of id: {minigameConfigIdMechanics}".Warning();

                        break;
                    }

                    if (double.TryParse(waveEnemyCount, out var _waveEnemyCount) == false)
                    {
                        $"Issue parsing field of id: {tempConfigId} in config of id: {GAMEPLAY_WAVES_CONFIG_KEY}".Error();

                        return;
                    }

                    enemiesCountPerWave.Add((int)_waveEnemyCount);
                }
            }
            else $"Could not find config of id: {GAMEPLAY_WAVES_CONFIG_KEY}".Warning();


            if (enemiesCountPerWave.Count == 0) enemiesCountPerWave = ENEMY_COUNT_PER_WAVE_CONFIG_FIELD_VALUES;

            float durationPerWave = enemiesCountPerWave.Count == 1 ? 60 : 60 / (float)enemiesCountPerWave.Count;

            waves = new();
            Boom.Utility.CollectionUtil.Iterate(enemiesCountPerWave, e =>
            {
                waves.Add(new(e, durationPerWave));
            });
        }

        private void AssetsEnemyMeleConfigHandler(string configId, (string model_, string effectSpawn_, string effectDeath_) assetsDefault, ToggableValue<string> overrideModel)
        {
            if (enemiesSpawnChance == null) enemiesSpawnChance = new();
            if (enemiesPower == null) enemiesPower = new();
            if (enemySpeed == null) enemySpeed = new();

            if (ConfigUtil.TryGetConfig(BoomManager.Instance.WORLD_CANISTER_ID, configId, out var config))
            {
                if (config.fields.TryGetValue("model", out var modelValue) == false)
                {
                    $"Could not find field of id: model in config of id: {configId}".Warning();
                    modelValue = assetsDefault.model_;
                }

                if (config.fields.TryGetValue("effect_spawn", out var effectSpawn) == false)
                {
                    $"Could not find field of id: effect_spawn in config of id: {configId}".Warning();
                    effectSpawn = assetsDefault.effectSpawn_;
                }

                if (config.fields.TryGetValue("effect_death", out var effectDeath) == false)
                {
                    $"Could not find field of id: effect_death in config of id: {configId}".Warning();
                    effectDeath = assetsDefault.effectDeath_;
                }

                if (config.fields.TryGetValue("spawn_chance", out var spawnChance) == false)
                {
                    $"Could not find field of id: spawn_chance in config of id: {configId}".Warning();
                    effectDeath = assetsDefault.effectDeath_;
                }


                if (config.fields.TryGetValue("speed", out var speed) == false)
                {
                    $"Could not find field of id: speed in config of id: {configId}".Warning();
                    effectDeath = assetsDefault.effectDeath_;
                }


                if (config.fields.TryGetValue("power", out var power) == false)
                {
                    $"Could not find field of id: effect_death in config of id: {configId}".Warning();
                    effectDeath = assetsDefault.effectDeath_;
                }

                if (spawnChance.TryParseValue<float>(out float _spawnChance) == false)
                {
                    $"Failure to parse {nameof(spawnChance)} from text to {_spawnChance.GetType().Name}, current value: {spawnChance}".Warning();
                }

                if (power.TryParseValue<float>(out float _power) == false)
                {
                    $"Failure to parse {nameof(spawnChance)} from text to {_power.GetType().Name}, current value: {power}".Warning();
                }

                if (speed.TryParseValue<float>(out float _speed) == false)
                {
                    $"Failure to parse {nameof(speed)} from text to {_speed.GetType().Name}, current value: {speed}".Warning();
                }

                enemiesSpawnChance.Add(_spawnChance);
                enemiesPower.Add(_power);
                enemySpeed.Add(_speed);

                assetsEnemyMeleAddressablesKeys.Add((overrideModel.Enabled ? overrideModel.Value : modelValue, effectSpawn, effectDeath));

            }
            else
            {
                enemiesSpawnChance.Add(100);
                enemiesPower.Add(15);
                enemiesSpawnChance.Add(3.5f);

                $"Could not find config of id: {configId}".Warning();
                assetsEnemyMeleAddressablesKeys.Add((overrideModel.Enabled ? overrideModel.Value : assetsDefault.model_, assetsDefault.effectSpawn_, assetsDefault.effectDeath_));
            }
        }

        protected override void OnInitiatingHandler(int countDown)
        {
        }

        protected override void OnPlayingHandler()
        {
            InitiateNextWave();
        }

        protected override void OnGameOverWonHandler()
        {
            if (waveRoutine != null) StopCoroutine(waveRoutine);
        }

        protected override void OnGameOverLostHandler()
        {
            if(waveRoutine != null) StopCoroutine(waveRoutine);
        }

        private void InitiateNextWave()
        {
            if (currentWaveCount == waves.Count) return;

            currentWave = waves[currentWaveCount];
            ++currentWaveCount;

            if (currentWaveCount == waves.Count)
            {
                //On Last Wave Handler
            }

            waveRoutine = StartCoroutine(WaveRoutine());
        }

        private IEnumerator WaveRoutine()
        {
            //Handle Enemy Spawning
            SpawnEnemyHandler();

            yield return new WaitForSeconds(currentWave.waveDuration);

            InitiateNextWave();
        }

        private void SpawnEnemyHandler()
        {
            var zones = RuntimeSet.Find(RuntimeSet.Group.EnemySpawnpoint, RuntimeSet.Channel.A, playerMask, 7.5f, false);
            if (zones == null) return;

            int zoneCount = zones.Count;
            //Debug.Log($"+ Valid zones count: {zoneCount}");

            int zonesToUseCount = Random.Range(1, Mathf.Min(zoneCount, currentWave.enemyCount));
            //Debug.Log($"+ Zones to use count: {zonesToUseCount}");
            int enemiesPerZone = Mathf.CeilToInt(currentWave.enemyCount / (float)zonesToUseCount);
            //Debug.Log($"+ Enemies per zone: {enemiesPerZone}");

            List<Transform> _randomZones = new();

            LinkedList<Transform> spawnPointsInTheZone= new();
            LinkedList<Transform> spawnPointsUsedInTheZone = new();

            for (int i = 0; i < zonesToUseCount; i++)
            {
                var randomZone = zones.GetRandom(e => !_randomZones.Contains(e));

                if(randomZone == null)
                {
                    $"Something went wrong getting a random zone".Warning();
                    break;
                }

                _randomZones.Add(randomZone);
            }

            int spawnedEnemiesCount = 0;

            foreach (var zone in _randomZones) 
            {
                var enemiesToSpawnCount = spawnedEnemiesCount + enemiesPerZone > currentWave.enemyCount?  currentWave.enemyCount - spawnedEnemiesCount : enemiesPerZone;

                spawnPointsInTheZone.Clear();
                spawnPointsUsedInTheZone.Clear();

                foreach (Transform spawnpoint in zone.transform)
                {
                    spawnPointsInTheZone.AddLast(spawnpoint);
                }

                enemiesToSpawnCount.Iterate(index =>
                {
                    var randomSpawnPoint = spawnPointsInTheZone.GetRandom(e => !spawnPointsUsedInTheZone.Contains(e));

                    if (randomSpawnPoint == null)
                    {
                        $"Something went wrong getting a random SpawnPoint".Warning();
                        return;
                    }
                    spawnPointsUsedInTheZone.AddLast(randomSpawnPoint);
                    SpawnEnemy(randomSpawnPoint).Forget();
                });
            }
        }

        private async UniTaskVoid SpawnEnemy(Transform spawnPoint)
        {
            var enemyClass = enemiesToSpawn.GenerateItem();

            if(enemyClass == null)
            {
                $"Could not spawn enemy".Error();
                return;
            }

            var newEnemy = Instantiate(meleMonsterPrefab);

            newEnemy.transform.position = spawnPoint.position + Vector3.up;
            newEnemy.transform.forward = spawnPoint.forward;

            if (newEnemy.TryGetComponent(out ConfigurableCharacter configurableCharacter) == false)
            {
                $"Failure to find component of name: {nameof(ConfigurableCharacter)}".Error();

                return;
            }

            if(newEnemy.TryGetComponent(out FollowEnemy agent) == false)
            {
                $"Could not find component of {nameof(FollowEnemy)} in spawned agent".Error();

                return;
            }

            HealthComponent healthComponent = newEnemy.GetComponent<HealthComponent>();

            if (healthComponent == null)
            {
                $"Could not find component of {nameof(HealthComponent)} in spawned agent".Error();

                return;
            }

            configurableCharacter.SetupEffects(enemyClass.Value.effectSpawn, enemyClass.Value.effectDeath);

            healthComponent.Heal(1);
            agent.EditForce(enemyClass.Value.power);
            agent.EditSpeed(enemyClass.Value.speed);

            var skinResult = await Addressables.InstantiateAsync(enemyClass.Value.skin);

            configurableCharacter.SetupSkin(skinResult);
        }


        protected override void OnPlayerActionEndHandler()
        {
            var target = RuntimeSet.FindClosest(RuntimeSet.Group.Damageable, RuntimeSet.Channel.A, playerControllerTopdown.RotatingPart, PlayerAttackRange, PlayerAttackFovAngle, (e) =>
            {
                if (e.TryGetComponent<NavMeshAgent>(out var component) == false)
                {
                    return false;
                }
                return component.enabled;
            });


            if (target == false) return;

            if (target.gameObject.TryGetComponent<HealthComponent>(out var component) == false)
            {
                $"Target of name: {target.gameObject.name} is missing a {typeof(HealthComponent)}".Warning(GetType().Name);
                return;
            }

            component.TakeDamage(100);
        }

        private void OnDrawGizmos()
        {
            if (playerControllerTopdown == null) return;

            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(playerControllerTopdown.transform.position, PlayerAttackRange);
            playerControllerTopdown.RotatingPart.transform.DrawFieldOfView(PlayerAttackRange, PlayerAttackFovAngle, Color.red);
        }
    }
}