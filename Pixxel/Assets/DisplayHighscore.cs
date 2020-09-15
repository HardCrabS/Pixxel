using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class DisplayHighscore : MonoBehaviour
{
    [SerializeField] WorldInfoDisplay worldInfoDisplay;
    [SerializeField] Text firstPlaceScoreText;
    [SerializeField] Text firstPlaceTitleText;
    [SerializeField] Image firstPlaceImage;

    [SerializeField] GameObject scorePanel;
    [SerializeField] Transform allScoresContainer;

    LinkedList<GameObject> scorePanels;

    private User[] allUsers;
    private int playerIndex;
    private int currCenterIndex;

    void Start()
    {
        scorePanels = new LinkedList<GameObject>();
    }

    public async void SetLeaderboard()
    {
        if (scorePanels.Count > 0)
        {
            return;
        }
        var allUsersTask = DatabaseManager.GetAllUsersInfo(worldInfoDisplay.worldInformation.WorldName);
        allUsers = await allUsersTask;

        if (allUsers == null)
            return;

        Array.Reverse(allUsers);
        string playerId = PlayGamesPlatform.Instance.localUser.id;
        //string playerId = "editor12345";
        playerIndex = GetPlayerIndex(allUsers, playerId);

        if (playerIndex > 0)
        {
            SpawnScoresNearPlayer();
            currCenterIndex = playerIndex;
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
        if (playerIndex > 0 && playerIndex < allUsers.Length - 1)
        {
            startIndex = playerIndex - 1;
            endIndex = playerIndex + 1;
        }
        else if (playerIndex >= allUsers.Length - 1)
        {
            startIndex = playerIndex - 2;
            endIndex = playerIndex;
        }
        else if (playerIndex == 0)
        {
            startIndex = 0;
            endIndex = playerIndex + 2;
        }

        if (startIndex < 0) startIndex = 0;
        if (endIndex >= allUsers.Length) endIndex = allUsers.Length - 1;

        for (int i = startIndex; i <= endIndex; i++)
        {
            GameObject scorePanelClone = SpawnScorePanel(allUsers[i], i);
            scorePanels.AddLast(scorePanelClone);
        }
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
    }

    public void SpawnScorePanelUp()
    {
        if (currCenterIndex - 1 <= 0)
        {
            return;
        }
        currCenterIndex--;
        GameObject scorePanelClone = SpawnScorePanel(allUsers[currCenterIndex - 1], currCenterIndex-1);
        scorePanels.AddFirst(scorePanelClone);

        if (scorePanels.Count > 3)
        {
            Destroy(scorePanels.Last.Value);
            scorePanels.RemoveLast();
        }

        RectTransform rectTransform = allScoresContainer.GetComponent<RectTransform>();
        RectTransform scorePanelRectTransform = scorePanelClone.GetComponent<RectTransform>();
        scorePanelRectTransform.SetAsFirstSibling();

        float scorePanelHeight = scorePanelRectTransform.rect.height;
        float allScoresContainerHeight = rectTransform.rect.height;
    }
    public void SpawnScorePanelDown()
    {
        if (currCenterIndex + 2 >= allUsers.Length)
        {
            return;
        }
        currCenterIndex++;
        GameObject scorePanelClone = SpawnScorePanel(allUsers[currCenterIndex + 1], currCenterIndex+1);
        scorePanels.AddLast(scorePanelClone);

        if (scorePanels.Count > 3)
        {
            Destroy(scorePanels.First.Value);
            scorePanels.RemoveFirst();
        }

        RectTransform rectTransform = allScoresContainer.GetComponent<RectTransform>();
        RectTransform scorePanelRectTransform = scorePanelClone.GetComponent<RectTransform>();
        scorePanelRectTransform.SetAsLastSibling();

        float scorePanelHeight = scorePanelRectTransform.rect.height;
        float allScoresContainerHeight = rectTransform.rect.height;
    }

    GameObject SpawnScorePanel(User user, int index)
    {
        GameObject go = Instantiate(scorePanel, allScoresContainer);

        Text text = go.GetComponentInChildren<Text>();
        text.text = "\t#<size=450><color=yellow>" + (index + 1) + "</color></size>  |  " + user.username
        + "  |  " + user.score + "\n\n";
        go.transform.Find("Title Panel/Title Text").GetComponent<Text>().text = "\"" + user.titleText + "\"";
        Image[] images = go.GetComponentsInChildren<Image>();
        images[1].sprite = Resources.Load<Sprite>(user.spritePath);

        return go;
    }
}