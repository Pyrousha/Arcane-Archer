using TMPro;
using UnityEngine;
using static LeaderboardCallHandler;

public class PlayerRank : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankLabel;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI timeLabel;

    private void SetData(int _rank, string _name, float _time)
    {
        rankLabel.text = $"#{_rank}";
        nameLabel.text = _name;
        timeLabel.text = Timer.TimeToString(_time);

        Color colToUse;
        switch (_rank)
        {
            case 1:
                colToUse = LeaderboardUIController.Instance.firstColor;
                break;
            case 2:
                colToUse = LeaderboardUIController.Instance.secondColor;
                break;
            case 3:
                colToUse = LeaderboardUIController.Instance.thirdColor;
                break;
            default:
                colToUse = LeaderboardUIController.Instance.normalColor;
                break;
        }

        rankLabel.color = colToUse;
        nameLabel.color = colToUse;
        timeLabel.color = colToUse;
    }

    public void SetData(LeaderboardData data)
    {
        SetData(data.rank, data.username, data.score / (1000f));
    }
}
