
namespace PlethoraV2.Addressables
{
    using Boom;
    using Boom.Patterns.Broadcasts;
    using Boom.Utility;
    using Boom.Values;
    using Cysharp.Threading.Tasks;
    using PlethoraV2.Utility;
    using System;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.Events;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.ResourceManagement.ResourceProviders;

    public class SceneManager : Singleton<SceneManager>
    {
        public const float BEFORE_UNLOADING_DELAY = 1f;

        public const float SCENE_FADE_DURATION = 1.1f;
        public const float BEFORE_FADEIN_DELAY = 1.5f;
        public abstract class TransitionBaseArg { }
        public class TransitionArgAssetReference : TransitionBaseArg
        {
            public AssetReference assetReference;

            public TransitionArgAssetReference(AssetReference assetReference)
            {
                this.assetReference = assetReference;
            }
        }
        public class TransitionArgKey : TransitionBaseArg
        {
            public string key;

            public TransitionArgKey(string key)
            {
                this.key = key;
            }
        }

        public enum SceneLoadState
        {
            Error,
            Idle,
            TransitionInitiated,
            SceneFadeOut,
            SceneFadeOutCompleted,
            PreUnload,
            Unloading,
            UnloadingCompleted,
            PreLoading,
            Loading,
            LoadCompleted,
            SceneFadeIn,
            SceneFadeInCompleted,
        }
        public struct OnLoadingScene : IBroadcast
        {

            public float progress;

            public OnLoadingScene(float progress)
            {
                this.progress = progress;
            }
        }

        public struct OnLoadingSteteChange : IBroadcastState
        {

            public SceneLoadState state;

            public OnLoadingSteteChange(SceneLoadState state)
            {
                this.state = state;
            }

            int IBroadcastState.MaxSavedStatesCount()
            {
                return 0;
            }
        }

        [SerializeField] private InitValue<SceneLoadState> sceneLoadState;
        public SceneLoadState SceneLoadState_
        {
            get { return sceneLoadState.Value; }
            private set
            {
                SceneLoadState newValue = value;
                if (newValue != sceneLoadState.Value || sceneLoadState.IsInit == false)
                {
                    bool wasInit = sceneLoadState.IsInit;

                    sceneLoadState.Value = newValue;

                    if(wasInit) BroadcastState.Invoke(new OnLoadingSteteChange(newValue));
                    else BroadcastState.Set(new OnLoadingSteteChange(newValue));
                }
            }
        }

        [SerializeField] private float transitionDelay = 1f;


        private InitValue<SceneInstance> prevLoadedScene;

        [field: SerializeField, ShowOnly] public bool AddressableValid { get; private set; }

        private readonly UnityEvent onAddressableInitializationCompleted = new();

        protected override void Awake_()
        {
            SceneLoadState_ = SceneLoadState.Idle;

            Addressables.InitializeAsync().Completed += e =>
            {
                AddressableValid = true;
                onAddressableInitializationCompleted.Invoke();
                onAddressableInitializationCompleted.RemoveAllListeners();
            };
        }

        public void AddListenerToAddressableInitializationCompleted(UnityAction action)
        {
            if(AddressableValid) action();
            else
            {
                onAddressableInitializationCompleted.AddListener(action);
            }
        }


        public void StartSceneTransition<T>(T scene) where T : TransitionBaseArg
        {
            if (SceneLoadState_ != SceneLoadState.Idle) return;

            StartSceneTransition_(scene).Forget();
        }

        private async UniTaskVoid StartSceneTransition_<T>(T scene) where T : TransitionBaseArg
        {
            Time.timeScale = 1;


            SceneLoadState_ = SceneLoadState.TransitionInitiated;

            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

            SceneLoadState_ = SceneLoadState.SceneFadeOut;

            await UniTask.Delay((int)(SCENE_FADE_DURATION * 1000), true);

            SceneLoadState_ = SceneLoadState.SceneFadeOutCompleted;

            await UniTask.Delay((int)(BEFORE_UNLOADING_DELAY * 1000), true);

            if (prevLoadedScene.IsInit)
            {

                SceneLoadState_ = SceneLoadState.PreUnload;

                Broadcast.Invoke(new OnLoadingScene(0));

                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

                SceneLoadState_ = SceneLoadState.Unloading;

                //UNLOAD SCENE
                var unloadingHandler = Addressables.UnloadSceneAsync(prevLoadedScene.Value);


                while (!unloadingHandler.IsDone)
                {
                    var unloadingHandlerPerc = unloadingHandler.GetDownloadStatus().Percent;

                    Broadcast.Invoke(new OnLoadingScene(MathUtil.MapUnclamped(unloadingHandlerPerc, 0, .99f, 0, .5f)));

                    await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
                }

                SceneLoadState_ = SceneLoadState.UnloadingCompleted;

                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            }
            else
            {
                float fakeProgress = 0;
                float fakeProgreesCap = 0.5f;
                while (fakeProgress < fakeProgreesCap)
                {

                    Broadcast.Invoke(new OnLoadingScene(MathUtil.MapUnclamped(fakeProgress, 0, .99f, .5f, 1)));

                    fakeProgress += Time.deltaTime;
                    fakeProgress = Mathf.Clamp(fakeProgress, 0, fakeProgreesCap);

                    await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
                }
            }

            SceneLoadState_ = SceneLoadState.PreLoading;

            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

            SceneLoadState_ = SceneLoadState.Loading;

            //LOAD SCENE
            AsyncOperationHandle<SceneInstance> loadingHandler = default;

            switch (scene)
            {
                case TransitionArgAssetReference a:
                    loadingHandler = Addressables.LoadSceneAsync(a.assetReference);
                    break;
                case TransitionArgKey a:
                    loadingHandler = Addressables.LoadSceneAsync(a.key);
                    break;
                default:

                    $"Generic type parameter T cannot be an abstract class.".Error(GetType().Name);

                    SceneLoadState_ = SceneLoadState.Error;

                    await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

                    SceneLoadState_ = SceneLoadState.Idle;

                    Broadcast.Invoke(new OnLoadingScene(0));
                    return;
            }

            var loadingHandlerPerc = 0f;
            while (!loadingHandler.IsDone)
            {
                loadingHandlerPerc = loadingHandler.GetDownloadStatus().Percent;

                Broadcast.Invoke(new OnLoadingScene(MathUtil.MapUnclamped(loadingHandlerPerc, 0, .99f, .5f, .99f)));

                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            }


            await UniTask.Delay(2 * 1000, true);

            await UniTask.WaitUntil(() => (ConfigurableMaterial.SwapProcessCount + ConfigurableParticleMaterial.SwapProcessCount) == 0);

            //We wait for responses only when the player wants to transition to another scene and has not yet logged in.
            if (UserUtil.IsLoggedIn() == false)
            {
                await UniTask.WaitUntil(() =>
                {
                    if (BroadcastState.TryRead<WaitingForResponse>(out var waitingForResponse))
                    {
                        return waitingForResponse.value == false;
                    }
                    return true;
                });
            }

            Broadcast.Invoke(new OnLoadingScene(1));

            SceneLoadState_ = SceneLoadState.LoadCompleted;

            await UniTask.Delay((int)(BEFORE_FADEIN_DELAY * 1000), true);

            SceneLoadState_ = SceneLoadState.SceneFadeIn;

            await UniTask.Delay((int)(SCENE_FADE_DURATION * 1000), true);

            SceneLoadState_ = SceneLoadState.SceneFadeInCompleted;

            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

            SceneLoadState_ = SceneLoadState.Idle;

            Broadcast.Invoke(new OnLoadingScene(0));
        }

        protected override void OnDestroy_()
        {

        }
    }
}