using UnityEngine;

public class FilterController : MonoBehaviour
{
    [SerializeField] private AudioReverbFilter filter0;
    [SerializeField] private AudioChorusFilter filter1;
    [SerializeField] private AudioHighPassFilter filter2;
    [SerializeField] private AudioLowPassFilter filter3;

    private void OnValidate()
    {
        if (filter0 == null)
            filter0 = GetComponent<AudioReverbFilter>();

        if (filter1 == null)
            filter1 = GetComponent<AudioChorusFilter>();

        if (filter2 == null)
            filter2 = GetComponent<AudioHighPassFilter>();

        if (filter3 == null)
            filter3 = GetComponent<AudioLowPassFilter>();
    }

    private void Start()
    {
        SetFilterStatus(false);
    }

    private bool currFilterStatus = false;
    public void SetFilterStatus(bool _status)
    {
        if (currFilterStatus == _status)
            return;

        if (filter0 != null) filter0.enabled = _status;
        if (filter1 != null) filter1.enabled = _status;
        if (filter2 != null) filter2.enabled = _status;
        if (filter3 != null) filter3.enabled = _status;

        currFilterStatus = _status;
    }
}
