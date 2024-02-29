using TMPro;
using UnityEngine;

public class SensitivityText : Singleton<SensitivityText>
{
    [SerializeField] private TextMeshProUGUI text;

    private void Start()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        //float val = PlayerController.MouseSens = Mathf.Round(PlayerController.MouseSens * 100f) / 100f;
        //text.text = "Mouse Sens: " + val;
    }
}
