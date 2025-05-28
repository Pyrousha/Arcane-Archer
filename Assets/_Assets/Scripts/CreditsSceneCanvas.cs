using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreditsSceneCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI newTimeLabel;
    [SerializeField] private TextMeshProUGUI bestTimeLabel;
    [SerializeField] private GameObject newBestTimeNotification;
    [SerializeField] private Selectable toMenuButton;

    // Start is called before the first frame update
    void Start()
    {
        toMenuButton.Select();

        Cursor.lockState = CursorLockMode.None;

        newTimeLabel.text = Timer.TimeToString(Timer.Instance.TotalTime);

        if (SceneTransitioner.GotNewBestTime)
        {
            bestTimeLabel.transform.parent.gameObject.SetActive(false);
            newBestTimeNotification.SetActive(true);
        }
        else
        {
            if (Timer.Instance.TotalTime == SaveData.CurrSaveData.BestFullTime)
            {
                //This was the first clear of the game
                bestTimeLabel.transform.parent.gameObject.SetActive(false);
            }
            else
                bestTimeLabel.text = Timer.TimeToString(SaveData.CurrSaveData.BestFullTime);
        }
    }

    public void OnMainMenuClicked()
    {
        SceneTransitioner.Instance.ToMainMenu();
    }
}
