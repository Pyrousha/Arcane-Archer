using TMPro;
using UnityEngine;
using static RebindControlsMenu;

public class TutorialText : Singleton<TutorialText>
{
    [SerializeField] private TextMeshProUGUI tutText;
    [SerializeField] private GameObject dialogueParent;

    const string prefix = "<color=#FF8000>";
    const string suffix = "</color>";

    int currLevelIndex;

    private void Start()
    {
        currLevelIndex = SceneTransitioner.CurrBuildIndex - SceneTransitioner.FIRST_LEVEL_INDEX;
        SetText();
    }

    public void SetText()
    {
        dialogueParent.SetActive(SaveData.CurrSaveData.ShowTutText);
        tutText.text = GetTutorialTextForLevelIndex(currLevelIndex);
    }

    private string GetTutorialTextForLevelIndex(int _levelIndex)
    {
        if (!SaveData.CurrSaveData.ShowTutText)
            return "";

        switch (_levelIndex)
        {
            case 0:
                return $"Welcome Archer!\nUse {prefix + GetNameOfBinding(InputID.FORWARD) + suffix}{prefix + GetNameOfBinding(InputID.LEFT) + suffix}" +
                       $"{prefix + GetNameOfBinding(InputID.BACK) + suffix}{prefix + GetNameOfBinding(InputID.RIGHT) + suffix} to move and {prefix + GetNameOfBinding(InputID.JUMP) + suffix} to jump.";

            case 1:
                return $"Open the pause menu with {prefix}ESC{suffix}.";

            case 2:
                return $"Hold and release {prefix + GetNameOfBinding(InputID.SHOOT) + suffix} to shoot," +
                       $"\nthen press {prefix + GetNameOfBinding(InputID.KABOOM) + suffix} to detonate!" +
                       $"\nLonger charge = more powerful explosion!";

            case 3:
                return $"It's ok if you miss!" +
                       $"\nPress {prefix + GetNameOfBinding(InputID.KABOOM) + suffix} to recall your arrow at any time";

            case 4:
                return $"Pro gamer tip:" +
                       $"\nJump right before detonation for extra height!";

            case 5:
                return $"Falling too slow?" +
                       $"\nHold {prefix + GetNameOfBinding(InputID.FALL) + suffix} to fall faster!" +
                       $"\n(Don't worry about how it works)";

            case 6:
                return $"Scale walls by detonating arrows in them!";

            case 7:
                return $"You'll fall through green terrain,\n but arrows won't!" +
                       $"\nTime those detonations well!";

            case 8:
                return $"Time to test that right-clicking!";

            case 9:
                return $"Ready for some \"Wall jumps\"?";

            case 10:
                return $"Getting close to the end!";

            case 11:
                return $"Who needs floor anyways?";

            default:
                return "ERROR_NO_TUTORIAL_TEXT";
        }
    }
}