using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static RebindControlsMenu;

public class TutorialText : Singleton<TutorialText>
{
    [SerializeField] private TextMeshProUGUI tutText;
    [SerializeField] private TextMeshProUGUI closeText;
    [SerializeField] private GameObject dialogueParent;
    [SerializeField] private Animator anim;
    [SerializeField] private TypewriterEffect typewriterEffect;
    [SerializeField] private AudioClip voiceAudioClip;

    const char prefix = '[';
    const char suffix = ']';

    int currLevelIndex;

    private bool shownText = false;
    private bool doneTypingText = false;
    private bool dismissedText = false;

    private void Start()
    {
        currLevelIndex = SceneTransitioner.CurrBuildIndex;
        if (currLevelIndex >= SceneTransitioner.CREDITS_SCENE_INDEX)
            currLevelIndex--;
        currLevelIndex -= SceneTransitioner.FIRST_LEVEL_INDEX;
        SetText();
    }

    private void Update()
    {
        if (doneTypingText && InputHandler.Instance.Interact.Down && !dismissedText)
        {
            dismissedText = true;
            anim.SetTrigger("FadeOut");
        }
    }

    public void SetText()
    {
        dialogueParent.SetActive(SaveData.CurrSaveData.ShowTutText);

        if (shownText || !SaveData.CurrSaveData.ShowTutText)
            return;

        shownText = true;

        tutText.text = "";
        string textToShow = GetTutorialTextForLevelIndex(currLevelIndex);

        int len = 0;

        List<int> coloredIndices = new List<int>();
        for (int i = 0; i < textToShow.Length; i++)
        {
            int endIndex = -1;
            if (textToShow[i] == prefix)
            {
                for (int j = i + 1; j < textToShow.Length; j++)
                {
                    if (textToShow[j] == suffix)
                    {
                        endIndex = j - 2;

                        textToShow = textToShow.Remove(i, 1);
                        textToShow = textToShow.Remove(j - 1, 1);

                        coloredIndices.Add(i - len);

                        string splice = textToShow.Substring(i, endIndex - i + 1);
                        len += (splice.Length - 1);

                        for (int a = i; a <= endIndex; a++)
                            coloredIndices.Add(a);

                        break;
                    }
                }

                i = endIndex;
            }
        }
        coloredIndices.Sort();
        for (int i = coloredIndices.Count - 1; i > 0; i--)
        {
            if (coloredIndices[i] == coloredIndices[i - 1])
                coloredIndices.RemoveAt(i);
        }

        coloredIndices.Reverse();
        typewriterEffect.ShowText(textToShow, tutText, voiceAudioClip, coloredIndices);
    }

    public void OnTextDoneTyping()
    {
        doneTypingText = true;
        closeText.text = $"[Press <color=#{typewriterEffect.ColoredTextColor.ToHexString()}>{RebindControlsMenu.Instance.GetNameOfBinding(InputID.INTERACT)}</color> to close]";
    }

    private string GetTutorialTextForLevelIndex(int _levelIndex)
    {
        if (!SaveData.CurrSaveData.ShowTutText)
            return "";

        switch (_levelIndex)
        {
            case 0:
                return $"Welcome Archer!\nUse {prefix + RebindControlsMenu.Instance.GetNameOfBinding(InputID.FORWARD) + suffix}{prefix + RebindControlsMenu.Instance.GetNameOfBinding(InputID.LEFT) + suffix}" +
                       $"{prefix + RebindControlsMenu.Instance.GetNameOfBinding(InputID.BACK) + suffix}{prefix + RebindControlsMenu.Instance.GetNameOfBinding(InputID.RIGHT) + suffix} to move and {prefix + RebindControlsMenu.Instance.GetNameOfBinding(InputID.JUMP) + suffix} to jump.";

            case 1:
                return $"Move the camera with your mouse." +
                       $"\nAlso, {prefix + RebindControlsMenu.Instance.GetNameOfBinding(InputID.PAUSE) + suffix} will Open the pause menu. " +
                       $"\ntry changing controls or sensitivity!";

            case 2:
                return $"Hold and release {prefix + RebindControlsMenu.Instance.GetNameOfBinding(InputID.SHOOT) + suffix} to shoot," +
                       $"\nthen press {prefix + RebindControlsMenu.Instance.GetNameOfBinding(InputID.DETONATE) + suffix} to detonate!" +
                       $"\nLonger charge = more powerful explosion!";

            case 3:
                return $"It's ok if you miss!" +
                       $"\nPress {prefix + RebindControlsMenu.Instance.GetNameOfBinding(InputID.DETONATE) + suffix} to recall your arrow at any time";

            case 4:
                return $"Pro gamer tip:" +
                       $"\nJump right before detonation for extra height!";

            case 5:
                return $"Falling too slow?" +
                       $"\nHold {prefix + RebindControlsMenu.Instance.GetNameOfBinding(InputID.FALL) + suffix} to fall faster!" +
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

            case 12:
                return $"Level 13";

            case 13:
                return $"Level 14";

            case 14:
                return $"Level 15";

            case 15:
                return $"Level 16";

            case 16:
                return $"Level 17";

            case 17:
                return $"Level 18";

            default:
                return "ERROR_NO_TUTORIAL_TEXT";
        }
    }
}