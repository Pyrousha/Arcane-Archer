using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    private static float bestTime = float.MaxValue;

    private static bool unlocked = false;

    [SerializeField] private GameObject unlockText;
    [SerializeField] private GameObject newBestTimeText;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private bool isMainMenu;
    [SerializeField] private bool isFirstLevel;
    [SerializeField] private bool isCreditsScene;

    private static float totalTime = 0;
    private float currTime;

    private bool isPaused = true;

    private float startingTime;

    private void Start()
    {
        if (isMainMenu)
        {
            //Load stuff
            bestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue);
            if (bestTime < float.MaxValue)
                unlocked = true;

            PlayerController.turnSpeedX = PlayerPrefs.GetFloat("Sens", PlayerController.turnSpeedX);
        }

        if (isFirstLevel)
            totalTime = 0;

        UpdateUI();

        if (isCreditsScene)
        {
            if (!unlocked)
            {
                bestTime = totalTime;
                unlocked = true;
                unlockText.SetActive(true);
            }
            else
            {
                if (totalTime < bestTime)
                {
                    bestTime = totalTime;
                    newBestTimeText.SetActive(true);
                }
            }
            PlayerPrefs.SetFloat("BestTime", bestTime);
            return;
        }

        text.gameObject.SetActive(unlocked);
        if (!isMainMenu)
            StartCoroutine(ResumeTimer());
    }

    public void PauseTimer()
    {
        totalTime += currTime;
        isPaused = true;
    }

    public IEnumerator ResumeTimer()
    {
        yield return new WaitForSeconds(0.5f);

        isPaused = false;
        startingTime = Time.time;
        currTime = 0;
    }

    void Update()
    {
        if (isPaused)
            return;

        currTime = Time.time - startingTime;

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (text == null)
            return;

        currTime = Mathf.Max(0, currTime);
        float secsNum = totalTime + currTime;

        if (isMainMenu)
            secsNum = bestTime;

        int mins = Mathf.FloorToInt(secsNum / 60);
        string minsStr = mins.ToString();
        if (minsStr.Length < 2)
            minsStr = "0" + minsStr;

        int secs = Mathf.FloorToInt(secsNum - mins * 60);
        string secsStr = secs.ToString();
        if (secsStr.Length < 2)
            secsStr = "0" + secsStr;

        int ms = Mathf.FloorToInt((secsNum % 1) * 60);
        string msStr = ms.ToString();
        if (msStr.Length < 2)
            msStr = "0" + msStr;

        text.text = minsStr + ":" + secsStr + ":" + msStr;
        if (isMainMenu)
            text.text = "Best Time: " + text.text;
    }
}
