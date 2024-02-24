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
        levelSelectButton.interactable = SaveData.CurrSaveData.BestFullTime > 0;
        if (SaveData.CurrSaveData.BestFullTime <= 0)
        {
            //Game has not been finished before, disable level select button
            levelSelectButton.interactable = false;

            Navigation nav_play = new Navigation();
            nav_play.mode = Navigation.Mode.Explicit;
            nav_play.selectOnUp = quitButton;
            nav_play.selectOnDown = settingsButton;
            playButton.navigation = nav_play;

            Navigation nav_settings = new Navigation();
            nav_play.mode = Navigation.Mode.Explicit;
            nav_play.selectOnUp = playButton;
            nav_play.selectOnDown = quitButton;
            settingsButton.navigation = nav_settings;

            Navigation nav_quit = new Navigation();
            nav_play.mode = Navigation.Mode.Explicit;
            nav_play.selectOnUp = settingsButton;
            nav_play.selectOnDown = playButton;
            playButton.navigation = nav_quit;
        }

        playButton.Select();
    }

    public void OnPlayClicked()
    {
        SceneTransitioner.IsFullGame = true;
        //TODO: Reset Timer
        SceneTransitioner.Instance.LoadSceneWithIndex(SceneTransitioner.FIRST_LEVEL_INDEX);
    }

    public void OnLevelSelectClicked()
    {
        LevelSelectCanvas.Instance.SelectFromPast(this);
    }

    public void OnSettingsClicked()
    {
        //TODO: Settings Menu
    }

    public void OnQuitClicked()
    {
        Application.Quit();
    }
}
