using UnityEngine;

public class SettingsCanvas : Submenu
{
    [SerializeField] private GameObject parent;

    #region Singleton
    private static SettingsCanvas instance = null;

    public static SettingsCanvas Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<SettingsCanvas>();
            return instance;
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning("Duplicate instance of singleton found: " + gameObject.name + ", destroying.");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    #endregion
}
