namespace PlethoraV2.Minigames
{
    using System.Collections;
    using Boom;
    using Boom.Patterns.Broadcasts;
    using Boom.UI;
    using Boom.Utility;
    using UnityEngine;
    using Cysharp.Threading.Tasks;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using Addressables;
    using UI;
    using Mono;
    using Controllers;
    using Input;
    using Patterns.RuntimeSet;
    using static global::PlethoraV2.Minigames.BaseMinigameManager;

    public abstract class BaseMinigameManager : MonoBehaviour
    {
        public class MainConfig
        {
            public string minigameId;

            public bool isEnabled;

            public string name;
            public string coverImage;
            public string description;
            public string sceneName;

            public string missionTitle;
            public int missionGoal;
            public string missionLogo;

            public int durationSeconds;

            public int playerHealth;
            public string playerModel;
            public string playerWeaponModel;
            public string winAction;

            public string missionControlAction;

            public MainConfig() { }
            public MainConfig(string minigameId, bool isEnabled, string name, string coverImage, string description, string sceneName, string missionTitle, int missionGoal, string missionLogo, string missionControlAction, int durationSeconds, int playerHealth, string playerModel, string playerWeaponModel, string winAction)
            {
                this.minigameId = minigameId;
                this.isEnabled = isEnabled;
                this.name = name;
                this.coverImage = coverImage;
                this.description = description;
                this.sceneName = sceneName;
                this.missionTitle = missionTitle;
                this.missionGoal = missionGoal;
                this.missionLogo = missionLogo;
                this.missionControlAction = missionControlAction;
                this.durationSeconds = durationSeconds;
                this.playerHealth = playerHealth;
                this.playerModel = playerModel;
                this.playerWeaponModel = playerWeaponModel;
                this.winAction = winAction; 
            }
        }

        public static MainConfig MainConfig_ = null;
        public static MainConfig SetupMinigameConfig(MainDataTypes.AllConfigs.Config mainConfig_, string minigameId, string winAction)
        {
            //IsEnable

            if (mainConfig_.fields.TryGetValue("is_enabled", out string fieldTextValue) == false)
            {
                fieldTextValue = "false";
            }
            else fieldTextValue = fieldTextValue.ToLower();

            var isEnable = fieldTextValue == "true";


            //Name

            if (mainConfig_.fields.TryGetValue("name", out fieldTextValue) == false)
            {
                fieldTextValue = "Minigame";
            }

            var name = fieldTextValue;


            //URL

            mainConfig_.fields.TryGetValue("cover_image", out fieldTextValue);

            var coverImage = fieldTextValue;


            //Description

            if (mainConfig_.fields.TryGetValue("description", out fieldTextValue) == false)
            {
                fieldTextValue = "";
            }

            var description = fieldTextValue;


            //SceneName

            if (mainConfig_.fields.TryGetValue("scene_name", out fieldTextValue) == false)
            {
                fieldTextValue = "";
            }

            var sceneName = fieldTextValue;

            //GoalTitle

            if (mainConfig_.fields.TryGetValue("mission_title", out fieldTextValue) == false)
            {
                fieldTextValue = "Get 20 points";
            }

            var missionTitle = fieldTextValue;


            //GoalScore

            if (mainConfig_.fields.TryGetValue("mission_goal", out fieldTextValue) == false)
            {
                fieldTextValue = "20";
            }

            if (int.TryParse(fieldTextValue, out var missionGoal) == false)
            {
                missionGoal = 20;
            }

            //MissionLogo

            if (mainConfig_.fields.TryGetValue("mission_logo", out fieldTextValue) == false)
            {
                fieldTextValue = "MissionLogoKill";
            }

            var missionLogo = fieldTextValue;

            //MissionControl

            if (mainConfig_.fields.TryGetValue("mission_control_action", out fieldTextValue) == false)
            {
                fieldTextValue = "USE WASD OR ARROWS <color=green>TO MOVE</color>\nPRESS SPACEBAR TO <color=green>ATTACK</color> ENEMIES";
            }

            var missionControlAction = fieldTextValue;

            //DurationSeconds

            if (mainConfig_.fields.TryGetValue("duration_seconds", out fieldTextValue) == false)
            {
                fieldTextValue = "180.0";
            }

            if (int.TryParse(fieldTextValue, out var durationSeconds) == false)
            {
                durationSeconds = 180;
            }

            //PlayerHealth

            if (mainConfig_.fields.TryGetValue("player_health", out fieldTextValue) == false)
            {
                fieldTextValue = "260";
            }

            if(int.TryParse(fieldTextValue, out var playerHealth) == false)
            {
                playerHealth = 260;
            }


            //PlayerModel

            if (mainConfig_.fields.TryGetValue("player_model", out fieldTextValue) == false)
            {
                fieldTextValue = "SkinMoonwalkerDefault";
            }

            var playerModel = fieldTextValue;

            if (AssetOverride.Instance.PlayerModelOverride.Enabled)
            {
                playerModel = AssetOverride.Instance.PlayerModelOverride.Value;
            }

            //PlayerWeaponModel

            if (mainConfig_.fields.TryGetValue("player_weapon_model", out fieldTextValue) == false)
            {
                fieldTextValue = "Hammer";
            }

            var playerWeaponModel = fieldTextValue;

            MainConfig mainconfig = new(minigameId, isEnable, name, coverImage, description, sceneName, missionTitle, missionGoal, missionLogo, missionControlAction, durationSeconds, playerHealth, playerModel, playerWeaponModel, winAction);
            return mainconfig;
        }

        public enum GameState
        {
            None,
            Initiate,
            Initiating,
            Playing,
            Paused,
            GameOverSuccess,
            GameOverFailure,
            Quit
        }

        public enum GameplayDurationState
        {
            Initiating,
            Depleting,
        }

        public enum ScoreType
        {
            x1 = 1,
            x2 = 2,
            x3 = 3,
            x5 = 5,
            x10 = 10,
            x15 = 15,
            x25 = 25,
            x50 = 50,
            x100 = 100,
        }

        //
        public struct AddScore : IBroadcast
        {
            public float score;

            public AddScore(ScoreType score)
            {
                this.score = (int)score;
            }
            public AddScore(float score)
            {
                this.score = score;
            }
        }

        //
        public struct ScoreState : IBroadcastState
        {
            public float missionGoal;
            public float missionGoalProgress;

            public ScoreState(float missionGoalProgress, float missionGoal)
            {
                this.missionGoalProgress = missionGoalProgress;
                this.missionGoal = missionGoal;
            }

            public int MaxSavedStatesCount()
            {
                return 0;
            }
        }

        public struct MinigameState : IBroadcastState
        {
            public GameState gameState;
            public int countDown;
            public double? reward;

            public MinigameState(GameState gameState, int countDown, double? reward = null)
            {
                this.gameState = gameState;
                this.countDown = countDown;
                this.reward = reward;
            }

            public int MaxSavedStatesCount()
            {
                return 0;
            }
        }

        public struct GameplayTimeLeft : IBroadcast
        {
            public float currentTimeLeft;
            public GameplayDurationState state;

            public GameplayTimeLeft(float currentDuration, GameplayDurationState gameplayDurationState)
            {
                this.currentTimeLeft = currentDuration;
                this.state = gameplayDurationState;
            }
        }


        [SerializeField, ShowOnly] public GameState gameState;

        [SerializeField, ShowOnly] int currentStartCountdownDuration = 0;

        [SerializeField, ShowOnly] public float currentGameplayDuration;

        [SerializeField, ShowOnly] public float missionGoalProgress;

        PauseWindow pauseWindow = null;

        [SerializeField, ShowOnly] protected PlayerControllerTopdown playerControllerTopdown;
        [SerializeField, ShowOnly] protected HealthComponent playerHealthComponent;

        [SerializeField] protected AnimatorOverrideController playerAnimatorOverride;

        [field: SerializeField] public static bool MinigameManagerExist { get; private set; }
        bool isTimerInitialized = false;


        protected virtual void Awake()
        {
            InputMaster.RegisterToKeyPress(InputMaster.InputType.Menu, OnPauseHandler);

            Broadcast.Register<AddScore>(AddScoreHandler);
            BroadcastState.Register<ScoreState>(ScoreStateChangeHandler);
            BroadcastState.Register<MinigameState>(StateChangeHandler);

            BroadcastState.Register<SceneManager.OnLoadingSteteChange>(SceneLoadingStateChangeHandler);

            MinigameManagerExist = true;
        }


        protected virtual void Start()
        {
            Time.timeScale = 1;
            Debug.Log($"*** Start: {GetType().Name}");

            if (MainConfig_ == null) MainConfig_ = SetupMinigameConfig(new MainDataTypes.AllConfigs.Config(new("", new())), "minigame_1", "minigame_win_1");

            var player = RuntimeSet.FindFirst(RuntimeSet.Group.Player, RuntimeSet.Channel.A);

            if (player == null)
            {
                $"Could not find player!".Error();
                return;
            }

            playerControllerTopdown = player.GetComponent<PlayerControllerTopdown>();

            if (playerControllerTopdown == null)
            {
                $"Could not find component: {nameof(PlayerControllerTopdown)} on the player!".Error();
                return;
            }

            playerControllerTopdown.OnEndActionHandler.AddListener(OnPlayerActionEndHandler);

            playerHealthComponent = player.GetComponent<HealthComponent>();

            if (playerHealthComponent == null)
            {
                $"Could not find component: {nameof(HealthComponent)} on the player!".Error();
                return;
            }

            playerHealthComponent.OnDeath.AddListener(OnPlayerDeathHandler);

            BroadcastState.Invoke(new ScoreState(0, MainConfig_.missionGoal));

            gameState = GameState.None;
            BroadcastState.Invoke(new MinigameState(gameState, currentStartCountdownDuration));

            SetupPlayer();

            if (SceneManager.Instance.SceneLoadState_ == SceneManager.SceneLoadState.Idle) StartGame();

            currentGameplayDuration = MainConfig_.durationSeconds;
        }

        private void SetupPlayer()
        {
            Addressables.InstantiateAsync(MainConfig_.playerModel).Completed += e =>
            {
                if (e.Status == AsyncOperationStatus.Failed)
                {
                    $"Failure to load addressable of id: {MainConfig_.playerModel}".Error();
                    return;
                }

                if (playerControllerTopdown.TryGetComponent<ConfigurableCharacter>(out var configurableCharacter) == false)
                {
                    $"Failure to find component of name: {nameof(ConfigurableCharacter)}".Error();

                    return;
                }

                configurableCharacter.SetupSkin(e.Result);
                configurableCharacter.SetupEffects("EffectSpawnNormal", "EffectDeathNormal");

                if (playerAnimatorOverride != null)
                {
                    Debug.Log("Override Player Animator");
                    configurableCharacter.Animator.runtimeAnimatorController = playerAnimatorOverride;
                }


                Addressables.InstantiateAsync(MainConfig_.playerWeaponModel).Completed += e =>
                {
                    configurableCharacter.SetupHandTool(e.Result, false);
                };
            };

            playerHealthComponent.UpdateSettings(new HealthComponent.HealthSetting() { maxHealth = MainConfig_.playerHealth, currentHealth = MainConfig_.playerHealth });
        }

        private void OnPlayerDeathHandler()
        {
            ForceGameOver(false);
        }

        protected virtual void OnDestroy()
        {
            if (playerControllerTopdown)
            {
                playerControllerTopdown.OnEndActionHandler.RemoveListener(OnPlayerActionEndHandler);
            }
            if (playerHealthComponent)
            {
                playerHealthComponent.OnDeath.RemoveListener(OnPlayerDeathHandler);
            }

            InputMaster.UnregisterToKeyPress(InputMaster.InputType.Menu, OnPauseHandler);

            Broadcast.Unregister<AddScore>(AddScoreHandler);
            BroadcastState.Unregister<ScoreState>(ScoreStateChangeHandler);
            BroadcastState.Unregister<MinigameState>(StateChangeHandler);
            BroadcastState.Unregister<SceneManager.OnLoadingSteteChange>(SceneLoadingStateChangeHandler);

            if (pauseWindow)
            {
                pauseWindow.Close();
                Time.timeScale = 1.0f;
            }
        }

        private void SceneLoadingStateChangeHandler(SceneManager.OnLoadingSteteChange change)
        {

            //Completed loading into minigame scene
            if (change.state == SceneManager.SceneLoadState.SceneFadeInCompleted)
            {
                BroadcastState.Set(new ScoreState(0, MainConfig_.missionGoal));
                BroadcastState.Invoke(new MinigameState(GameState.None, currentStartCountdownDuration));
                StartGame();
            }

            //Initiating Transition back to minigames window
            if (change.state == SceneManager.SceneLoadState.TransitionInitiated)
            {
                BroadcastState.Invoke(new MinigameState(GameState.Quit, currentStartCountdownDuration));
            }
        }

        protected void ForceGameOver(bool success)
        {
            if (success) gameState = GameState.GameOverSuccess;
            else gameState = GameState.GameOverFailure;

            BroadcastState.Invoke(new MinigameState(gameState, currentStartCountdownDuration));
        }

        private void StartGame()
        {
            Debug.Log("*** AAA");
            if (gameState != GameState.None || MinigameDevelopmentManager.DisableGameplay) return;
            Debug.Log("*** BBB");

            StartCoroutine(StartGameRoutine());
        }

        IEnumerator StartGameRoutine()
        {
            Debug.Log("*** CCC");

            yield return new WaitForSeconds(3f);

            gameState = GameState.Initiate;

            BroadcastState.Invoke(new MinigameState(gameState, currentStartCountdownDuration));

            yield return new WaitForSeconds(.5f);

            gameState = GameState.Initiating;

            currentStartCountdownDuration = 5;

            BroadcastState.Invoke(new MinigameState(gameState, currentStartCountdownDuration));

            yield return new WaitForSeconds(1.25f);

            currentStartCountdownDuration = 4;

            BroadcastState.Invoke(new MinigameState(gameState, currentStartCountdownDuration));

            yield return new WaitForSeconds(1.25f);

            currentStartCountdownDuration = 3;

            BroadcastState.Invoke(new MinigameState(gameState, currentStartCountdownDuration));

            yield return new WaitForSeconds(1.25f);

            currentStartCountdownDuration = 2;

            BroadcastState.Invoke(new MinigameState(gameState, currentStartCountdownDuration));

            yield return new WaitForSeconds(1.25f);

            currentStartCountdownDuration = 1;

            BroadcastState.Invoke(new MinigameState(gameState, currentStartCountdownDuration));

            yield return new WaitForSeconds(1.25f);

            gameState = GameState.Playing;

            BroadcastState.Invoke(new MinigameState(gameState, currentStartCountdownDuration));

            currentStartCountdownDuration = 0;
        }

        private void AddScoreHandler(AddScore score)
        {
            BroadcastState.Invoke<ScoreState>(e => new ScoreState(e.missionGoalProgress + (int)score.score, MainConfig_.missionGoal));
        }


        private void ScoreStateChangeHandler(ScoreState state)
        {
            missionGoalProgress = state.missionGoalProgress;

            if (missionGoalProgress < MainConfig_.missionGoal) return;

            if (gameState != GameState.Playing) return;

            gameState = GameState.GameOverSuccess;

            BroadcastState.Invoke(new MinigameState(gameState, currentStartCountdownDuration));

            ExecuteSuccessAction();
        }

        private  void ExecuteSuccessAction()
        {
            ActionUtil.ProcessAction(MainConfig_.winAction).Forget();

            if(ConfigUtil.TryGetActionPart(MainConfig_.winAction, e => e.callerAction.Outcomes, out var outcomes))
            {
                foreach (var outcome in outcomes)
                {
                    foreach(var possibleOutcome in outcome.PossibleOutcomes) 
                    {
                        
                        if(possibleOutcome.possibleOutcomeType == Candid.World.Models.ActionOutcomeOption.OptionInfoTag.UpdateEntity)
                        {
                            var asEntityUpdates = possibleOutcome as PossibleOutcomeTypes.UpdateEntity;

                            if(asEntityUpdates.Eid == "xp")
                            {
                                foreach (var entityUpdate in asEntityUpdates.Fields)
                                {
                                    if (entityUpdate.Value is EntityFieldEdit.Numeric asIncrementNumber)
                                    {
                                        if(asIncrementNumber.NumericType_ == EntityFieldEdit.Numeric.NumericType.Increment)
                                        {
                                            BroadcastState.Invoke(new MinigameState(gameState, currentStartCountdownDuration, asIncrementNumber.Value));
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Debug.LogError("Something went wrong displaying the reward");

        }

        private void OnPauseHandler()
        {
            if (pauseWindow)
            {
                pauseWindow.PlayCloseAnimation();
                Time.timeScale = 1.0f;
            }
            else
            {
                if (gameState != GameState.Playing) return;

                pauseWindow = WindowManager.Instance.OpenWindow<PauseWindow>();
                Time.timeScale = 0.0f;
            }
        }

        private void Update()
        {
            if (isTimerInitialized == false)
            {
                isTimerInitialized = true;

                Broadcast.Invoke(new GameplayTimeLeft(currentGameplayDuration, GameplayDurationState.Initiating));
            }

            if (gameState != GameState.Playing) return;


            float _newCurrentGameplayDuration = currentGameplayDuration - Time.deltaTime;
            if (_newCurrentGameplayDuration < 0) _newCurrentGameplayDuration = 0;

            currentGameplayDuration = _newCurrentGameplayDuration;

            if (currentGameplayDuration != 0)
            {
                Broadcast.Invoke(new GameplayTimeLeft(currentGameplayDuration, GameplayDurationState.Depleting));
            }
            else
            {
                gameState = GameState.GameOverFailure;

                BroadcastState.Invoke(new MinigameState(gameState, currentStartCountdownDuration));
            }
        }
        GameState prevGameState;
        private void StateChangeHandler(MinigameState state)
        {
            if (state.gameState == GameState.Initiating)
            {
                Debug.Log("*** Initiating: "+ state.countDown);

                OnInitiatingHandler(state.countDown);
            }
            else if(state.gameState == GameState.Playing)
            {
                Debug.Log("*** Playing!");

                OnPlayingHandler();
            }
            else if(state.gameState == GameState.GameOverSuccess)
            {
                if (prevGameState == GameState.Playing) OnGameOverWonHandler();
            }  
            else if (state.gameState == GameState.GameOverFailure)
            {
                if (prevGameState == GameState.Playing) OnGameOverLostHandler();
            }

            prevGameState = state.gameState;
        }

        protected abstract void OnInitiatingHandler(int countDown);

        protected abstract void OnPlayingHandler();

        protected abstract void OnGameOverWonHandler();

        protected abstract void OnGameOverLostHandler();

        protected abstract void OnPlayerActionEndHandler();
    }
}