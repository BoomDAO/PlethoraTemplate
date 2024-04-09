namespace PlethoraV2.Values
{
    using UnityEngine;
    using System;

    [Serializable]
    public struct ToggableValue<T>
    {
        [SerializeField] private T value;

        [SerializeField] private bool enabled;

        public T Value
        {
            get { return value; }
            set
            {
                this.value = value;
            }
        }
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
            }
        }

        public ToggableValue(T value, bool enabled = false)
        {
            this.value = value;
            this.enabled = enabled;
        }
    }
}