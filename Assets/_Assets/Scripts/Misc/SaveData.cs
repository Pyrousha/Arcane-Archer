using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SaveData : Singleton<SaveData>
{
    public static SerializedSaveData CurrSaveData;

    public const string SAVE_KEY = "SaveData";
    public const int NUM_TOTAL_LEVELS = 12;

    public void Start()
    {
        LoadSaveData();
    }

    public void LoadSaveData()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            try
            {
                string saveJson = PlayerPrefs.GetString(SAVE_KEY);

                Debug.Log("Loaded Save Data:\n" + saveJson);

                CurrSaveData = JsonUtility.FromJson<SerializedSaveData>(saveJson);
                CurrSaveData.AfterFirstLoad();
            }
            catch (Exception)
            {
                if (CurrSaveData == null)
                {
                    Debug.LogError("Error when loading save data, try again?");
                }
                else
                {
                    SerializedSaveData newSave = new SerializedSaveData();
                    if (newSave.Version != CurrSaveData.Version)
                    {
                        Debug.LogError("Incompatible old save data, deleting...");
                        CurrSaveData = newSave;
                        //Save();
                    }
                }
            }
        }
        else
        {
            //Load default data and save
            CurrSaveData = new SerializedSaveData();
            Save();
        }
    }

    public void Save()
    {
        string saveJson = JsonUtility.ToJson(CurrSaveData);
        PlayerPrefs.SetString(SAVE_KEY, saveJson);
        PlayerPrefs.Save();

        Debug.Log("Saved Save Data:\nLength: " + saveJson.Length + "\n" + saveJson);
    }

    #region Debug Functions
    public void ResetSaveData()
    {
        CurrSaveData = new SerializedSaveData();

        PlayerPrefs.DeleteAll();

        SceneTransitioner.Instance.ToMainMenu();
    }
    #endregion


    /// <returns> If a new record was set for this level (requires the level to already be completed). </returns>
    public bool OnLevelCompleted(int _levelIndex, float _secs)
    {
        if (_levelIndex + 1 < CurrSaveData.LevelsList.Count)
            CurrSaveData.LevelsList[_levelIndex + 1].Unlock();

        bool toReturn = CurrSaveData.LevelsList[_levelIndex].UpdateTime(_secs);

        Save();

        return toReturn;
    }

    /// <returns> If a new record was set (requires full game to already be completed). </returns>
    public bool OnFullGameCompleted(float _secs)
    {
        bool toReturn = false;

        if (CurrSaveData.BestFullTime > 0)
        {
            //Game has been finished before
            if (_secs < CurrSaveData.BestFullTime)
                toReturn = true;

            CurrSaveData.BestFullTime = Mathf.Min(CurrSaveData.BestFullTime, _secs);
        }
        else
            CurrSaveData.BestFullTime = _secs;

        Save();

        return toReturn;
    }
}

[Serializable]
public class SerializedSaveData
{
    public float Version = 0.125f;

    public float MusicVol = 0.5f;
    public float SfxVol = 0.5f;

    public float MouseSens = 64f / 255f;
    public float Fov = 90;
    public float BestFullTime = 0;

    public string reboundControls = null;

    public List<LevelStruct> LevelsList;

    public SerializedSaveData()
    {
        LevelsList = new List<LevelStruct>() { new LevelStruct(true) };
        AfterFirstLoad();
    }

    public void AfterFirstLoad()
    {
        //Populate List
        while (LevelsList.Count < SaveData.NUM_TOTAL_LEVELS)
            LevelsList.Add(new LevelStruct(false));
    }
}

[Serializable]
public class LevelStruct
{
    public float Seconds = -1;
    public bool Unlocked = false;

    public LevelStruct(bool unlocked)
    {
        Unlocked = unlocked;
    }

    /// <returns> If this is a new best time </returns>
    public bool UpdateTime(float seconds)
    {
        if (Seconds > 0)
        {
            bool toReturn = false;

            //Level has been played before
            if (seconds < Seconds)
                toReturn = true;

            Seconds = Mathf.Min(seconds, Seconds);
            return toReturn;
        }

        //First time playing level
        Seconds = seconds;
        return false;
    }

    /// <returns> If this level was not unlocked already </returns>
    public bool Unlock()
    {
        bool toReturn = true;
        if (Unlocked)
            toReturn = false;

        Unlocked = true;
        return toReturn;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SaveData))]
public class SaveData_Inspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SaveData data = target as SaveData;

        if (GUILayout.Button("Reset All Data"))
        {
            data.ResetSaveData();
        }
    }
}
#endif
