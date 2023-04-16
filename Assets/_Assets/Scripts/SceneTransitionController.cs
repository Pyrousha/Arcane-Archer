using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionController : Singleton<SceneTransitionController>
{
    public bool LevelFinished { get; private set; } = false;

    [SerializeField] private Animator anim;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
            return;

        LevelFinished = true;
        StartCoroutine(OnLevelFinished());
    }

    private IEnumerator OnLevelFinished()
    {
        anim.SetTrigger("FadeToBlack");

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
