using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SensitivityText : Singleton<SensitivityText>
{
    [SerializeField] private TextMeshProUGUI text;

    private void Start()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        float val = PlayerController.turnSpeedX = Mathf.Round(PlayerController.turnSpeedX * 100f) / 100f;
        text.text = "Mouse Sens: " + val;
    }
}
