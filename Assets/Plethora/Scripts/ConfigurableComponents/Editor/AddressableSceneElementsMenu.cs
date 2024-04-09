using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class AddressableSceneElementsMenu
{
    [MenuItem("Tools/Addressables/Swap to Real Assets")]
    private static void SwapToRealAssets()
    {
        var result = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (var obj in result)
        {
            var children = obj.GetComponentsInChildren<IAddressableSwap>();

            foreach (var child in children)
            {
                child.SwapToRealAddressable();
            }
        }
    }
    [MenuItem("Tools/Addressables/Swap to Plain Assets")]
    private static void SwapToPlainAssets()
    {
        var result = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (var obj in result)
        {
            var children = obj.GetComponentsInChildren<IAddressableSwap>();

            foreach (var child in children)
            {
                child.SwapToPlainAsset();
            }
        }
    }
}
