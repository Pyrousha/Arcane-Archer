using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsCanvas : Submenu
{
    private bool isOpen = false;

    [SerializeField] private GameObject parent;
    [Space(10)]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private TMP_InputField musicInputField;
    [Space(10)]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TMP_InputField sfxInputField;


    #region Singleton
    private static SettingsCanvas instance = null;

    public static SettingsCanvas Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<SettingsCanvas>();
            return instance;
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning("Duplicate instance of singleton found: " + gameObject.name + ", destroying.");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    #endregion

    private void Start()
    {
        musicSlider.value = SaveData.CurrSaveData.MusicVol;
        musicInputField.text = Mathf.RoundToInt(SaveData.CurrSaveData.MusicVol * 100).ToString();

        sfxSlider.value = SaveData.CurrSaveData.SfxVol;
        sfxInputField.text = Mathf.RoundToInt(SaveData.CurrSaveData.SfxVol * 100).ToString();
    }

    public override void OnSubmenuSelected()
    {
        OpenPopup();
    }

    private void OpenPopup()
    {
        if (isOpen)
            return;

        isOpen = true;
        parent.SetActive(true);
    }

    public void ClosePopup()
    {
        if (!isOpen)
            return;

        isOpen = false;
        parent.SetActive(false);

        ToLastSubmenu();
    }

    public void OnFullscreenTypeChanged(int _resolutionType)
    {
        switch (_resolutionType)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;

            case 1:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;

            case 2:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;

            default:
                Debug.LogError("Unknown fullscreen type: " + _resolutionType);
                break;
        }
    }

    public void OnMusicVolChanged_Slider(float _vol)
    {
        SaveData.CurrSaveData.MusicVol = Mathf.Clamp(_vol, 0, 1);
        musicInputField.text = Mathf.RoundToInt(SaveData.CurrSaveData.MusicVol * 100).ToString();

        Music.Instance.ChangeMusicVolume(SaveData.CurrSaveData.MusicVol);
    }

    public void OnMusicVolChanged_InputField(string _volStr)
    {
        try
        {
            SaveData.CurrSaveData.MusicVol = Mathf.Clamp(float.Parse(_volStr), 0, 1);
        }
        catch { }

        musicSlider.value = SaveData.CurrSaveData.MusicVol;
        musicInputField.text = Mathf.RoundToInt(SaveData.CurrSaveData.MusicVol * 100).ToString();

        Music.Instance.ChangeMusicVolume(SaveData.CurrSaveData.MusicVol);
    }

    public void OnSfxVolChanged_Slider(float _vol)
    {
        SaveData.CurrSaveData.SfxVol = Mathf.Clamp(_vol, 0, 1);
        sfxInputField.text = Mathf.RoundToInt(SaveData.CurrSaveData.SfxVol * 100).ToString();
    }

    public void OnSfxVolChanged_InputField(string _volStr)
    {
        try
        {
            SaveData.CurrSaveData.SfxVol = Mathf.Clamp(float.Parse(_volStr), 0, 1);
        }
        catch { }

        sfxSlider.value = SaveData.CurrSaveData.SfxVol;
        sfxInputField.text = Mathf.RoundToInt(SaveData.CurrSaveData.SfxVol * 100).ToString();
    }
}
