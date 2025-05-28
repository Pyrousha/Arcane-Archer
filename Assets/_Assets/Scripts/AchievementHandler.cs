using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AchievementHandler : Singleton<AchievementHandler>
{
    public class AchievementStruct
    {
        public string APIName;
        public bool isUnlocked;
    }

    public enum AchievementIDEnum
    {
        DIE = 0,
        FINISH = 1,
        FINISH_10M = 2,
        FINISH_5M = 3,
        FINISH_2M = 4
    }

    private Dictionary<AchievementIDEnum, AchievementStruct> achievementsDict;

    bool initialized = false;

    private void Awake()
    {
        achievementsDict = new Dictionary<AchievementIDEnum, AchievementStruct>();

        for (int i = 0; i < Enum.GetNames(typeof(AchievementIDEnum)).Length; i++)
        {
            AchievementIDEnum currAcID = (AchievementIDEnum)i;
            achievementsDict.Add(currAcID, new AchievementStruct
            {
                APIName = $"{i}_{currAcID}",
                isUnlocked = false
            });
        }
    }

    private void Start()
    {
        TryInit();
    }

    private void TryInit()
    {
        if (initialized)
            return;

        if (SteamUserStats.RequestCurrentStats())
            initialized = true;
    }

    public void TryUnlockAchievement(AchievementIDEnum _acToUnlock)
    {
        try
        {
            if (!initialized)
            {
                Debug.LogError("Need to initialize first!");
                TryInit();

                if (!initialized)
                    return;
            }

            AchievementStruct currAc = achievementsDict[_acToUnlock];
            if (currAc.isUnlocked)
            {
                Debug.Log("AC: " + _acToUnlock.ToString() + " is already unlocked.");
                return;
            }

            currAc.isUnlocked = true;

            Debug.Log("Unlocked AC: " + _acToUnlock.ToString() + "!");

            SteamUserStats.SetAchievement(currAc.APIName);
            SteamUserStats.StoreStats();
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.ToString());
        }
    }
}
