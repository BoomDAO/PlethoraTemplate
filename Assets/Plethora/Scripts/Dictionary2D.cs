namespace PlethoraV2.Collection
{
    using System.Collections;
    using System.Collections.Generic;

    public class Dictionary2D<K1, K2, T> : IEnumerable<KeyValuePair<K1, Dictionary<K2, T>>>
    {
        public Dictionary<K1, Dictionary<K2, T>> dictionary2D;

        public Dictionary2D()
        {
            this.dictionary2D = new();
        }
        public Dictionary2D(Dictionary<K1, Dictionary<K2, T>> dictionary2D)
        {
            this.dictionary2D = dictionary2D;
        }

        private Dictionary<K2, T> GetSubDictionary(K1 k1)
        {
            if(dictionary2D.TryGetValue(k1, out var subDictionary) == false)
            {
                subDictionary = new Dictionary<K2, T>();
                dictionary2D.Add(k1, subDictionary);
            }

            return subDictionary;
        }

        public T GetValue(K1 k1, K2 k2)
        {
            Dictionary<K2, T> subDictionary = GetSubDictionary(k1);

            subDictionary.TryGetValue(k2, out T outValue);

            return outValue;
        }
        public void SetValue(K1 k1, K2 k2, T value)
        {
            Dictionary<K2, T> subDictionary = GetSubDictionary(k1);

            subDictionary[k2] = value;
        }

        public T this[K1 k1, K2 k2]
        {
            get
            {
                return GetValue(k1, k2);
            }
            set
            {
                SetValue(k1, k2, value);
            }
        }

        public bool TryGetValue(K1 k1, K2 k2, out T outValue)
        {
            Dictionary<K2, T> subDictionary = GetSubDictionary(k1);

            return subDictionary.TryGetValue(k2, out outValue);
        }
        public bool TryAdd(K1 k1, K2 k2, T value) 
        {
            Dictionary<K2, T> subDictionary = GetSubDictionary(k1);

            return subDictionary.TryAdd(k2, value);
        }
        public void Add(K1 k1, K2 k2, T value)
        {
            Dictionary<K2, T> subDictionary = GetSubDictionary(k1);

            subDictionary.Add(k2, value);
        }
        public bool Remove(K1 k1, K2 k2)
        {
            Dictionary<K2, T> subDictionary = GetSubDictionary(k1);

            return subDictionary.Remove(k2);
        }

        public IEnumerator<KeyValuePair<K1, Dictionary<K2, T>>> GetEnumerator()
        {
            return dictionary2D.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dictionary2D.GetEnumerator();
        }
    }
}