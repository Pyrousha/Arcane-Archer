using UnityEngine;
using UnityEngine.UI;

public class Submenu : MonoBehaviour
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

    private void Update()
    {
        if (InputHandler.Instance.Pause.Down)
        {
            if (ActiveSubmenu == this)
            {
                ToLastSubmenu();
            }
        }
    }

    public void SetLastLayout(Submenu _menu)
    {
        LastSubmenu = _menu;
    }

    public void ToLastSubmenu()
    {
        if (LastSubmenu != null)
            LastSubmenu.Select();
    }

    public void SelectFromPast(Submenu _submenu)
    {
        LastSubmenu = _submenu;

        firstSelectable.Select();
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

    public virtual void OnSubmenuSelected() { }
}