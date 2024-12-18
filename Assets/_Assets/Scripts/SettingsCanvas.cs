using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsCanvas : Submenu
{
    private bool isOpen = false;

    [SerializeField] private GameObject parent;
    [Space(10)]
    [SerializeField] private Slider sensSlider;
    [SerializeField] private TMP_InputField sensInputField;
    [Space(5)]
    [SerializeField] private Slider fovSlider;
    [SerializeField] private TMP_InputField fovInputField;
    [Space(10)]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private TMP_InputField musicInputField;
    [Space(5)]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TMP_InputField sfxInputField;
    [Space(5)]
    [SerializeField] private Toggle screenshakeToggle;
    [SerializeField] private Toggle timerToggle;
    [SerializeField] private Toggle tutToggle;


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

    private const float SENS_MULTIPLIER = 0.25f;

    private void Start()
    {
        LoadValuesFromSaveData();
    }

    public void LoadValuesFromSaveData()
    {
        sensSlider.value = SaveData.CurrSaveData.MouseSens;
        sensInputField.text = Mathf.RoundToInt(SaveData.CurrSaveData.MouseSens * 255).ToString();
        PlayerController.MouseSens = Mathf.Round(SaveData.CurrSaveData.MouseSens * 255f * SENS_MULTIPLIER);

        fovSlider.value = Utils.Remap(SaveData.CurrSaveData.Fov, 60, 120, 0, 1);
        fovInputField.text = Mathf.RoundToInt(SaveData.CurrSaveData.Fov).ToString();

        musicSlider.value = SaveData.CurrSaveData.MusicVol;
        musicInputField.text = Mathf.RoundToInt(SaveData.CurrSaveData.MusicVol * 100).ToString();

        sfxSlider.value = SaveData.CurrSaveData.SfxVol;
        sfxInputField.text = Mathf.RoundToInt(SaveData.CurrSaveData.SfxVol * 100).ToString();

        tutToggle.isOn = SaveData.CurrSaveData.ShowTutText;
        timerToggle.isOn = SaveData.CurrSaveData.ShowTimer;
        screenshakeToggle.isOn = SaveData.CurrSaveData.EnableScreenshake;
    }

    public override void OnSubmenuSelected()
    {
        OpenPopup();
    }
    public override void OnSubmenuClosed()
    {
        ClosePopup();
    }

    private void OpenPopup()
    {
        if (isOpen)
            return;

        isOpen = true;
        parent.SetActive(true);
    }

    private void ClosePopup()
    {
        if (!isOpen)
            return;

        isOpen = false;
        parent.SetActive(false);

        if (Timer.Instance != null)
            Timer.Instance.RefreshTimerState();

        if (TutorialText.Instance != null)
            TutorialText.Instance.SetText();

        SaveData.Instance.Save();
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
        //_vol is the new value that the player selected from the slider

        SaveData.CurrSaveData.MusicVol = Mathf.Clamp(_vol, 0, 1);
        musicInputField.text = Mathf.RoundToInt(SaveData.CurrSaveData.MusicVol * 100).ToString();

        Music.Instance.ChangeMusicVolume(SaveData.CurrSaveData.MusicVol);
    }

    public void OnMusicVolChanged_InputField(string _volStr)
    {
        try
        {
            SaveData.CurrSaveData.MusicVol = Mathf.Clamp(float.Parse(_volStr) / 100f, 0, 1);
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
            SaveData.CurrSaveData.SfxVol = Mathf.Clamp(float.Parse(_volStr) / 100f, 0, 1);
        }
        catch { }

        sfxSlider.value = SaveData.CurrSaveData.SfxVol;
        sfxInputField.text = Mathf.RoundToInt(SaveData.CurrSaveData.SfxVol * 100).ToString();
    }

    public void OnSensChanged_Slider(float _percent)
    {
        SaveData.CurrSaveData.MouseSens = Mathf.Clamp(_percent, 0, 1);
        sensInputField.text = Mathf.RoundToInt(SaveData.CurrSaveData.MouseSens * 255).ToString();

        PlayerController.MouseSens = Mathf.Round(SaveData.CurrSaveData.MouseSens * 255f * SENS_MULTIPLIER);
    }

    public void OnSensChanged_InputField(string _sensText)
    {
        try
        {
            SaveData.CurrSaveData.MouseSens = Mathf.Clamp(float.Parse(_sensText) / 255f, 0, 1);
        }
        catch { }

        sensSlider.value = SaveData.CurrSaveData.MouseSens;
        sensInputField.text = Mathf.RoundToInt(SaveData.CurrSaveData.MouseSens * 255).ToString();

        PlayerController.MouseSens = Mathf.Round(SaveData.CurrSaveData.MouseSens * 255f * SENS_MULTIPLIER);
    }

    public void OnFovChanged_Slider(float _percent)
    {
        SaveData.CurrSaveData.Fov = Utils.Remap(Mathf.Clamp(_percent, 0, 1), 0, 1, 60, 120);
        fovInputField.text = Mathf.RoundToInt(SaveData.CurrSaveData.Fov).ToString();

        if (ObjReferencer.Instance != null)
            ObjReferencer.Instance.MainCamera.fieldOfView = SaveData.CurrSaveData.Fov;
    }

    public void OnFovChanged_InputField(string _fovText)
    {
        try
        {
            SaveData.CurrSaveData.Fov = Mathf.Clamp(float.Parse(_fovText), 60, 120);
        }
        catch { }

        fovSlider.value = Utils.Remap(SaveData.CurrSaveData.Fov, 60, 120, 0, 1);
        fovInputField.text = Mathf.RoundToInt(SaveData.CurrSaveData.Fov).ToString();

        if (ObjReferencer.Instance != null)
            ObjReferencer.Instance.MainCamera.fieldOfView = SaveData.CurrSaveData.Fov;
    }

    public void OnTutToggled(bool _on)
    {
        SaveData.CurrSaveData.ShowTutText = _on;
    }

    public void OnTimerToggled(bool _on)
    {
        SaveData.CurrSaveData.ShowTimer = _on;
    }

    public void OnScreenshakeToggled(bool _on)
    {
        SaveData.CurrSaveData.EnableScreenshake = _on;
    }
}
