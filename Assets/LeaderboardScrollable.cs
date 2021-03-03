using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardScrollable : MonoBehaviour
{
    [Header("First Place Info")]
    [SerializeField] Text firstPlaceScoreText;
    [SerializeField] Text firstPlaceTitleText;
    [SerializeField] Image firstPlaceBanner;
    [SerializeField] Image firstPlaceImage;

    [Header("Common")]
    [SerializeField] GameObject scorePanel;
    [SerializeField] Transform allScoresContainer;
    [SerializeField] int scorePanelsToSpawnFromPlayer = 5;
    [SerializeField] float spawnScorePanelOffset = 1;

    float currScrollOffset = 0;
    int playerIndex;
    int currCenterIndex;

    List<GameObject> scorePanels;
    private User[] allUsers;
    void Start()
    {
        if (ShareController.CheckForInternetConnection())
        {
            scorePanels = new List<GameObject>();
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
            : LevelSettingsKeeper.settingsKeeper.worldInfo.id;
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
        FillListWithPanels();
        if (playerIndex >= 0)   //player is in leaderboard
        {
            currCenterIndex = scorePanels.Count / 2;
            SetPanelsAroundPlayer();
        }
        else
        {
            //SpawnFirst3Players();
            currCenterIndex = 1;
        }
    }

    void FillListWithPanels()
    {
        foreach (Transform panel in allScoresContainer)
        {
            scorePanels.Add(panel.gameObject);
        }
    }

    void SetPanelsAroundPlayer()
    {
        int startIndex = Mathf.Clamp(playerIndex - scorePanelsToSpawnFromPlayer, 0, playerIndex);
        int endIndex = Mathf.Clamp(playerIndex + scorePanelsToSpawnFromPlayer, 0, allUsers.Length - 1);

        int scorePanelIndex = 0;
        for (int i = startIndex; i <= endIndex; i++)
        {
            SetScorePanel(allUsers[i], i, scorePanels[scorePanelIndex]);
            scorePanelIndex++;
        }
    }
    void SetScorePanel(User user, int index, GameObject _scorePanel)
    {
        _scorePanel.GetComponent<Image>().sprite = Resources.Load<Sprite>(user.bannerPath);

        Text text = _scorePanel.GetComponentInChildren<Text>();
        text.text = "\t#<size=450><color=yellow>" + (index + 1) + "</color></size>  |  " + user.username
        + "  |  " + user.score + "\n\n";
        _scorePanel.transform.Find("Title Panel/Title Text").GetComponent<Text>().text = "\"" + user.titleText + "\"";
        Image[] images = _scorePanel.GetComponentsInChildren<Image>();
        Sprite userSprite = Resources.Load<Sprite>(user.spritePath);
        images[1].sprite = userSprite != null ? userSprite : Resources.Load<Sprite>("Sprites/UI images/Trinkets/DefaultAvatar");
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
    public void ScrollContainer(Vector2 value)
    {
        print(value);
        print(currScrollOffset);
        currScrollOffset += value.y;
        if (Mathf.Abs(currScrollOffset) >= spawnScorePanelOffset)
        {
            int offsetSign = currScrollOffset < 0 ? 1 : -1;

            if (currCenterIndex + offsetSign * scorePanelsToSpawnFromPlayer >= allUsers.Length 
                || currCenterIndex + offsetSign * scorePanelsToSpawnFromPlayer <= 0)
            {
                currScrollOffset = 0;
                return;
            }

            currCenterIndex += offsetSign;
            int indexToShow = currCenterIndex + offsetSign;   //index of score panel to show
            int indexToReplace = offsetSign == 1 ? 0 : scorePanels.Count - 1;   // index of panel to move
            SetScorePanel(allUsers[indexToShow], indexToShow, scorePanels[indexToReplace]);
            RectTransform scorePanelRectTransform = scorePanels[indexToReplace].GetComponent<RectTransform>();
            if (offsetSign == 1)    //scrolling down
            {
                scorePanelRectTransform.SetAsLastSibling();
            }
            else       //scrolling up
            {
                scorePanelRectTransform.SetAsFirstSibling();
            }
            currScrollOffset = 0;
        }
    }
}