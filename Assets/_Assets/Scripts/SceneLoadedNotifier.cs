using UnityEngine;

public class SceneLoadedNotifier : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SceneTransitionController.Instance.OnSceneFinishedLoading();
    }
}
