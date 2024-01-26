using System.Collections.Generic;
using UnityEngine;

public class LevelSelectCanvas : Singleton<LevelSelectCanvas>
{
    [SerializeField] private GameObject parent;
    [SerializeField] private List<LevelButton> levels;
    private bool isOpen = false;

    public void OpenPopup()
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

    public void ClosePopup()
    {
        if (!isOpen)
            return;

        isOpen = false;
        parent.SetActive(false);
    }
}
