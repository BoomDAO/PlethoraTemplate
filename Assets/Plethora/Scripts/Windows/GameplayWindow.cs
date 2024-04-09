namespace PlethoraV2.UI
{
    using Boom.Patterns.Broadcasts;
    using Boom.UI;
    using Cysharp.Threading.Tasks;
    using Newtonsoft.Json;
    using PlethoraV2.Addressables;
    using PlethoraV2.Minigames;
    using TMPro;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class GameplayWindow : Window
    {
        [SerializeField] GameObject getReadyText;
        [SerializeField] GameObject mission;
        [SerializeField] GameObject gameplayInformation;
        [SerializeField] GameObject control;

        [SerializeField] TextMeshProUGUI goalTitleText;
        [SerializeField] TextMeshProUGUI goalScoreText;
        [SerializeField] Image missionLogo;

        [SerializeField] TextMeshProUGUI goalGameplayScoreText;
        [SerializeField] Image missionGameplayLogo;

        [SerializeField] TextMeshProUGUI countdownText;
        [SerializeField] TextMeshProUGUI timerText;

        [SerializeField] TextMeshProUGUI controlActionText;

        [SerializeField] Animator countdownAnimator;

        protected override void Awake()
        {
            base.Awake();
            BroadcastState.Register<BaseMinigameManager.MinigameState>(StateChangeHandler, new BroadcastState.BroadcastSetting() { invokeOnRegistration = true });
            BroadcastState.Register<BaseMinigameManager.ScoreState>(ScoreStateHandler, new BroadcastState.BroadcastSetting() { invokeOnRegistration = true });
            Broadcast.Register<BaseMinigameManager.GameplayTimeLeft>(GameplayDurationHandler);

            BroadcastState.Register<SceneManager.OnLoadingSteteChange>(SceneLoadingStateChangeHandler); 
        }



        private void Start()
        {
            countdownText.text = "GET READY!";
            control.SetActive(false);
            gameplayInformation.SetActive(false);

            countdownAnimator.enabled = true;
        }
        protected void OnDestroy()
        {
            BroadcastState.Unregister<BaseMinigameManager.MinigameState>(StateChangeHandler);
            BroadcastState.Unregister<BaseMinigameManager.ScoreState>(ScoreStateHandler);
            Broadcast.Unregister<BaseMinigameManager.GameplayTimeLeft>(GameplayDurationHandler);

            BroadcastState.Unregister<SceneManager.OnLoadingSteteChange>(SceneLoadingStateChangeHandler);

        }

        private void StateChangeHandler(BaseMinigameManager.MinigameState state)
        {


            if (state.gameState == BaseMinigameManager.GameState.Initiating)
            {
                //mission.SetActive(false);
                //getReadyText.SetActive(false);
                control.SetActive(true);

                if (!countdownAnimator.enabled) countdownAnimator.enabled = true;

                countdownText.text = $"{state.countDown}";

                countdownAnimator.SetTrigger("shirnk");
            }
            else if (state.gameState == BaseMinigameManager.GameState.Playing)
            {
                countdownText.text = "GO!";

                countdownAnimator.SetTrigger("shirnk");
                timerText.text = "60:00";

                gameplayInformation.SetActive(true);
                //control.SetActive(false);
                if (!countdownAnimator.enabled) countdownAnimator.enabled = true;

                CoroutineManagerUtil.DelayAction(() => { countdownAnimator.enabled = false; }, 2.5f, transform);
            }
        }
        private void ScoreStateHandler(BaseMinigameManager.ScoreState state)
        {
            goalGameplayScoreText.text = $"{state.missionGoalProgress}/{state.missionGoal}";
        }

        private void GameplayDurationHandler(BaseMinigameManager.GameplayTimeLeft duration)
        {
            timerText.text = $"{DisplayDeepTime((long)(duration.currentTimeLeft * 1000))}";
        }

        private void SceneLoadingStateChangeHandler(SceneManager.OnLoadingSteteChange change)
        {
            if (change.state == SceneManager.SceneLoadState.PreUnload)
            {
                Close();
            }
        }
        public override bool RequireUnlockCursor()
        {
            return true;
        }

        public async override void Setup(object data)
        {
            if (BaseMinigameManager.MainConfig_ == null) return;

            goalTitleText.text = BaseMinigameManager.MainConfig_.missionTitle;
            goalScoreText.text = $"x{BaseMinigameManager.MainConfig_.missionGoal}";
            goalGameplayScoreText.text = $"{0}/{BaseMinigameManager.MainConfig_.missionGoal}";
            controlActionText.text = BaseMinigameManager.MainConfig_.missionControlAction;

            var missionLogoSprite = await Addressables.LoadAssetAsync<Sprite>(BaseMinigameManager.MainConfig_.missionLogo);

            missionLogo.sprite = missionLogoSprite;
            missionGameplayLogo.sprite = missionLogoSprite;

        }


        public  string DisplayTime(long timeMs, bool includeHours = true)
        {
            long timeSeconds = timeMs / 1000;
            int seconds = Mathf.FloorToInt(timeSeconds % 60);
            int minutes = Mathf.FloorToInt(timeSeconds / 60) % 60;
            int hours = Mathf.FloorToInt(timeSeconds / (60 * 60));

            if (includeHours)
            {
                return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
            }
            else
            {
                return string.Format("{0:00}:{1:00}", minutes, seconds);
            }
        }

        public string DisplayDeepTime(long timeMs, bool singleDigit = false, bool includeHours = false)
        {
            string time;

            if (singleDigit)
            {
                int milliseconds = Mathf.FloorToInt(((float)(timeMs % 1000) / 1000) * 10);
                time = DisplayTime(timeMs, includeHours);

                time += $":{milliseconds}";
            }
            else
            {
                int milliseconds = Mathf.FloorToInt(((float)(timeMs % 1000) / 1000) * 100);
                time = DisplayTime(timeMs, includeHours);

                time += $":{(milliseconds < 10 ? $"0{milliseconds}" : $"{milliseconds}")}";
            }

            return time;
        }
    }
}