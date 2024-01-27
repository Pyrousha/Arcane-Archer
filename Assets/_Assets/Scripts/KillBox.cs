using UnityEngine;

public class KillBox : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        SceneTransitioner.Instance.OnDeath();
    }
}
