namespace PlethoraV2.Addressables
{

    using UnityEngine;
    using UnityEngine.AddressableAssets;

    public class LoadAddressableSceneBehaviour : MonoBehaviour
    {
        [SerializeField] AssetReference scene;

        public void LoadScene()
        {
            SceneManager.Instance.StartSceneTransition(new SceneManager.TransitionArgAssetReference(scene));
        }
    }
}