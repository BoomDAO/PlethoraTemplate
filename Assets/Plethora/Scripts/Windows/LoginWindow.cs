namespace PlethoraV2.UI
{
    using Boom;
    using Boom.UI;
    using PlethoraV2.Addressables;
    using PlethoraV2.Minigames;

    public class LoginWindow : Window
    {
        protected override void Awake()
        {
            base.Awake();

            UserUtil.AddListenerMainDataChange<MainDataTypes.LoginData>(OnLoginStateChange);
        }
        private void OnDestroy()
        {
            UserUtil.RemoveListenerMainDataChange<MainDataTypes.LoginData>(OnLoginStateChange);
        }

        private void OnLoginStateChange(MainDataTypes.LoginData data)
        {
            if(data.state == MainDataTypes.LoginData.State.LoggedIn)
            {
                if (ConfigUtil.TryGetConfig(BoomManager.Instance.WORLD_CANISTER_ID, "minigame_1", out MainDataTypes.AllConfigs.Config config) == false)
                {
                    return;
                }

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