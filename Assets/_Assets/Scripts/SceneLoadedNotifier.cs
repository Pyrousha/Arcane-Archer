using UnityEngine;

public class SceneLoadedNotifier : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SceneTransitioner.Instance.OnSceneFinishedLoading();
    }
}
