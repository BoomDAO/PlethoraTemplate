namespace PlethoraV2.Gacha
{
    using System.Collections.Generic;
    using System;
    using PlethoraV2.Utility;
    using UnityEngine;
    public static class Gacha
    {
        public static GachaItem<T> GenerateItem<T>(this GachaRoll<T> gachaRoll)
        {
            float accumulatedValue = 0;

            gachaRoll.Variables.Iterate((k) =>
            {
                accumulatedValue += k.Weight;
            });


            float rand = UnityEngine.Random.Range(0, 1f) * accumulatedValue;

            for (int i = 0; i < gachaRoll.Variables.Count; ++i)
            {
                if (gachaRoll.Variables[i].Weight >= rand)
                {
                    return gachaRoll.Variables[i];
                }
                else
                {
                    rand -= gachaRoll.Variables[i].Weight;
                }
            }

            return null;
        }

        public static LinkedList<GachaItem<T>> GenerateItems<T>(this Gacha<T> gacha, Predicate<GachaItem<T>> match = null)
        {
            if (match == null) match = e => true;

            LinkedList<GachaItem<T>> rewards = new();

            gacha.rolls.Iterate((e) =>
            {
                float accumulatedValue = 0;

                e.Variables.Iterate((k) =>
                {
                    accumulatedValue += k.Weight;
                }, match);


                float rand = UnityEngine.Random.Range(0, 1f) * accumulatedValue;

                for (int i = 0; i < e.Variables.Count; ++i)
                {
                    if (match(e.Variables[i]))
                    {
                        if (e.Variables[i].Weight >= rand)
                        {
                            rewards.AddLast(e.Variables[i]);
                            break;
                        }
                        else
                        {
                            rand -= e.Variables[i].Weight;
                        }
                    }
                }
            });

            return rewards;
        }
    }

    [Serializable]
    public class Gacha<T>
    {
        public List<GachaRoll<T>> rolls;
        public Gacha()
        {
            this.rolls = new();
        }
        public Gacha(List<GachaRoll<T>> rolls)
        {
            this.rolls = rolls;
        }
    }
    [Serializable]
    public class GachaRoll<T>
    {
        public GachaRoll()
        {
            Variables = new();
        }
        public GachaRoll(List<GachaItem<T>> variables)
        {
            Variables = variables;
        }
        [field: SerializeField] public List<GachaItem<T>> Variables { get; private set; }
    }
    [Serializable]
    public class GachaItem<T>
    {
        public GachaItem(T value, float quantity, float weight)
        {
            Value = value;
            Quantity = quantity;
            Weight = weight;
        }

        [field: SerializeField] public T Value { get; private set; }
        [field: SerializeField] public float Quantity { get; private set; }
        [field: SerializeField] public float Weight { get; private set; }
    }
}