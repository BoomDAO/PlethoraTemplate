namespace PlethoraV2.Patterns
{
    using UnityEngine;

    //[CreateAssetMenu(fileName = "Singleton Scriptable Object", menuName = "Scriptable Objects/Singletons/Example")]
    public class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
    {
        private static T instance = null;
        public static T Instance
        {
            get
            {
                if (instance) return instance;
                else
                {
                    T[] assets = Resources.LoadAll<T>("");
                    if (assets == null || assets.Length < 1)
                    {
                        throw new System.Exception($"Could not find any singleton scriptable object of type {typeof(T)} in the resources folder");
                    }
                    else if (assets.Length > 1)
                    {
                        throw new System.Exception($"Multiple instances of singleton scriptable object of type {typeof(T)} in the resources folder");
                    }
                    else
                    {
                        //Debug.Log($"Success! {typeof(T)} Singleton scriptable object was in the resources folder");
                        instance = assets[0];
                        //instance.hideFlags = HideFlags.DontUnloadUnusedAsset;
                    }

                    return instance;
                }
            }
        }

        private void OnEnable()
        {
            if(instance != null)
            {
                if (instance != this) throw new System.Exception($"Multiple instances of singleton scriptable object of type {typeof(T)} in the resources folder");
                else Debug.Log($"Ensure singleton scriptable object of type {typeof(T)} in the resources folder");
            }
        }
    }
}