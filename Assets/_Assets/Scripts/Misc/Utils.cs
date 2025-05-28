using TMPro;
using UnityEngine.InputSystem;

public static class Utils
{
    /// <summary>
    /// Remaps a value in one range to another, keeping the same percentage between the given min and max
    /// </summary>
    /// <param name="_value"></param>
    /// <param name="_currentMin"></param>
    /// <param name="_currentMax"></param>
    /// <param name="_targetMin"></param>
    /// <param name="_targetMax"></param>
    /// <returns></returns>
    public static float Remap(float _value, float _currentMin, float _currentMax, float _targetMin, float _targetMax)
    {
        float currRange = _currentMax - _currentMin;
        float currAmountFromMin = _value - _currentMin;
        float percentFromMin = currAmountFromMin / currRange;

        float targRange = _targetMax - _targetMin;
        return _targetMin + percentFromMin * targRange;
    }

    public static string GetSpriteForBinding(InputBinding actionNeeded, TMP_SpriteAsset spriteAsset)
    {
        string stringButtonName = actionNeeded.effectivePath;
        stringButtonName = RenameInput(stringButtonName);


        foreach (TMP_SpriteCharacter sprAsset in spriteAsset.spriteCharacterTable)
        {
            if (sprAsset.name == stringButtonName)
                return $"<sprite=\"{spriteAsset.name}\" name=\"{stringButtonName}\">";
        }

        return stringButtonName.Replace("KB_", "");
    }

    private static string RenameInput(string stringButtonName)
    {
        stringButtonName = stringButtonName.Replace("Interact", string.Empty);
        stringButtonName = stringButtonName.Replace("<Keyboard>/", "KB_");
        stringButtonName = stringButtonName.Replace("<Mouse>/", "KB_");

        if (stringButtonName.Contains("KB_"))
        {
            stringButtonName = stringButtonName.Replace("leftShift", "shift");
            stringButtonName = stringButtonName.Replace("rightShift", "shift");
            stringButtonName = stringButtonName.Replace("leftCtrl", "ctrl");
            stringButtonName = stringButtonName.Replace("rightCtrl", "ctrl");
            stringButtonName = stringButtonName.Replace("leftAlt", "alt");
            stringButtonName = stringButtonName.Replace("rightAlt", "alt");
        }
        //alt, ctrl, shift

        //if (true)//XBOX BUTTONS)
        stringButtonName = stringButtonName.Replace("<Gamepad>/", "XBox_");
        //else
        //    stringButtonName = stringButtonName.Replace("<Gamepad>/", "PS5_");

        return stringButtonName;
    }
}
