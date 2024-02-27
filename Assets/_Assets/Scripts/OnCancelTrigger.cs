using UnityEngine;
using UnityEngine.EventSystems;

public class OnCancelTrigger : MonoBehaviour, ICancelHandler
{
    public void OnCancel(BaseEventData eventData)
    {
        SubmenuController.Instance.OnCancelPressed();
    }
}
