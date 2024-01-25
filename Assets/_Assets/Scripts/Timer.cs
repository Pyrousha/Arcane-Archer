using System.Collections;
using TMPro;
using UnityEngine;

public class Timer : Singleton<Timer>
{
    [SerializeField] private TextMeshProUGUI totalTime_Label;
    [SerializeField] private TextMeshProUGUI currTime_Label;

    private static float totalTime = 0;
    public float CurrTime { get; private set; }

    private bool isPaused = true;

    private float startingTime;

    private void Start()
    {
        //if (isMainMenu)
        //{
        //    //Load stuff
        //    bestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue);
        //    if (bestTime < float.MaxValue)
        //        unlocked = true;


        //}

        //if (isFirstLevel)
        //    totalTime = 0;

        //UpdateUI();

        //if (isCreditsScene)
        //{
        //    if (!unlocked)
        //    {
        //        bestTime = totalTime;
        //        unlocked = true;
        //        unlockText.SetActive(true);
        //    }
        //    else
        //    {
        //        if (totalTime < bestTime)
        //        {
        //            bestTime = totalTime;
        //            newBestTimeText.SetActive(true);
        //        }
        //    }
        //    PlayerPrefs.SetFloat("BestTime", bestTime);
        //    return;
        //}

        //if (!isMainMenu && !isCreditsScene && unlocked)
        //    currTime_Label.gameObject.SetActive(true);
        //else
        //    currTime_Label.gameObject.SetActive(false);

        //totalTime_Label.gameObject.SetActive(unlocked);
        //if (!isMainMenu)
        //    StartCoroutine(ResumeTimer());
    }

    public void PauseTimer()
    {
        totalTime += CurrTime;
        isPaused = true;
    }

    public IEnumerator ResumeTimer()
    {
        yield return new WaitForSeconds(SceneTransitionController.FADE_ANIM_DURATION);

        isPaused = false;
        startingTime = Time.time;
        CurrTime = 0;
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
        float secsNum = totalTime + CurrTime;

        totalTime_Label.text = TimeToString(secsNum);
        //if (isMainMenu)
        //    totalTime_Label.text = "Best Time: " + totalTime_Label.text;


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
        if (minsStr.Length < 2)
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
