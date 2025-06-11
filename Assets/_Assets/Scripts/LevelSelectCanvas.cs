using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private void OnValidate()
    {
        List<Selectable> selectables = new List<Selectable>();

        for (int i = 0; i < levels.Count; i++)
        {
            levels[i].Index = i;
            levels[i].gameObject.name = "Level_" + i;

            selectables.Add(levels[i].gameObject.GetComponent<Selectable>());
        }

        GetComponent<LinkSelectables>().SetSelectables(selectables);
    }

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

        if (SaveData.CurrSaveData.FinishedGame)
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

        LayoutRebuilder.ForceRebuildLayoutImmediate(parent.gameObject.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(parent.gameObject.GetComponent<RectTransform>());
    }

    private void ClosePopup()
    {
        if (!isOpen)
            return;

        isOpen = false;
        parent.SetActive(false);
    }
}
