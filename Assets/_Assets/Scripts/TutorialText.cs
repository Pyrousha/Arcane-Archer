using TMPro;
using UnityEngine;

public class TutorialText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tutText;

    private static readonly string[] tutorialTexts = {
        "Welcome Archer!\nUse WASD to move and space to jump.",
        "Open the pause menu with ESC.",
        "Hold and release left click to shoot,\nthen press right click to detonate!\nLonger charge = more powerful explosion!",
        "It's ok if you miss!\nPress right click to recall your arrow at any time",
        "Pro gamer tip:\nJump right before detonation for extra height!",
        "Falling too slow?\nHold shift to fall faster!\n(Don't worry about how it works)",
        "Scale walls by detonating arrows in them!",
        "You'll fall through green terrain, but arrows won't!\nTime those detonations well!",
        "Time to test that right-clicking!",
        "Ready for some \"Wall jumps\"?",
        "Getting close to the end!",
        "Who needs floor anyways?"
    };

    private void Start()
    {
        int currIndex = SceneTransitioner.CurrBuildIndex - SceneTransitioner.FIRST_LEVEL_INDEX;
        if (currIndex > -1 && currIndex < tutorialTexts.Length)
            tutText.text = tutorialTexts[currIndex];
    }
}