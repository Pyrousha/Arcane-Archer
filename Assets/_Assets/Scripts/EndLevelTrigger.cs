using UnityEngine;

public class EndLevelTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Level Finished!");

        SceneTransitionController.Instance.OnLevelFinished();
    }
}
