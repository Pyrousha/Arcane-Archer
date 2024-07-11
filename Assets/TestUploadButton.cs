using UnityEngine;

public class TestUploadButton : MonoBehaviour
{
    [SerializeField] private int score;

    public void OnClick()
    {
        LeaderboardCallHandler.Instance.UpdateScore(score);
    }
}
