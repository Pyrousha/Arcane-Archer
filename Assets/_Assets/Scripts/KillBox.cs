using UnityEngine;

public class KillBox : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        SceneTransitionController.Instance.OnDeath();
    }
}
