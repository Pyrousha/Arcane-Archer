using UnityEngine;

public class SubmenuController : Singleton<SubmenuController>
{
    // Update is called once per frame
    void Update()
    {
        if (InputHandler.Instance.Pause.Down && SceneTransitioner.IsFading == false)
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
