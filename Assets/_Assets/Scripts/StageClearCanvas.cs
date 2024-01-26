using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageClearCanvas : Singleton<StageClearCanvas>
{
    [SerializeField] private GameObject parent;
    [SerializeField] private TextMeshProUGUI timeLabel;
    [SerializeField] private TextMeshProUGUI bestTimeLabel;
    [SerializeField] private GameObject newBestPopup;
    [SerializeField] private Button nextLevelButton;
    private bool isOpen = false;

    public void OpenPopup(LevelStruct _levelStruct, bool _isNewBestTime)
    {
        if (isOpen)
            return;

        isOpen = true;
        timeLabel.text = "Time: " + Timer.TimeToString(Timer.Instance.CurrTime);

        newBestPopup.SetActive(_isNewBestTime);
        bestTimeLabel.gameObject.SetActive(!_isNewBestTime);
        if (!_isNewBestTime)
            bestTimeLabel.text = "Best: " + Timer.TimeToString(_levelStruct.Seconds);

        //If this is the last level, shouldn't be able to click next
        nextLevelButton.interactable = (SceneManager.GetActiveScene().buildIndex != SceneTransitionController.CREDITS_SCENE_INDEX - 1);
        parent.SetActive(true);
    }

    public void ClosePopup()
    {
        if (!isOpen)
            return;

        isOpen = false;

        parent.SetActive(false);
    }

    public void OnReplayClicked()
    {
        ClosePopup();
        SceneTransitionController.Instance.LoadSceneWithIndex(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnNextLevelClicked()
    {
        ClosePopup();
        SceneTransitionController.Instance.LoadSceneWithIndex(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OnLevelSelectClicked()
    {
        LevelSelectCanvas.Instance.OpenPopup();
    }

    public void OnMainMenuClicked()
    {
        ClosePopup();
        SceneTransitionController.Instance.ToMainMenu();
    }
}
