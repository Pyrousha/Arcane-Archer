using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static RebindControlsMenu;

public class RebindButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI InputName_Label;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI CurrBinding_Label;
    [SerializeField] private Image CurrBinding_Image;
    [SerializeField] private RebindControlsMenu rebindControlsMenu;

    [field: SerializeField] public InputID ID { get; private set; }

    public void OnClicked()
    {
        rebindControlsMenu.StartRebinding(ID, button);
    }

    public void SetLabel(string _label)
    {
        CurrBinding_Label.text = _label;
    }
}
