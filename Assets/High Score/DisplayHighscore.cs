//using GooglePlayGames;
//using GooglePlayGames.BasicApi;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class DisplayHighscore : MonoBehaviour
{
    [Header("First Place Info")]
    [SerializeField] Text firstPlaceScoreText;
    [SerializeField] Text firstPlaceTitleText;
    [SerializeField] Image firstPlaceBanner;
    [SerializeField] Image firstPlaceImage;

    [Header("Common")]
    [SerializeField] GameObject scorePanel;
    [SerializeField] Transform allScoresContainer;
    [SerializeField] GameObject playerArrow; 

    LinkedList<GameObject> scorePanels;

    private User[] allUsers;
    private int playerIndex;
    private int currCenterIndex;

    public static DisplayHighscore Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (ShareController.CheckForInternetConnection())
        {
            scorePanels = new LinkedList<GameObject>();
        }
    }

    public async void SetLeaderboard()
    {
        if (!ShareController.CheckForInternetConnection())
        {
            firstPlaceScoreText.text = "Oops! Internet connection is missing!";
            firstPlaceTitleText.text = "Oops! Internet connection is missing!";
            return;
        }
        if (scorePanels.Count > 0) // if players already loaded
        {
            return;
        }

        string worldName = LevelSettingsKeeper.settingsKeeper == null ? "Twilight City" 
            : LevelSettingsKeeper.settingsKeeper.worldInfo.WorldName;
        var allUsersTask = DatabaseManager.GetAllUsersInfo(worldName);
        allUsers = await allUsersTask;

        if (allUsers == null || allUsers.Length == 0)
        {
            firstPlaceScoreText.text = "No players found in Database";
            return;
        }
        Array.Reverse(allUsers);
        string playerId = SystemInfo.deviceUniqueIdentifier;

        playerIndex = GetPlayerIndex(allUsers, playerId);
        if (playerIndex >= 0)
        {
            currCenterIndex = playerIndex;
            SpawnScoresNearPlayer();
        }
        else
        {
            SpawnFirst3Players();
            currCenterIndex = 1;
        }
        SetFirstPlace();
    }

    public void ClearLeaderboard()
    {
        if (scorePanels.Count > 0)
        {
            while (scorePanels.Count > 0)
            {
                Destroy(scorePanels.First.Value);
                scorePanels.RemoveFirst();
            }
        }
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

    void SpawnScoresNearPlayer()
    {
        int startIndex = 0, endIndex = 0;
        if (playerIndex > 0 && playerIndex < allUsers.Length - 1) //between first and last
        {
            startIndex = playerIndex - 1;
            endIndex = playerIndex + 1;
        }
        else if (playerIndex == allUsers.Length - 1) //last
        {
            startIndex = playerIndex - 2;
            endIndex = playerIndex;
            currCenterIndex = playerIndex - 1;
        }
        else if (playerIndex == 0) //first
        {
            startIndex = 0;
            endIndex = playerIndex + 2;
            currCenterIndex = 1;
        }

        if (startIndex < 0) startIndex = 0;
        if (endIndex >= allUsers.Length) endIndex = allUsers.Length - 1;

        for (int i = startIndex; i <= endIndex; i++)
        {
            GameObject scorePanelClone = SpawnScorePanel(allUsers[i], i);
            scorePanels.AddLast(scorePanelClone);
        }
    }

    private void SpawnPlayerArrow(GameObject scorePanelClone) //arrow points to player score
    {
        var arrow = Instantiate(playerArrow, scorePanelClone.transform);
        Vector3 arrowPos = arrow.transform.position;
        arrow.transform.position = new Vector3(arrowPos.x, scorePanelClone.transform.position.y, arrowPos.z);

        //shift text and image to the right
        Transform child1 = scorePanelClone.transform.GetChild(0).GetComponent<RectTransform>();
        child1.position = new Vector3(child1.position.x + 25, child1.position.y, child1.position.z);

        Transform child2 = scorePanelClone.transform.GetChild(1).GetComponent<RectTransform>();
        child2.position = new Vector3(child2.position.x + 25, child2.position.y, child2.position.z);
    }

    void SpawnFirst3Players()
    {
        int firstPlayers = allUsers.Length >= 3 ? 3 : allUsers.Length;
        for (int i = 0; i < firstPlayers; i++)
        {
            GameObject scorePanelClone = SpawnScorePanel(allUsers[i], i);
            scorePanels.AddLast(scorePanelClone);
        }
    }

    void SetFirstPlace()
    {
        firstPlaceScoreText.text = "\t#<size=450><color=red>" + 1 + "</color></size>  |  " + allUsers[0].username
        + "  |  " + allUsers[0].score + "\n\n";
        firstPlaceTitleText.text = "\"" + allUsers[0].titleText + "\"";
        firstPlaceImage.sprite = Resources.Load<Sprite>(allUsers[0].spritePath);
        firstPlaceBanner.sprite = Resources.Load<Sprite>(allUsers[0].bannerPath);
    }

    public void SpawnScorePanelUp()
    {
        if (currCenterIndex - 1 <= 0)
        {
            return;
        }
        currCenterIndex--;
        GameObject scorePanelClone = SpawnScorePanel(allUsers[currCenterIndex - 1], currCenterIndex - 1);
        scorePanels.AddFirst(scorePanelClone);

        if (scorePanels.Count > 3)
        {
            Destroy(scorePanels.Last.Value);
            scorePanels.RemoveLast();
        }

        RectTransform scorePanelRectTransform = scorePanelClone.GetComponent<RectTransform>();
        scorePanelRectTransform.SetAsFirstSibling();
    }
    public void SpawnScorePanelDown()
    {
        if (currCenterIndex + 2 >= allUsers.Length)
        {
            return;
        }
        currCenterIndex++;
        GameObject scorePanelClone = SpawnScorePanel(allUsers[currCenterIndex + 1], currCenterIndex + 1);
        scorePanels.AddLast(scorePanelClone);

        if (scorePanels.Count > 3)
        {
            Destroy(scorePanels.First.Value);
            scorePanels.RemoveFirst();
        }

        RectTransform scorePanelRectTransform = scorePanelClone.GetComponent<RectTransform>();
        scorePanelRectTransform.SetAsLastSibling();
    }

    GameObject SpawnScorePanel(User user, int index)
    {
        GameObject go = Instantiate(scorePanel, allScoresContainer);
        if (index == playerIndex)
        {
            SpawnPlayerArrow(go);
        }

        go.GetComponent<Image>().sprite = Resources.Load<Sprite>(user.bannerPath);

        Text text = go.GetComponentInChildren<Text>();
        text.text = "\t#<size=450><color=yellow>" + (index + 1) + "</color></size>  |  " + user.username
        + "  |  " + user.score + "\n\n";
        go.transform.Find("Title Panel/Title Text").GetComponent<Text>().text = "\"" + user.titleText + "\"";
        Image[] images = go.GetComponentsInChildren<Image>();
        images[1].sprite = Resources.Load<Sprite>(user.spritePath);

        return go;
    }
}