using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuCanvas : Submenu
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button levelSelectButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    [Space(10)]
    [SerializeField] private TextMeshProUGUI versionLabel1;
    [SerializeField] private TextMeshProUGUI versionLabel2;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;

        levelSelectButton.interactable = SaveData.CurrSaveData.BestFullTime > 0;
        if (SaveData.CurrSaveData.BestFullTime <= 0)
        {
            //Game has not been finished before, disable level select button
            levelSelectButton.interactable = false;

            Navigation nav_play = playButton.navigation;
            nav_play.selectOnDown = settingsButton;
            playButton.navigation = nav_play;

            Navigation nav_settings = settingsButton.navigation;
            nav_settings.selectOnUp = playButton;
            settingsButton.navigation = nav_settings;
        }

        playButton.Select();

        versionLabel1.text = SaveData.Instance.VersionNumText;
        versionLabel2.text = SaveData.Instance.VersionNumText;
    }

    public void OnPlayClicked()
    {
        SceneTransitioner.IsFullGame = true;
        Timer.Instance.SetTimerVisualsStatus(true, true);
        SceneTransitioner.Instance.LoadSceneWithIndex(SceneTransitioner.FIRST_LEVEL_INDEX);
    }

    public void OnLevelSelectClicked()
    {
        LevelSelectCanvas.Instance.SelectFromPast(this);
    }

    public void OnSettingsClicked()
    {
        SettingsCanvas.Instance.SelectFromPast(this);
    }

    public void OnQuitClicked()
    {
        Application.Quit();
    }

    public override void OnSubmenuSelected()
    { }

    public override void OnSubmenuClosed()
    { }
}
