using UnityEngine;

public class Music : Singleton<Music>
{
    public static float musicVolume;
    public static float sfxVolume;

    public static Music music = null;
    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
        ChangeMusicVolume(PlayerPrefs.GetFloat("musicVol", 0.5f));
        //sfxVolume = PlayerPrefs.GetFloat("sfxVol", 0.5f);

        //slider.value = musicVolume;
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
}
