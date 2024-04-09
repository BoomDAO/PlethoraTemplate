using PlethoraV2.Addressables;
using PlethoraV2.Minigames;
using UnityEngine;

public class ReloadAddressableSceneBehaviour : MonoBehaviour
{
    public void LoadScene()
    {
        SceneManager.Instance.StartSceneTransition(new SceneManager.TransitionArgKey(BaseMinigameManager.MainConfig_.sceneName));
    }
}
