using UnityEngine;

public class Music : Singleton<Music>
{
    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
        audioSource.volume = SaveData.CurrSaveData.MusicVol;
    }

    public void ChangeMusicVolume(float _vol)
    {
        audioSource.volume = _vol;
    }
}
