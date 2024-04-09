namespace PlethoraV2.Input
{
    using ItsJackAnton.Patterns.Broadcasts;
    using PlethoraV2.Utility;
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public static class InputMaster
    {
        [Serializable]
        public enum InputType : int
        {
            Any = 1,
            Jump = 2,
            InteractRight = 4,
            InteractLeft = 8,
            Crouch = 16,
            Sprint = 32,
            Inventory = 64,
            Pickup = 128,
            Menu = 256,
            Dance = 512,
        }

        private static readonly UnityBroadcast OnTrigger = new();
        private static readonly UnityBroadcast OnRelease = new();
        private static ulong inputButtonData;
        private static bool isDisabled;

        private static Vector2 move;
        private static Vector2 look;
        private static Vector2 mousePos;
        private static float scroll;

        public static Vector2 Movement
        {
            get { return move; }
            set
            {
                if (isDisabled) return;
                move = value;
            }
        }
        public static Vector2 Look
        {
            get { return look; }
            set
            {
                if (isDisabled) return;
                look = value;
            }
        }
        //public static Vector2 CamMousePos
        //{
        //    get { return mousePos.MousePosToCameraView(); }
        //    set
        //    {
        //        if (isDisabled) return;
        //        mousePos = value;
        //    }
        //}
        //public static Vector2 WorldMousePos
        //{
        //    get { return mousePos.MousePosToCameraView(); }
        //    set
        //    {
        //        if (isDisabled) return;
        //        mousePos = value;
        //    }
        //}
        public static float Scroll
        {
            get { return scroll; }
            set
            {
                if (isDisabled) return;
                scroll = value;
            }
        }

        public static bool IsMoving { get { return Movement != Vector2.zero; } }
        public static bool IsMovingHorizontally { get { return Mathf.Abs(Movement.x) > .01f; } }
        public static bool IsMovingVertically { get { return Mathf.Abs(Movement.y) > .01f; } }
        public static bool IsLook { get { return Look != Vector2.zero; } }

        public static ulong InputButtonData { get { return inputButtonData; } }
        public static void PressButton(InputType button, bool isPressed)
        {
            if (isDisabled) return;

            ulong _buttonValue = (ulong)button;

            if (isPressed)
            {
                //Debug.Log($"On Key Press {button}");
                inputButtonData = inputButtonData.AddElementTo(_buttonValue);
                OnTrigger.Invoke((uint)_buttonValue);
            }
            else
            {
                //Debug.Log($"On Key Release {button}");
                inputButtonData = inputButtonData.RemoveElementTo(_buttonValue);
                OnRelease.Invoke((uint)_buttonValue);
            }
        }

        #region Registration
        public static void RegisterToKeyPress(InputType button, UnityAction listener)
        {
            OnTrigger.AddListener(listener, (uint)button);
        }
        public static void UnregisterToKeyPress(InputType button, UnityAction listener)
        {
            OnTrigger.RemoveListener(listener, (uint)button);
        }
        //
        public static void RegisterToKeyRelease(InputType button, UnityAction listener)
        {
            OnRelease.AddListener(listener, (uint)button);
        }
        public static void UnregisterToKeyRelease(InputType button, UnityAction listener)
        {
            OnRelease.RemoveListener(listener, (uint)button);
        }
        //
        public static void RegisterToAnyKeyPress(UnityAction listener)
        {
            var enumNames = Enum.GetNames(typeof(InputType));
            foreach (var enumName in enumNames)
            {
                if(Enum.TryParse<InputType>(enumName, false, out InputType result))
                {
                    OnTrigger.AddListener(listener, (uint)result);
                }
            }
        }
        public static void UnregisterToAnyKeyPress(UnityAction listener)
        {
            var enumNames = Enum.GetNames(typeof(InputType));
            foreach (var enumName in enumNames)
            {
                if (Enum.TryParse<InputType>(enumName, false, out InputType result))
                {
                    OnTrigger.RemoveListener(listener, (uint)result);
                }
            }
        }
        #endregion

        public static bool IsButtonPressed(InputType button)
        {
            return inputButtonData.ContainElement((ulong)button);
        }

        public static void Enable()
        {
            isDisabled = false;
            Movement = Vector2.zero;
            inputButtonData = 0;
        }
        public static void Disable()
        {
            isDisabled = true;
        }

        public static void Reset()
        {
            isDisabled = true;
            Movement = Vector2.zero;
            inputButtonData = 0;
        }

        #region Mouse
        public static Quaternion GetMouseDirection2D(this Vector3 objectPosition)
        {
            return GetMouseDirection2D(objectPosition, Camera.main);
        }

        public static Quaternion GetMouseDirection2D(this Vector3 objectPosition, Camera cam)
        {
            Vector3 dir = Look.XY3() - cam.WorldToScreenPoint(objectPosition);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
            return Quaternion.AngleAxis(angle, Vector3.forward);
        }
        //
        public static Vector3 GetMousePosition2D()
        {
            return GetMousePosition2D(Camera.main);
        }

        public static Vector3 GetMousePosition2D(Camera cam)
        {
            Vector3 mouseScreenPos = Look.XY3();
            mouseScreenPos.z = 10;
            Vector3 mouseWorldPos = cam.ScreenToWorldPoint(mouseScreenPos);

            return mouseWorldPos;
        }


        /// <summary>
        /// Check for this only in update
        /// </summary>
        /// <returns></returns>
        public static bool IsMouseOverUI()
        {
            if (EventSystem.current == null) return false;
            return EventSystem.current.IsPointerOverGameObject();
        }

        #endregion
    }
}