using TMPro;
using UnityEngine;

public class LevelButton : MonoBehaviour
{
    [SerializeField] protected GameObject lockedObj;
    [SerializeField] protected TextMeshProUGUI levelLabel;
    [SerializeField] protected TextMeshProUGUI timeLabel;
    [field: SerializeField] public int Index { get; set; }
    private LevelStruct levelInfo;

    // Start is called before the first frame update
    public virtual void SetData()
    {
        levelInfo = SaveData.CurrSaveData.LevelsList[Index];
        levelLabel.text = "Lv ";
        if (Index + 1 < 10)
            levelLabel.text += "0";
        levelLabel.text += (Index + 1);

        lockedObj.SetActive(!levelInfo.Unlocked && !SaveData.CurrSaveData.FinishedGame);
        if (levelInfo.Seconds > 0)
        {
            //Level finished before
            timeLabel.gameObject.SetActive(true);
            timeLabel.text = Timer.TimeToString(levelInfo.Seconds);
        }
        else
        {
            //Level not played yet
            timeLabel.gameObject.SetActive(false);
        }
    }

    public void SetSecondsString(float _seconds, bool locked = false)
    {
        lockedObj.SetActive(locked);

        if (_seconds > 0)
        {
            //Level finished before
            timeLabel.gameObject.SetActive(true);
            timeLabel.text = Timer.TimeToString(_seconds);
        }
        else
        {
            //Level not played yet
            timeLabel.gameObject.SetActive(false);
        }
    }

    public virtual void OnClicked()
    {
        LevelSelectCanvas.Instance.ToLastSubmenu();
        StageClearCanvas.Instance.ToLastSubmenu();
        SceneTransitioner.IsFullGame = false;
        Timer.Instance.SetTimerVisualsStatus(false, true);

        int targIndex = SceneTransitioner.FIRST_LEVEL_INDEX + Index;
        if (targIndex >= SceneTransitioner.CREDITS_SCENE_INDEX)
            targIndex++;
        SceneTransitioner.Instance.LoadSceneWithIndex(targIndex);
    }
}
