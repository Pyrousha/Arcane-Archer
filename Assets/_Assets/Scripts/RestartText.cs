using UnityEngine;

public class RestartText : MonoBehaviour
{
    [SerializeField] private SceneTransitioner sceneTransitioner;

    public void OnFadeoutDone()
    {
        sceneTransitioner.OnRestartFadeoutAnimFinished();
    }
}
