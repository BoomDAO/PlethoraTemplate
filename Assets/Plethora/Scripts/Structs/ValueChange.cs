namespace PlethoraV2.Structs
{
    public struct ValueChange<T>
    {
        public T prevValue;
        public T newValue;

        public ValueChange(T prevValue, T newValue)
        {
            this.prevValue = prevValue;
            this.newValue = newValue;
        }
    }
}