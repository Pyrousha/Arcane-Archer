using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowLightIndicator : Singleton<BowLightIndicator>
{
    [SerializeField] private Material unlit;
    [SerializeField] private Material canBoom;
    [SerializeField] private Material inRange;

    [SerializeField] private MeshRenderer gem1;
    [SerializeField] private MeshRenderer gem2;

    private ColorStateEnum currState = ColorStateEnum.Unlit;

    public enum ColorStateEnum
    {
        Unlit,
        CanBoom,
        InRange
    }

    public void SetColor(ColorStateEnum _state)
    {
        if (_state == currState)
            return;

        currState = _state;

        Material mat = unlit;

        switch (_state)
        {
            case ColorStateEnum.Unlit:
                mat = unlit;
                break;
            case ColorStateEnum.CanBoom:
                mat = canBoom;
                break;
            case ColorStateEnum.InRange:
                mat = inRange;
                break;
        }

        gem1.material = mat;
        gem2.material = mat;
    }
}
