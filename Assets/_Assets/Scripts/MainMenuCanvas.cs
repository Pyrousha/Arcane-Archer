using UnityEngine;
using UnityEngine.UI;

public class MainMenuCanvas : Submenu
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button levelSelectButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;

        levelSelectButton.interactable = SaveData.CurrSaveData.BestFullTime > 0;
        if (SaveData.CurrSaveData.BestFullTime <= 0)
        {
            //Game has not been finished before, disable level select button
            levelSelectButton.interactable = false;

            Navigation nav_play = playButton.navigation;
            nav_play.selectOnUp = quitButton;
            nav_play.selectOnDown = settingsButton;
            playButton.navigation = nav_play;

            Navigation nav_settings = settingsButton.navigation;
            nav_play.selectOnUp = playButton;
            nav_play.selectOnDown = quitButton;
            settingsButton.navigation = nav_settings;

            Navigation nav_quit = playButton.navigation;
            nav_play.selectOnUp = settingsButton;
            nav_play.selectOnDown = playButton;
            playButton.navigation = nav_quit;
        }

        playButton.Select();
    }

    public void OnPlayClicked()
    {
        SceneTransitioner.IsFullGame = true;
        if (SaveData.CurrSaveData.BestFullTime > 0) //Game has been finished before, show timer
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
}
