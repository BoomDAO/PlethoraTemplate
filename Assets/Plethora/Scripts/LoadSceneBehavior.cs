namespace PlethoraV2.Mono
{
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.SceneManagement;

    public class LoadSceneBehavior : MonoBehaviour
    {
        [SerializeField] string sceneName;

        public void LoadScene()
        {
            Debug.Log($"load scene of name: {sceneName}");
            SceneManager.LoadScene(sceneName);
        }
    }
}