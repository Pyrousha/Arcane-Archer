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

    private void Update()
    {
        if (InputHandler.Instance.Pause.Down)
        {
            if (SceneTransitioner.CurrBuildIndex > SceneTransitioner.MAIN_MENU_INDEX && SceneTransitioner.CurrBuildIndex != SceneTransitioner.CREDITS_SCENE_INDEX)
            {
                if (!SceneTransitioner.Instance.LevelFinished)
                {
                    if (IsOpen)
                        OnResumeClicked();
                    else
                        OpenPopup();
                }
            }
        }
    }

    public override void OnSubmenuSelected()
    {
        OpenPopup();
    }

    public void OpenPopup()
    {
        if (IsOpen)
            return;

        IsOpen = true;

        Cursor.lockState = CursorLockMode.None;

        resumeButton.Select();

        parent.SetActive(true);
        Time.timeScale = 0;
    }

    public void ClosePopup()
    {
        if (!IsOpen)
            return;

        parent.SetActive(false);
        Time.timeScale = 1;

        ToLastSubmenu();

        StartCoroutine(ClosePopupRoutine());
    }

    private IEnumerator ClosePopupRoutine()
    {
        //yield return null;
        //yield return null;
        yield return null;
        yield return new WaitForSeconds(0.1f);

        IsOpen = false;
    }

    public void OnResumeClicked()
    {
        Cursor.lockState = CursorLockMode.Locked;
        ClosePopup();
    }

    public void OnMainMenuClicked()
    {
        Cursor.lockState = CursorLockMode.None;
        ClosePopup();

        SceneTransitioner.Instance.ToMainMenu();
    }

    public void OnRetryClicked()
    {
        Cursor.lockState = CursorLockMode.Locked;
        ClosePopup();

        SceneTransitioner.Instance.LoadSceneWithIndex(SceneTransitioner.CurrBuildIndex);
    }

    public void OnOptionsClicked()
    {
        //OptionsMenuCanvas.Instance.OpenPopup();
    }
}
