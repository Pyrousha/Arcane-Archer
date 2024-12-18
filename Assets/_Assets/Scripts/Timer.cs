using System.Collections;
using TMPro;
using UnityEngine;

public class Timer : Singleton<Timer>
{
    [SerializeField] private TextMeshProUGUI totalTime_Label;
    [SerializeField] private TextMeshProUGUI currTime_Label;

    public float TotalTime { get; private set; }
    public float CurrTime { get; private set; }

    private bool isPaused = true;

    private float startingTime;

    private bool total_enabled;
    private bool curr_enabled;

    private void Start()
    {
        currTime_Label.text = TimeToString(0);
    }

    public void SetTimerVisualsStatus(bool _totalTimeVisible, bool _currTimeVisible)
    {
        total_enabled = _totalTimeVisible;
        curr_enabled = _currTimeVisible;

        RefreshTimerState();
    }

    public void RefreshTimerState()
    {
        totalTime_Label.gameObject.SetActive(total_enabled && SaveData.CurrSaveData.ShowTimer);
        currTime_Label.gameObject.SetActive(curr_enabled && SaveData.CurrSaveData.ShowTimer);
    }

    public IEnumerator RestartTimer()
    {
        CurrTime = 0;
        TotalTime = 0;

        UpdateUI();

        yield return new WaitForSeconds(SceneTransitioner.FADE_ANIM_DURATION);

        startingTime = Time.time;

        isPaused = false;
    }

    public void PauseTimer()
    {
        if (isPaused)
        {
            Debug.LogWarning("Timer already paused");
            return;
        }

        TotalTime += CurrTime;
        isPaused = true;
    }

    public IEnumerator ResumeTimer()
    {
        if (!isPaused)
        {
            Debug.LogWarning("Timer already unpaused");
            yield break;
        }

        yield return new WaitForSeconds(SceneTransitioner.FADE_ANIM_DURATION);

        CurrTime = 0;
        startingTime = Time.time;

        isPaused = false;
    }

    void Update()
    {
        if (isPaused)
            return;

        CurrTime = Time.time - startingTime;

        UpdateUI();
    }

    private void UpdateUI()
    {
        //Overall time
        if (totalTime_Label == null)
            return;

        CurrTime = Mathf.Max(0, CurrTime);
        float secsNum = TotalTime + CurrTime;

        totalTime_Label.text = TimeToString(secsNum);


        //Current level time
        if (currTime_Label == null)
            return;

        currTime_Label.text = TimeToString(CurrTime);
    }

    public static string TimeToString(float _secs)
    {
        float secsNum = _secs;

        int mins = Mathf.FloorToInt(secsNum / 60);
        string minsStr = mins.ToString();
        if (minsStr.Length < 1)
            minsStr = "0" + minsStr;

        int secs = Mathf.FloorToInt(secsNum - mins * 60);
        string secsStr = secs.ToString();
        if (secsStr.Length < 2)
            secsStr = "0" + secsStr;

        int ms = Mathf.FloorToInt((secsNum % 1) * 1000);
        string msStr = ms.ToString();
        while (msStr.Length < 3)
            msStr = "0" + msStr;

        return minsStr + ":" + secsStr + ":" + msStr;
    }
}
