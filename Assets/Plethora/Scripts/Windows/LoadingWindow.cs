namespace PlethoraV2.UI
{
    using Boom.Patterns.Broadcasts;
    using Boom.UI;
    using PlethoraV2.Addressables;
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class LoadingWindow : Window
    {
        [SerializeField] Slider slider;
        [SerializeField] TextMeshProUGUI text;
        bool init;

        public override Type[] GetConflictWindow()
        {
            return new Type[] { typeof(LoginWindow), typeof(GameplayWindow) };
        }

        protected override void Awake()
        {
            base.Awake();

            Broadcast.Register<SceneManager.OnLoadingScene>(OnLoadingHandler);

            Debug.Log("Loading window has been opened");
        }
        private void OnDestroy()
        {
            Broadcast.Unregister<SceneManager.OnLoadingScene>(OnLoadingHandler);
        }

        public override bool RequireUnlockCursor()
        {
            return true;
        }

        public override void Setup(object data)
        {
            if(slider) slider.value = 0;
            if (text) text.text = $"{0}";
        }

        private void OnLoadingHandler(SceneManager.OnLoadingScene data)
        {
            UpdateUI(data.progress);
        }

        private void UpdateUI(float progress)
        {
            progress = Mathf.Clamp01(progress);

            if (slider)
            {
                if (progress != slider.value || !init)
                {
                    init = true;
                    slider.value = progress;
                    if (text) text.text = $"{(int)(progress * 100)}";
                }
            }
            else
            {
                if (text)
                {
                    if (progress.ToString() != text.text || !init)
                    {
                        init = true;
                        text.text = $"{(int)(progress * 100)}";
                    }
                }
            }
        }
    }
}