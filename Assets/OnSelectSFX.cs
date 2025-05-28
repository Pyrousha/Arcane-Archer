using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnSelectSFX : MonoBehaviour, ISelectHandler
{
    [SerializeField] private Selectable selectable;

    private void OnValidate()
    {
        if (selectable == null)
            selectable = GetComponent<Selectable>();
    }

    private void Awake()
    {
        if (selectable.gameObject.TryGetComponent(out Button button))
        {
            button.onClick.AddListener(() => { SFXManager.Instance.Play(SFXManager.AudioTypeEnum.BUTTON_CLICK); });
        }
        if (selectable.gameObject.TryGetComponent(out Slider slider))
        {
            slider.onValueChanged.AddListener((val) => { SFXManager.Instance.Play(SFXManager.AudioTypeEnum.BUTTON_SELECT); });
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        SFXManager.Instance.Play(SFXManager.AudioTypeEnum.BUTTON_SELECT);
    }
}
