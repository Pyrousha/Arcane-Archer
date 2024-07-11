using System.Collections.Generic;
using UnityEngine;
using static LeaderboardCallHandler;

public class LeaderboardUIController : Submenu
{
    [SerializeField] private GameObject parentObj;
    [SerializeField] private PlayerRank localPlayerRank;
    [SerializeField] private GameObject localPlayerRankParent;

    [Space(10)]
    [SerializeField] private GameObject loadingObj;
    [SerializeField] private GameObject scrollViewObj;

    [Space(10)]
    [SerializeField] private Transform playerListParent;
    [SerializeField] private GameObject playerRankPrefab;

    [Space(10)]
    [field: SerializeField] public Color firstColor;
    [field: SerializeField] public Color secondColor;
    [field: SerializeField] public Color thirdColor;
    [field: SerializeField] public Color normalColor;

    #region Singleton
    private static LeaderboardUIController instance = null;

    public static LeaderboardUIController Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<LeaderboardUIController>();
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

    private bool isCalling = false;

    private void CallLeaderboard()
    {
        if (isCalling)
            return;

        isCalling = true;

        LeaderboardCallHandler.Instance.GetLeaderBoardData(Steamworks.ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, 100);

        loadingObj.SetActive(true);
        scrollViewObj.SetActive(false);
        localPlayerRankParent.SetActive(false);
    }

    public void OnLeaderboardUpdated(List<LeaderboardData> _leaderboardDataset)
    {
        loadingObj.SetActive(false);
        scrollViewObj.SetActive(true);

        for (int i = 0; i < playerListParent.childCount; i++)
        {
            Destroy(playerListParent.GetChild(i).gameObject);
        }

        for (int i = 0; i < _leaderboardDataset.Count; i++)
        {
            PlayerRank newPlayer = Instantiate(playerRankPrefab, playerListParent).GetComponent<PlayerRank>();
            LeaderboardData currPlayerData = _leaderboardDataset[i];
            newPlayer.SetData(currPlayerData);
        }

        //for (int i = 2; i < 67; i++)
        //{
        //    PlayerRank newPlayer = Instantiate(playerRankPrefab, playerListParent).GetComponent<PlayerRank>();
        //    LeaderboardData currPlayerData = new LeaderboardData();
        //    currPlayerData.rank = i;
        //    currPlayerData.username = "Fortnite Guy #" + (i - 1).ToString();
        //    currPlayerData.score = i * 10000 + 100000;
        //    newPlayer.SetData(currPlayerData);
        //}

        isCalling = false;
    }

    public void UpdateLocalLeaderboard(bool isPlayerInLeaderboard, LeaderboardData lD)
    {
        localPlayerRankParent.SetActive(isPlayerInLeaderboard);
        if (isPlayerInLeaderboard)
            localPlayerRank.SetData(lD);
    }

    public override void OnSubmenuSelected()
    {
        parentObj.SetActive(true);

        if (!LeaderboardCallHandler.Instance.IsDownloading)
            CallLeaderboard();
    }

    public override void OnSubmenuClosed()
    {
        parentObj.SetActive(false);
    }
}
