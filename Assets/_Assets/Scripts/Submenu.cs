using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Submenu : MonoBehaviour
{
    public static Submenu ActiveSubmenu { get; private set; }

    public Submenu LastSubmenu { get; private set; }

    [SerializeField] protected Selectable firstSelectable;

    public Selectable LastSelectedObj { get; private set; }
    public void SetLastSelected(Selectable _obj)
    {
        LastSelectedObj = _obj;
    }

    private void Start()
    {
        LastSelectedObj = firstSelectable;
    }

    public void SetLastLayout(Submenu _menu)
    {
        LastSubmenu = _menu;
    }

    public void ToLastSubmenu()
    {
        OnSubmenuClosed();

        if (LastSubmenu == null)
        {
            ActiveSubmenu = null;
            return;
        }

        LastSubmenu.Select();
    }

    public void SelectFromPast(Submenu _submenu)
    {
        if (_submenu != null)
            _submenu.SetLastSelected(EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>());

        LastSubmenu = _submenu;

        firstSelectable.Select();
        SFXManager.Instance.Play(SFXManager.AudioTypeEnum.BUTTON_SELECT);
        //LastSelectedObj.Select();

        ActiveSubmenu = this;
        OnSubmenuSelected();
    }

    public void Select()
    {
        LastSelectedObj.Select();

        ActiveSubmenu = this;
        OnSubmenuSelected();
    }

    public abstract void OnSubmenuSelected();
    public abstract void OnSubmenuClosed();
}