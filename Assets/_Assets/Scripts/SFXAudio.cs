using System.Collections;
using UnityEngine;

public class SFXAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float volume = 1;

    private Coroutine playRoutine;

    private void Start()
    {
        ChangeVolume();
    }

    private void ChangeVolume()
    {
        audioSource.volume = volume * SaveData.CurrSaveData.SfxVol;
    }

    public void Play(float _waitTime = 0)
    {
        if (_waitTime > 0)
        {
            if (playRoutine != null)
                StopCoroutine(playRoutine);

            playRoutine = StartCoroutine(PlayAfterDelay(_waitTime));

            return;
        }

        ChangeVolume();
        audioSource.Play();
    }

    private IEnumerator PlayAfterDelay(float _waitTime)
    {
        yield return new WaitForSeconds(_waitTime);

        ChangeVolume();
        audioSource.Play();

        playRoutine = null;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }
#endif
}
