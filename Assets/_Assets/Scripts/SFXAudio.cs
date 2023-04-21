using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
        ChangeVolume();
    }

    public void ChangeVolume()
    {
        audioSource.volume = Music.sfxVolume;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }
#endif
}
