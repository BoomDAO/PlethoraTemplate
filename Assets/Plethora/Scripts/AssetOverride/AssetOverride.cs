namespace PlethoraV2
{
    using System.Collections.Generic;
    using Patterns;
    using PlethoraV2.Values;
    using UnityEngine;

    [CreateAssetMenu(fileName = "AssetOverride", menuName = "ScriptableObjects/AssetOverride", order = 1)]
    public class AssetOverride : SingletonScriptableObject<AssetOverride>
    {
        [field: SerializeField] public ToggableValue<string> PlayerModelOverride { get; private set; }

        [field: SerializeField] public ToggableValue<string> EnemyChaseMeleModel1 { get; private set; }
        [field: SerializeField] public ToggableValue<string> EnemyChaseMeleModel2 { get; private set; }
    }
}