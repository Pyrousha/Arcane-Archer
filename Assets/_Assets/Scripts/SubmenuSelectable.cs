using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SubmenuSelectable : MonoBehaviour, ISelectHandler
{
    [field: SerializeField] public Submenu parentSubmenu { get; private set; }
    private Selectable c_selectable;

    private void Awake()
    {
        c_selectable = GetComponent<Selectable>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        //parentSubmenu.SetLastSelected(c_selectable);
    }
}
