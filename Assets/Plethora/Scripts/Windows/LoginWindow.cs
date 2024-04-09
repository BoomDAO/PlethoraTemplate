namespace PlethoraV2.UI
{
    using Boom;
    using Boom.UI;
    using PlethoraV2.Addressables;
    using PlethoraV2.Minigames;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class LoginWindow : Window
    {
        [SerializeField] Button playButton;

        protected override void Awake()
        {
            base.Awake();

            playButton.onClick.AddListener(OnClickHandler);
        }
        private void OnDestroy()
        {
            playButton.onClick.RemoveListener(OnClickHandler);
        }
        private void OnClickHandler()
        {
            if (ConfigUtil.TryGetConfig(BoomManager.Instance.WORLD_CANISTER_ID, "minigame_1", out MainDataTypes.AllConfigs.Config config) == false)
            {
                return;
            }

            playButton.interactable = false;

            var minigameConfig = BaseMinigameManager.SetupMinigameConfig(config, "minigame_1", "minigame_won_1");

            BaseMinigameManager.MainConfig_ = minigameConfig;

            ConfigUtil.TryGetConfigFieldAs<string>(BoomManager.Instance.WORLD_CANISTER_ID, minigameConfig.minigameId, "name", out var minigameName, "None");
            if (string.IsNullOrEmpty(minigameName)) minigameName = "None";

            CoroutineManagerUtil.DelayAction(() =>
            {

                SceneManager.Instance.StartSceneTransition(new SceneManager.TransitionArgKey(minigameConfig.sceneName));
                Close();

            }, 2f, transform);
        }

        public override bool RequireUnlockCursor()
        {
            return true;
        }

        public override void Setup(object data)
        {

        }
    }
}