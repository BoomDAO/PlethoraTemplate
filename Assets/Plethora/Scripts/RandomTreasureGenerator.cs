using System.Collections.Generic;
using Boom;
using Boom.Patterns.Broadcasts;
using Boom.Utility;
using Cysharp.Threading.Tasks;
using PlethoraV2.Controllers;
using PlethoraV2.Gacha;
using PlethoraV2.Minigames;
using PlethoraV2.Patterns.RuntimeSet;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

public class RandomTreasureGenerator : MonoBehaviour
{
    [System.Serializable]
    public struct RewardData
    {
        public string rewardAssetAddressableKey;
        public float rewardAmount;

        public RewardData(string rewardAssetAddressableKey, float rewardAmount)
        {
            this.rewardAssetAddressableKey = rewardAssetAddressableKey;
            this.rewardAmount = rewardAmount;
        }
    }

    [SerializeField] Transform source;
    [SerializeField] Vector3 spawnOffset;
    [SerializeField] bool lookAtPlayer;

    [SerializeField, Range(0, 10)] float updateCurrentMissionScoreDelay = 1;

    [SerializeField] string gachaConfigIdOverride;
    [SerializeField] GachaRoll<RewardData> gacha = new();
    [SerializeField] UnityEvent<GameObject> onSpawn = new();

    private void Awake()
    {
        if (source == null) source = transform;
    }

    private void Start()
    {
        if (!string.IsNullOrEmpty(gachaConfigIdOverride))
        {
            if(Boom.ConfigUtil.TryGetConfig(BoomManager.Instance.WORLD_CANISTER_ID, gachaConfigIdOverride, out var gachaConfig))
            {
                var configFields = gachaConfig.fields;

                List<GachaItem<RewardData>> gachaItems = new();

                foreach (var item in configFields)
                {
                    float.TryParse(item.Value, out var weight);

                    if (string.IsNullOrEmpty(item.Key) || item.Key.ToLower() == "none")
                    {
                        gachaItems.Add(new(new RewardData("", 0), 1, weight));
                        continue;
                    }

                    if (Boom.ConfigUtil.TryGetConfig(BoomManager.Instance.WORLD_CANISTER_ID, item.Key, out var rewardConfig))
                    {

                        if(rewardConfig.TryGetConfigFieldAs<string>("addressable_asset_key", out var addressablekey))
                        {
                            if (rewardConfig.TryGetConfigFieldAs<float>("reward_amount_gold", out var reward_amount_gold))
                            {
                                gachaItems.Add(new(new RewardData(addressablekey, reward_amount_gold), 1, weight));
                            }
                        }
                    }
                }

                gacha = new GachaRoll<RewardData>(gachaItems);
            }
        }
    }

    public void Instantiate(System.Action<GameObject> onCompleted)
    {
        Instantiate_(onCompleted).Forget();
    }

    public void Instantiate()
    {
        Instantiate(null);
    }

    public async UniTaskVoid Instantiate_(System.Action<GameObject> onCompleted)
    {
        var gachaResult = gacha.GenerateItem();

        var rewardDetails = gachaResult.Value;

        if (string.IsNullOrEmpty(rewardDetails.rewardAssetAddressableKey))
        {
            onCompleted?.Invoke(null);
            return;
        }

        var rewardAsset = await Addressables.InstantiateAsync(rewardDetails.rewardAssetAddressableKey);



        if(!lookAtPlayer) rewardAsset.transform.forward = source.forward;
        else
        {
            var player = RuntimeSet.FindFirst(RuntimeSet.Group.Player, RuntimeSet.Channel.A);

            if (player == null)
            {
                $"Could not find player!".Error();
                return;
            }

            rewardAsset.transform.forward = -player.GetComponent<PlayerControllerTopdown>().RotatingPart.forward;
        }

        rewardAsset.transform.SetParent(source);
        rewardAsset.transform.localScale = Vector3.one;
        rewardAsset.transform.localPosition = Vector3.zero;


        if (updateCurrentMissionScoreDelay > 0)
        {
            System.Action a = () =>
            {
                Broadcast.Invoke(new BaseMinigameManager.AddScore(rewardDetails.rewardAmount));
            };

            a.DelayAction(updateCurrentMissionScoreDelay, transform);
        }
        else
        {
            Broadcast.Invoke(new BaseMinigameManager.AddScore(rewardDetails.rewardAmount));
        }

        onSpawn.Invoke(rewardAsset);

        onCompleted?.Invoke(rewardAsset);
    }

    private void OnDrawGizmos()
    {
        var source_ = source ? source : transform;
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(source_.position + (source_.rotation * spawnOffset), .25f);
    }
}
