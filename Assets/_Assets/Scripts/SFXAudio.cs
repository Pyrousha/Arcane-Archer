using UnityEngine;

public class SFXAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float volume = 1;


    private void Start()
    {
        ChangeVolume();
    }

    private void ChangeVolume()
    {
        audioSource.volume = volume * SaveData.CurrSaveData.SfxVol;
    }

    public void Play()
    {
        ChangeVolume();
        audioSource.Play();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }
#endif
}
