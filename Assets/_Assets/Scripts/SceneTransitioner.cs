using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitioner : Singleton<SceneTransitioner>
{
    public bool LevelFinished { get; private set; } = false;

    [SerializeField] private Animator anim;
    [SerializeField] private Animator restartTextAnim;

    private bool restartActive;

    public const int MAIN_MENU_INDEX = 1;
    public const int FIRST_LEVEL_INDEX = 2;
    public const int CREDITS_SCENE_INDEX = 14;

    public const float FADE_ANIM_DURATION = 0.25f;

    public static bool IsFullGame { get; set; }

    public static int CurrBuildIndex = 0;

    public static bool GotNewBestTime { get; private set; }

    public static bool IsFading { get; private set; } = false;

    private void Start()
    {
        ToMainMenu();
    }

    public void OnRestartFadeoutAnimFinished()
    {
        restartActive = false;
    }

    private void Update()
    {
        if (InputHandler.Instance.Restart.Down)
        {
            if (CurrBuildIndex > MAIN_MENU_INDEX && CurrBuildIndex < CREDITS_SCENE_INDEX)
            {
                if (!restartActive)
                {
                    restartActive = true;

                    restartTextAnim.ResetTrigger("ToNotVisible");
                    restartTextAnim.SetTrigger("ToVisible");
                }
                else
                {
                    restartTextAnim.ResetTrigger("ToVisible");
                    restartTextAnim.SetTrigger("ToNotVisible");

                    restartActive = false;
                    Restart();
                }
            }
        }
    }

    public void OnLevelFinished()
    {
        LevelFinished = true;
        Timer.Instance.PauseTimer();
        bool isNewBestTime = SaveData.Instance.OnLevelCompleted(CurrBuildIndex - FIRST_LEVEL_INDEX, Timer.Instance.CurrTime);

        if (IsFullGame)
        {
            int nextIndex = CurrBuildIndex + 1;
            if (nextIndex == CREDITS_SCENE_INDEX)
            {
                //Full game just finished! 
                GotNewBestTime = SaveData.Instance.OnFullGameCompleted(Timer.Instance.TotalTime);
                LeaderboardCallHandler.Instance.UpdateScore((int)(Timer.Instance.TotalTime * 1000));

                AchievementHandler.Instance.TryUnlockAchievement(AchievementHandler.AchievementIDEnum.FINISH);
                if (Timer.Instance.TotalTime < 60 * 10)
                    AchievementHandler.Instance.TryUnlockAchievement(AchievementHandler.AchievementIDEnum.FINISH_10M);
                if (Timer.Instance.TotalTime < 60 * 5)
                    AchievementHandler.Instance.TryUnlockAchievement(AchievementHandler.AchievementIDEnum.FINISH_5M);
                if (Timer.Instance.TotalTime < 60 * 2)
                    AchievementHandler.Instance.TryUnlockAchievement(AchievementHandler.AchievementIDEnum.FINISH_2M);
            }

            LoadSceneWithIndex(nextIndex);
        }
        else
        {
            StageClearCanvas.Instance.OpenPopup(SaveData.CurrSaveData.LevelsList[CurrBuildIndex - FIRST_LEVEL_INDEX], isNewBestTime);
            PlayerController.Instance.OnLevelEnd();
        }
    }

    public void ToMainMenu()
    {
        LoadSceneWithIndex(MAIN_MENU_INDEX);
    }

    public void LoadSceneWithIndex(int _index)
    {
        Timer.Instance.PauseTimer();
        StartCoroutine(LoadSceneRoutine(_index));
    }

    private IEnumerator LoadSceneRoutine(int _index)
    {
        if (IsFading)
        {
            Debug.LogWarning("Already fading!");
            yield break;
        }

        IsFading = true;

        anim.ResetTrigger("ToClear");
        anim.SetTrigger("ToBlack");

        yield return new WaitForSeconds(FADE_ANIM_DURATION);

        LevelFinished = false;
        SceneManager.LoadScene(_index);
    }

    public void OnDeath()
    {
        if (LevelFinished)
            return;

        AchievementHandler.Instance.TryUnlockAchievement(AchievementHandler.AchievementIDEnum.DIE);

        LoadSceneWithIndex(CurrBuildIndex);
    }

    public void OnSceneFinishedLoading()
    {
        CurrBuildIndex = SceneManager.GetActiveScene().buildIndex;

        anim.ResetTrigger("ToBlack");
        anim.SetTrigger("ToClear");

        switch (CurrBuildIndex)
        {
            case 0:
                {
                    Debug.LogError("OnFinishedLoading should not be called in boot");
                    break;
                }
            case MAIN_MENU_INDEX:
                {
                    Timer.Instance.SetTimerVisualsStatus(false, false);
                    break;
                }
            case FIRST_LEVEL_INDEX:
                {
                    StartCoroutine(Timer.Instance.RestartTimer());
                    break;
                }
            case CREDITS_SCENE_INDEX:
                {
                    Timer.Instance.SetTimerVisualsStatus(false, false);
                    break;
                }
            default:
                {
                    StartCoroutine(Timer.Instance.ResumeTimer());
                    break;
                }
        }

        StartCoroutine(AfterFadeInAnimationDone());
    }

    private IEnumerator AfterFadeInAnimationDone()
    {
        if (!IsFading)
            yield break;

        yield return new WaitForSeconds(FADE_ANIM_DURATION);

        IsFading = false;
    }

    public void Restart()
    {
        if (IsFullGame)
            LoadSceneWithIndex(FIRST_LEVEL_INDEX);
        else
            LoadSceneWithIndex(CurrBuildIndex);
    }
}
