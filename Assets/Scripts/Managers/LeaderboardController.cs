using Mopsicus.InfiniteScroll;
using System;
using System.Collections;
using UnityEngine;

public class LeaderboardController : MonoBehaviour
{
    [SerializeField] HoldScorePanel firstPlacePanel;

    [SerializeField] InfiniteScroll Scroll;
    [SerializeField] GameObject loadingPanel;
    [SerializeField] GameObject playerArrowPrefab;

    private int Count = 100;

    private User[] allUsers;
    public int playerIndex;

    Transform playerArrow;

    public static LeaderboardController Instance;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        Scroll.OnFill += SetScorePanel;
        Scroll.OnHeight += OnHeightItem;

        //SetLeaderboard();
    }
    public async void SetLeaderboard()
    {
        StartCoroutine(PlayGamesController.checkInternetConnection(async (isConnected) =>
        {
            if (isConnected)
            {
                await GetUsersAndCreatePanels();
            }
            else
            {
                string noInternet = "Oops! Internet connection is missing!";
                firstPlacePanel.SetScorePanel(noInternet, null, null, noInternet);
            }
        }));
    }

    private async System.Threading.Tasks.Task GetUsersAndCreatePanels()
    {
        string loadingStr = "Loading...";
        firstPlacePanel.SetScorePanel(loadingStr, null, null, loadingStr);
        loadingPanel.SetActive(true);
        //enable loading circle on panel
        loadingPanel.transform.GetChild(0).gameObject.SetActive(true);

        string worldName = LevelSettingsKeeper.settingsKeeper == null ? "Twilight City"
            : LevelSettingsKeeper.settingsKeeper.worldInfo.id;
        var allUsersTask = DatabaseManager.GetAllUsersInfo(worldName);
        allUsers = await allUsersTask;

        if (allUsers == null || allUsers.Length == 0)
        {
            firstPlacePanel.SetScorePanel("No players found in Database");
            //disable loading circle on panel
            loadingPanel.transform.GetChild(0).gameObject.SetActive(false);
            return;
        }
        Array.Reverse(allUsers);
        Count = allUsers.Length;

        string playerId = SystemInfo.deviceUniqueIdentifier;
        playerIndex = GetPlayerIndex(allUsers, playerId);
        //playerIndex = 10;
        if (playerIndex >= 0 && playerArrow == null)
            playerArrow = Instantiate(playerArrowPrefab, Vector3.zero, playerArrowPrefab.transform.rotation).transform;
        Scroll.InitData(Count, playerIndex);
        if (playerIndex > 2)
            StartCoroutine(MoveUpAndDown());
        else
            loadingPanel.SetActive(false);
        SetFirstPlace();
    }

    IEnumerator MoveUpAndDown()
    {
        int topIndex = Mathf.Clamp(playerIndex - 10, 0, Count + 1);
        int bottomIndex = Mathf.Clamp(playerIndex + 15, 0, Count + 10);

        int delta = playerIndex - topIndex;
        yield return StartCoroutine(Scroll.MoveByDelta(InfiniteScroll.Direction.Top, delta));

        delta = bottomIndex - topIndex;
        yield return StartCoroutine(Scroll.MoveByDelta(InfiniteScroll.Direction.Bottom, delta));

        delta = Mathf.Clamp(bottomIndex - playerIndex - 4, 0, Count + 1);
        yield return StartCoroutine(Scroll.MoveByDelta(InfiniteScroll.Direction.Top, delta));
        loadingPanel.SetActive(false);
    }
    public void ShowPlayerPos()
    {
        loadingPanel.SetActive(true);
        //string playerId = SystemInfo.deviceUniqueIdentifier;
        //playerIndex = GetPlayerIndex(allUsers, playerId);
        //playerIndex = 10;
        Scroll.InitData(Count, playerIndex);
        if (playerIndex > 2)
            StartCoroutine(MoveUpAndDown());
        else
            loadingPanel.SetActive(false);
    }
    int GetPlayerIndex(User[] allUsers, string playerId)
    {
        for (int i = 0; i < allUsers.Length; i++)
        {
            if (allUsers[i].id.CompareTo(playerId) == 0)
            {
                return i;
            }
        }
        return -1;
    }
    void SetFirstPlace()
    {
        string name = "\t#<size=450><color=red>" + 1 + "</color></size>  |  " + allUsers[0].username
        + "  |  " + allUsers[0].score + "\n\n";
        string title = "\"" + allUsers[0].titleText + "\"";
        Sprite userSprite = Resources.Load<Sprite>(allUsers[0].spritePath);
        var profileImage = userSprite != null ? userSprite : Resources.Load<Sprite>("Sprites/UI images/Trinkets/DefaultAvatar");
        var banner = Resources.Load<Sprite>(allUsers[0].bannerPath);

        firstPlacePanel.SetScorePanel(name, profileImage, banner, title);
    }
    void SetScorePanel(int index, GameObject _scorePanel)
    {
        if (index == playerIndex)
        {
            SetPlayerArrow(_scorePanel);
        }
        else if (playerArrow && playerArrow.parent == _scorePanel.transform)
        {
            playerArrow.SetParent(null, false);
            playerArrow.position = Vector3.zero;
        }
        User user = allUsers[index];
        HoldScorePanel holdScorePanel = _scorePanel.GetComponent<HoldScorePanel>();

        var banner = Resources.Load<Sprite>(user.bannerPath);
        var name = "\t#<size=450><color=yellow>" + (index + 1) + "</color></size>  |  " + user.username
        + "  |  " + user.score + "\n\n";
        var title = "\"" + user.titleText + "\"";
        Sprite userSprite = Resources.Load<Sprite>(user.spritePath);
        var profileImage = userSprite != null ? userSprite : Resources.Load<Sprite>("Sprites/UI images/Trinkets/DefaultAvatar");
        holdScorePanel.SetScorePanel(name, profileImage, banner, title);
    }

    int OnHeightItem(int index)
    {
        return 25;//height of leaderboard panel
    }

    private void SetPlayerArrow(GameObject scorePanelClone) //arrow points to player score
    {
        playerArrow.SetParent(scorePanelClone.transform, false);
        playerArrow.GetComponent<RectTransform>().SetAsFirstSibling();
    }
}