using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionController : Singleton<SceneTransitionController>
{
    public bool LevelFinished { get; private set; } = false;

    [SerializeField] private Animator anim;

    public const int MAIN_MENU_INDEX = 1;
    public const int FIRST_LEVEL_INDEX = 2;

    public const float FADE_ANIM_DURATION = 0.25f;

    private void Start()
    {
        ToMainMenu();
    }

    public void OnLevelFinished()
    {
        int currBuildIndex = SceneManager.GetActiveScene().buildIndex;

        LevelFinished = true;
        Timer.Instance.PauseTimer();
        SaveData.Instance.OnLevelCompleted(currBuildIndex - FIRST_LEVEL_INDEX, Timer.Instance.CurrTime);

        LoadSceneWithIndex(currBuildIndex + 1);
    }

    public void ToMainMenu()
    {
        LoadSceneWithIndex(MAIN_MENU_INDEX);
    }

    public void ToNextLevel()
    {
        LevelFinished = true;
        LoadSceneWithIndex(SceneManager.GetActiveScene().buildIndex + 1);
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

        SceneManager.LoadScene(_index);
    }

    public void OnDeath()
    {
        if (LevelFinished)
            return;

        Timer.Instance.PauseTimer();
        StartCoroutine(ReloadCurrLevel());
    }

    private IEnumerator ReloadCurrLevel()
    {
        anim.ResetTrigger("ToClear");
        anim.SetTrigger("ToBlack");

        yield return new WaitForSeconds(FADE_ANIM_DURATION);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnSceneFinishedLoading()
    {
        anim.ResetTrigger("ToBlack");
        anim.SetTrigger("ToClear");
        StartCoroutine(Timer.Instance.ResumeTimer());
    }
}
