using System.Collections.Generic;
using UnityEngine;

public class RebindControlsMenu : Submenu
{
    public enum InputID
    {
        FORWARD,
        BACK,
        LEFT,
        RIGHT,
        JUMP,
        FALL,
        SHOOT,
        KABOOM
    }

    private bool isOpen = false;

    [SerializeField] private GameObject parent;
    [Space(10)]

    [SerializeField] private List<RebindButton> rebindButton;

    public void CloseMenu(bool _saveChanges)
    {
        if (!_saveChanges)
        {
            //TODO: Check to see if player wants to discard changes
        }

        SaveData.Instance.Save();

        ToLastSubmenu();
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
