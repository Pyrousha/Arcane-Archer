using UnityEngine;

public class MainMenuCanvas : MonoBehaviour
{
    public void OnPlayClicked()
    {
        SceneTransitionController.IsFullGame = true;
        //TODO: Reset Timer
        SceneTransitionController.Instance.LoadSceneWithIndex(SceneTransitionController.FIRST_LEVEL_INDEX);
    }

    public void OnLevelSelectClicked()
    {
        LevelSelectCanvas.Instance.OpenPopup();
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
