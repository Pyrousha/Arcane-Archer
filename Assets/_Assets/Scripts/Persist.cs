using UnityEngine;

public class Persist : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (transform.parent != null)
            transform.parent = null;
        DontDestroyOnLoad(gameObject);
    }
}
