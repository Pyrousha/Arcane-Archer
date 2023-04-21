using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Music : Singleton<Music>
{
    public static float musicVolume;
    public static float sfxVolume;

    public static Music music = null;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Slider slider;

    private void Start()
    {
        if (music == null)
            music = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        ChangeMusicVolume(PlayerPrefs.GetFloat("musicVol", 0.5f));
        //sfxVolume = PlayerPrefs.GetFloat("sfxVol", 0.5f);

        slider.value = musicVolume;
    }

    public void ChangeMusicVolume(float _vol)
    {
        PlayerPrefs.SetFloat("musicVol", _vol);

        musicVolume = _vol;
        sfxVolume = _vol;

        audioSource.volume = sfxVolume;
    }

    // public void ChangeSFXVolume(float _vol)
    // {
    //     sfxVolume = _vol;
    // }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }
#endif
}
