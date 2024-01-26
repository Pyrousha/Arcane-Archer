using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tutText;

    private static readonly string[] tutorialTexts = {
        "Welcome Archer!\nUse WASD to move and space to jump.",
        "Open the pause menu with ESC",
        "Hold and release left click to shoot,\nthen press right click to detonate!\nLonger charge = more powerful explosion!",
        "It's ok if you miss!\nPress right click to recall your arrow at any time",
        "Pro gamer tip:\nJump right before detonation for extra height!",
        "Falling too slow?\nHold shift to fall faster!\n(Don't worry about how it works)",
        "Detonate arrows in the wall to scale even the tallest walls!",
        "You'll fall through green terrain, but arrows won't!\nTime those detonations well!",
        "Time to test that right-clicking!",
        "Ready for some \"Wall jumps\"?",
        "Getting close to the end!",
        "Who needs floor anyways?"
    };

    private void Start()
    {
        tutText.text = tutorialTexts[SceneManager.GetActiveScene().buildIndex - SceneTransitionController.FIRST_LEVEL_INDEX];
    }
}
