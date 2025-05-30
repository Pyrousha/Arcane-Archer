using AYellowpaper.SerializedCollections;
using UnityEngine;

public class SFXManager : Singleton<SFXManager>
{
    [System.Serializable]
    public struct SFXReference
    {
        public AudioClip Clip;
        [Range(0f, 1f)] public float VolumeMultiplier;
    }

    public enum AudioTypeEnum
    {
        FULLCHARGE,
        ENTER_BOOM,
        EXIT_BOOM,
        BUTTON_SELECT,
        BUTTON_CLICK
    }

    [SerializeField] private AudioSource source;
    [SerializeField]
    private SerializedDictionary<AudioTypeEnum, SFXReference> clips;


    public void Play(AudioTypeEnum type)
    {
        SFXReference clipToPlay = clips[type];

        source.volume = SaveData.CurrSaveData.SfxVol * clipToPlay.VolumeMultiplier;
        source.PlayOneShot(clipToPlay.Clip);
    }
}
