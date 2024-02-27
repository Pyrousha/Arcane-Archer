using UnityEngine;
using UnityEngine.InputSystem.UI;

public class SubmenuController : Singleton<SubmenuController>
{
    [SerializeField] private InputSystemUIInputModule inputModule;

    // Update is called once per frame
    void Update()
    {
        if (InputHandler.Instance.Pause.Down)
        {
            if (Submenu.ActiveSubmenu == null)
                PauseMenuCanvas.Instance.TryOpen();
        }
    }

    public void OnCancelPressed()
    {
        if (Submenu.ActiveSubmenu != null)
        {
            Submenu.ActiveSubmenu.ToLastSubmenu();
        }
        else
            Debug.LogError("Somehow Cancel was pressed with no active submenu???");
    }
}
