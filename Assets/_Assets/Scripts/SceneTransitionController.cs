using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionController : Singleton<SceneTransitionController>
{
    public bool LevelFinished { get; private set; } = false;

    [SerializeField] private Animator anim;
    [SerializeField] private Timer timer;

    private void OnTriggerEnter(Collider other)
    {
        timer.PauseTimer();

        LevelFinished = true;
        StartCoroutine(OnLevelFinished());
    }

    public void ToMainMenu()
    {
        StartCoroutine(ToMainMenuRoutine());
    }

    private IEnumerator ToMainMenuRoutine()
    {
        anim.SetTrigger("FadeToBlack");

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(0);
    }

    private IEnumerator OnLevelFinished()
    {
        anim.SetTrigger("FadeToBlack");

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ToNextLevel()
    {
        LevelFinished = true;
        StartCoroutine(OnLevelFinished());
    }

    public void OnDeath()
    {
        if (LevelFinished)
            return;

        timer.PauseTimer();

        StartCoroutine(ReloadCurrLevel());
    }

    private IEnumerator ReloadCurrLevel()
    {
        anim.SetTrigger("FadeToBlack");

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
