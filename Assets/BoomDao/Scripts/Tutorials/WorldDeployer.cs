namespace Boom.Tutorials
{
    using Boom.Utility;
    using Boom;
    using Cysharp.Threading.Tasks;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using Candid.World.Models;
    using Candid.WorldDeployer;
    using Candid.World;
    using EdjCase.ICP.Candid.Models;
    using System;

    public class WorldDeployer : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] TMP_Text logText;
        [SerializeField] Button createWorldButton;
        [SerializeField] Button candidUIButton;

        [SerializeField] BoomSettings boomSettings;

        [Header("Specify a WorldId where you want to copy and paste configs and actions from.")]
        [SerializeField] string optionalReferenceWorldId;

        [SerializeField, ShowOnly] bool deployingWorld;
        #endregion


        #region MONO
        private void Awake()
        {
            createWorldButton.onClick.RemoveAllListeners();

            createWorldButton.onClick.AddListener(ButtonClickHandler);

            candidUIButton.onClick.AddListener(OpenCandidUI);
        }

        private void Start()
        {
            logText.text = string.IsNullOrEmpty(boomSettings.WorldId) ? $"Your {boomSettings.name} has not yet been set up!" : $"You world id is: {boomSettings.WorldId}";
            Debug.Log($"You world id is: {boomSettings.WorldId}");

            createWorldButton.enabled = string.IsNullOrEmpty(boomSettings.WorldId);

            candidUIButton.enabled = string.IsNullOrEmpty(boomSettings.WorldId) == false;
        }


        private void OnDestroy()
        {
            createWorldButton.onClick.RemoveListener(ButtonClickHandler);
            candidUIButton.onClick.RemoveListener(OpenCandidUI);
        }
        #endregion


        #region ACTION

        private void OpenCandidUI()
        {
            Application.OpenURL($"https://5pati-hyaaa-aaaal-qb3yq-cai.raw.icp0.io/?id={boomSettings.WorldId}");
        }

        public void ButtonClickHandler()
        {
            CreateWorld().Forget();
        }

        public async UniTaskVoid CreateWorld()
        {

            if (UserUtil.IsLoggedIn(out var loginData) == false)
            {
                Debug.LogError("You must log in!");
                return;
            }

            //SECTION A: Set up arguments


            if (string.IsNullOrEmpty(boomSettings.WorldId) == false)
            {
                Debug.LogError("Boom's Settings are already setup");
                return;
            }

            if (deployingWorld)
            {
                return;
            }
            deployingWorld = true;


            WorldDeployerApiClient worldDeployer = new WorldDeployerApiClient(loginData.agent, Principal.FromText(boomSettings.DeploymentEnv_ == BoomSettings.DeploymentEnv.Production ? Env.CanisterIds.WORLD_DEPLOYER.PRODUCTION : Env.CanisterIds.WORLD_DEPLOYER.STAGING));

            logText.text = "Creaing a new world";
            var worldCanisterId = await worldDeployer.CreateWorldCanister("New World", "");
            logText.text = $"Setting up world";
            WorldApiClient world = new WorldApiClient(loginData.agent, Principal.FromText(worldCanisterId));

            //world

            var worldSetupResult = await world.CreateMinigameWinAction();


            if (string.IsNullOrEmpty(optionalReferenceWorldId) == false)
            {
                logText.text = $"Importing Actions from reference world of Id: {optionalReferenceWorldId}";

                var importActionsResult = await world.ImportAllActionsOfWorld(new WorldApiClient.ImportAllActionsOfWorldArg0(optionalReferenceWorldId));

                if(importActionsResult.Tag == Result4Tag.Err)
                {
                    Debug.LogError(importActionsResult.AsErr());
                }

                logText.text = $"Importing Configs from reference world of Id: {optionalReferenceWorldId}";

                var importConfigsResult = await world.ImportAllConfigsOfWorld(new WorldApiClient.ImportAllConfigsOfWorldArg0(optionalReferenceWorldId));

                if (importConfigsResult.Tag == Result4Tag.Err)
                {
                    Debug.LogError(importConfigsResult.AsErr());
                }
            }

            if (worldSetupResult.Tag == Result4Tag.Ok)
            {
                logText.text = $"You have created a world of Id: {worldCanisterId}";
                Debug.Log($"You have created a world of Id: {worldCanisterId}");
            }
            else
            {
                logText.text = $"Failure to setup world of Id: {worldCanisterId}";
                Debug.Log($"Failure to setup world of Id: {worldCanisterId}");
            }


            boomSettings.WorldId = worldCanisterId;
            //boomSettings.WorldName = worldName;
            boomSettings.WorldEnv_ = boomSettings.DeploymentEnv_ == BoomSettings.DeploymentEnv.Production ? BoomSettings.WorldEnv.Production : BoomSettings.WorldEnv.Staging;

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty( boomSettings );
#endif

            createWorldButton.enabled = string.IsNullOrEmpty(boomSettings.WorldId);

            candidUIButton.enabled = string.IsNullOrEmpty(boomSettings.WorldId) == false;

            deployingWorld = false;
        }
#endregion
    }
}