using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuCanvas : Submenu
{
    [SerializeField] private GameObject parent;
    [SerializeField] private Button resumeButton;
    public bool IsOpen { get; private set; } = false;

    #region Singleton
    private static PauseMenuCanvas instance = null;

    public static PauseMenuCanvas Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<PauseMenuCanvas>();
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

    public void TryOpen()
    {
        if (SceneTransitioner.CurrBuildIndex > SceneTransitioner.MAIN_MENU_INDEX && SceneTransitioner.CurrBuildIndex != SceneTransitioner.CREDITS_SCENE_INDEX)
        {
            if (!SceneTransitioner.Instance.LevelFinished)
            {
                if (IsOpen)
                    ToLastSubmenu();
                else
                    SelectFromPast(null);
            }
        }
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
        if (IsOpen)
            return;

        IsOpen = true;

        Cursor.lockState = CursorLockMode.None;

        resumeButton.Select();

        parent.SetActive(true);
        Time.timeScale = 0;
    }

    private void ClosePopup()
    {
        if (!IsOpen)
            return;

        Cursor.lockState = CursorLockMode.Locked;
        Debug.Log("Locked mouse!");

        parent.SetActive(false);
        Time.timeScale = 1;

        StartCoroutine(ClosePopupRoutine());
    }

    private IEnumerator ClosePopupRoutine()
    {
        yield return null;
        yield return new WaitForSeconds(0.1f);

        IsOpen = false;
    }

    public void OnResumeClicked()
    {
        ToLastSubmenu();
    }

    public void OnMainMenuClicked()
    {
        ToLastSubmenu();

        SceneTransitioner.Instance.ToMainMenu();
    }

    public void OnRetryClicked()
    {
        ToLastSubmenu();

        SceneTransitioner.Instance.LoadSceneWithIndex(SceneTransitioner.CurrBuildIndex);
    }

    public void OnOptionsClicked()
    {
        SettingsCanvas.Instance.SelectFromPast(this);
    }
}
