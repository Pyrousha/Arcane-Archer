using TMPro;
using UnityEngine;

public class LevelButton : MonoBehaviour
{
    private const int FIRST_LEVEL_BUILD_INDEX = 1;

    [SerializeField] private GameObject lockedObj;
    [SerializeField] private TextMeshProUGUI levelLabel;
    [SerializeField] private TextMeshProUGUI timeLabel;
    [field: SerializeField] public int Index { get; set; }
    private LevelStruct levelInfo;

    // Start is called before the first frame update
    void Start()
    {
        levelInfo = SaveData.CurrSaveData.LevelsList[Index];
        levelLabel.text = "Lv ";
        if (Index + 1 < 10)
            levelLabel.text += "0";
        levelLabel.text += (Index + 1);

        lockedObj.SetActive(!levelInfo.Unlocked);
        if (levelInfo.Seconds > 0)
        {
            //Level finished before
            timeLabel.text = Timer.TimeToString(levelInfo.Seconds);
        }
        else
        {
            //Level not played yet
            timeLabel.gameObject.SetActive(false);
        }
    }

    public void OnClicked()
    {
        SceneTransitionController.Instance.LoadSceneWithIndex(SceneTransitionController.FIRST_LEVEL_INDEX + Index);
    }
}
