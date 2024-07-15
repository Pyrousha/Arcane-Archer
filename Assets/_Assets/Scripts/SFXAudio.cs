using UnityEngine;

public class SFXAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float volume = 1;


    private void Start()
    {
        ChangeVolume();
    }

    public void ChangeVolume()
    {
        audioSource.volume = volume * SaveData.CurrSaveData.SfxVol;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }
#endif
}
