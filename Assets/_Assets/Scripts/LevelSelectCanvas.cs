using System.Collections.Generic;
using UnityEngine;

public class LevelSelectCanvas : Submenu
{
    [SerializeField] private GameObject parent;
    [SerializeField] private List<LevelButton> levels;
    [SerializeField] private LevelButton_All allLevelsButton;
    [SerializeField] private LevelButton_All sumOfBest;
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

    private void Start()
    {
        if (parent.activeSelf)
            parent.SetActive(false);
    }

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
        if (isOpen)
            return;

        foreach (LevelButton levelButton in levels)
        {
            levelButton.SetData();
        }
        allLevelsButton.SetData();

        if (SaveData.CurrSaveData.BestFullTime > 0)
        {
            float sum = 0;
            for (int i = 0; i < levels.Count; i++)
            {
                sum += SaveData.CurrSaveData.LevelsList[i].Seconds;
            }

            sumOfBest.SetSecondsString(sum);
        }

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
