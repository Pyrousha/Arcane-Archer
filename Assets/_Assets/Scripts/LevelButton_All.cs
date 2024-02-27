public class LevelButton_All : LevelButton
{
    // Start is called before the first frame update
    public override void SetData()
    {
        levelLabel.text = "All";

        lockedObj.SetActive(false);

        float time = SaveData.CurrSaveData.BestFullTime;
        if (time > 0)
        {
            //Level finished before
            timeLabel.text = Timer.TimeToString(time);
        }
        else
        {
            //Level not played yet
            timeLabel.gameObject.SetActive(false);
        }
    }

    public override void OnClicked()
    {
        LevelSelectCanvas.Instance.ToLastSubmenu();
        StageClearCanvas.Instance.ToLastSubmenu();
        SceneTransitioner.IsFullGame = true;
        Timer.Instance.SetTimerVisualsStatus(true, true);
        SceneTransitioner.Instance.LoadSceneWithIndex(SceneTransitioner.FIRST_LEVEL_INDEX);
    }
}
