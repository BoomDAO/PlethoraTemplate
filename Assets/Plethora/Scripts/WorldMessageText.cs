namespace PlethoraV2
{
    using Boom.UI;
    using Boom.Utility;
    using PlethoraV2.Patterns.RuntimeSet;
    using PlethoraV2.UI;
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class WorldMessageText : Window
    {
        [SerializeField] TextMeshProUGUI m_TextMeshPro;
        [SerializeField] float livetime = 5;
        public class WindowData { public string message;

            public WindowData(string message)
            {
                this.message = message;
            }
        }

        private void SetText(string text)
        {
            m_TextMeshPro.text = text;
        }
        public static void Instantiate(string text, Vector3 position)
        {
            //var worldCanvas = RuntimeSet.FindFirst(RuntimeSet.Group.WorldCanvas, RuntimeSet.Channel.A);

            //if (worldCanvas == null)
            //{
            //    $"Could not find a world canvas!".Error();
            //    return;
            //}

            //WorldMessageText prefab = Resources.Load<WorldMessageText>($"{nameof(WorldMessageText)}");

            //if (prefab == null)
            //{
            //    $"{nameof(WorldMessageText)} is not located at a resources folder".Error();
            //    return;
            //}

            //var textObject = Instantiate(prefab, worldCanvas);
            //textObject.SetText(text);
            //textObject.transform.position = position;
            //textObject.transform.rotation = Quaternion.LookRotation(-(Camera.main.transform.position - textObject.transform.position).normalized);

            //Destroy(textObject, textObject.livetime);

            WindowManager.Instance.AddWidgets<WorldMessageText>(new WindowData(text));
        }

        public override void Setup(object data)
        {       
            WindowData _data = (WindowData)data;
            if (_data == null)
            {
                Debug.Log($"Window of name {gameObject.name}, requires data, data cannot be null");
                return;
            }
            SetText(_data.message);

            Action destroyThis = () => { GameObject.Destroy(gameObject); };

            destroyThis.DelayAction(3, WindowManager.Instance.transform);
        }

        public override bool RequireUnlockCursor()
        {
            return false;
        }
    }
}