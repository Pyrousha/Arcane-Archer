using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionController : Singleton<SceneTransitionController>
{
    public bool LevelFinished { get; private set; } = false;

    [SerializeField] private Animator anim;

    public const int MAIN_MENU_INDEX = 1;
    public const int FIRST_LEVEL_INDEX = 2;
    public const int CREDITS_SCENE_INDEX = 14;

    public const float FADE_ANIM_DURATION = 0.25f;

    public static bool IsFullGame { get; set; }

    private void Start()
    {
        ToMainMenu();
    }

    public void OnLevelFinished()
    {
        int currBuildIndex = SceneManager.GetActiveScene().buildIndex;

        LevelFinished = true;
        Timer.Instance.PauseTimer();
        bool isNewBestTime = SaveData.Instance.OnLevelCompleted(currBuildIndex - FIRST_LEVEL_INDEX, Timer.Instance.CurrTime);

        if (IsFullGame)
            LoadSceneWithIndex(currBuildIndex + 1);
        else
        {
            StageClearCanvas.Instance.OpenPopup(SaveData.CurrSaveData.LevelsList[currBuildIndex - FIRST_LEVEL_INDEX], isNewBestTime);
            PlayerController.Instance.OnLevelEnd();
        }
    }

    public void ToMainMenu()
    {
        LoadSceneWithIndex(MAIN_MENU_INDEX);
    }

    public void LoadSceneWithIndex(int _index)
    {
        StartCoroutine(LoadSceneRoutine(_index));
    }
    private IEnumerator LoadSceneRoutine(int _index)
    {
        anim.ResetTrigger("ToClear");
        anim.SetTrigger("ToBlack");

        yield return new WaitForSeconds(FADE_ANIM_DURATION);

        LevelFinished = false;
        SceneManager.LoadScene(_index);
    }

    public void OnDeath()
    {
        Debug.Log("U DED LOL");
        if (LevelFinished)
            return;

        Timer.Instance.PauseTimer();
        LoadSceneWithIndex(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnSceneFinishedLoading()
    {
        anim.ResetTrigger("ToBlack");
        anim.SetTrigger("ToClear");
        StartCoroutine(Timer.Instance.ResumeTimer());
    }
}
