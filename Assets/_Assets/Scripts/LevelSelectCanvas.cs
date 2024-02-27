using System.Collections.Generic;
using UnityEngine;

public class LevelSelectCanvas : Submenu
{
    [SerializeField] private GameObject parent;
    [SerializeField] private List<LevelButton> levels;
    private bool isOpen = false;


    #region Singleton
    private static LevelSelectCanvas instance = null;

    public static LevelSelectCanvas Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<LevelSelectCanvas>();
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

    public override void OnSubmenuSelected()
    {
        OpenPopup();
    }
    public override void OnSubmenuClosed()
    {
        ClosePopup();
    }

    private void OpenPopup()
    {
        foreach (LevelButton levelButton in levels)
        {
            levelButton.SetData();
        }

        if (isOpen)
            return;

        isOpen = true;
        parent.SetActive(true);
    }

    private void ClosePopup()
    {
        if (!isOpen)
            return;

        isOpen = false;
        parent.SetActive(false);
    }
}
