namespace PlethoraV2.UI
{
    using Boom.UI;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using System;
    using PlethoraV2.Minigames;
    using UnityEngine.AddressableAssets;
    using Cysharp.Threading.Tasks;
    using UnityEngine.EventSystems;

    public partial class MinigameWidget : Window, IPointerEnterHandler, IPointerExitHandler
    {
        public class WindowData
        {
            public int minigameIndex;
            public BaseMinigameManager.MainConfig minigameConfig;
            public Action<BaseMinigameManager.MainConfig> onSceneSelectedCallback;

            public WindowData(int minigameIndex, BaseMinigameManager.MainConfig minigameConfig, Action<BaseMinigameManager.MainConfig> onSceneSelectedCallback)
            {
                this.minigameIndex = minigameIndex;
                this.minigameConfig = minigameConfig;
                this.onSceneSelectedCallback = onSceneSelectedCallback;
            }
        }

        [SerializeField] GameObject defaultImagel;
        [SerializeField] Image minigameImage;
        //[SerializeField] GameObject selectionFeedback;
        //[SerializeField] TextMeshProUGUI minigameindexText;
        [SerializeField] TextMeshProUGUI minigameNameText;
        [SerializeField] Button button;
        [SerializeField] Image footerImage;
        [SerializeField] Sprite footerEnableSprite;
        [SerializeField] TransformTweening pointerEnterAnim;

        private WindowData windowData;

        protected override void Awake()
        {
            base.Awake();
            button.onClick.AddListener(OnClickHandler);
            button.interactable = false;
        }


        private void OnDestroy()
        {
            button.onClick.RemoveListener(OnClickHandler);
        }

        public override bool RequireUnlockCursor()
        {
            return false;
        }

        public async override void Setup(object data)
        {
            if (data is not WindowData _windowData) return;
            windowData = _windowData;

            defaultImagel.SetActive(false);
            minigameImage.gameObject.SetActive(true);
            //selectionFeedback.SetActive(false);
            //minigameindexText.text = (windowData.minigameIndex+1).ToString();

            if (windowData.minigameConfig == null)
            {
                minigameNameText.text = "Comming Soon!";

                return;
            }

            var missionLogoSprite = await Addressables.LoadAssetAsync<Sprite>(windowData.minigameConfig.coverImage);

            if (missionLogoSprite)
            {
                minigameImage.sprite = missionLogoSprite;
            }
            else
            {
                minigameImage.gameObject.SetActive(false);
                defaultImagel.SetActive(true);
            }

            minigameNameText.text = windowData.minigameConfig.name;

            button.interactable = true;
        }

        private void OnClickHandler()
        {
            if (BaseMinigameManager.MainConfig_ != null) return;

            windowData.onSceneSelectedCallback.Invoke(windowData.minigameConfig);
            //selectionFeedback.SetActive(true);
            footerImage.sprite = footerEnableSprite;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (BaseMinigameManager.MainConfig_ != null) return;

            pointerEnterAnim.PlayFoward();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (BaseMinigameManager.MainConfig_ != null) return;

            pointerEnterAnim.PlayBackward();
        }
    }
}