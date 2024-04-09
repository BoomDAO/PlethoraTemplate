namespace PlethoraV2
{
    using System;
    using TMPro;
    using UnityEngine;

    [ExecuteInEditMode]
    public class TextSizer : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI text;
        public RectTransform rect;
        public float margin = 0f;

        void Update()
        {
            if (text == null || rect == null) return;
            float width = Application.isPlaying ? text.GetPreferredValues().x * .5f : text.GetPreferredValues().x;
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width + margin);
        }
    }
}