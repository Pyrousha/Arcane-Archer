using UnityEngine;
using UnityEngine.UI;

public class MainMenuCanvas : MonoBehaviour
{
    [SerializeField] private Button levelSelectButton;

    private void Start()
    {
        levelSelectButton.interactable = SaveData.CurrSaveData.BestFullTime > 0;
    }

    public void OnPlayClicked()
    {
        SceneTransitioner.IsFullGame = true;
        //TODO: Reset Timer
        SceneTransitioner.Instance.LoadSceneWithIndex(SceneTransitioner.FIRST_LEVEL_INDEX);
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
