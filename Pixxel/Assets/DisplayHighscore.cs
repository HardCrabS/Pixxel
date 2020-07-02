using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class DisplayHighscore : MonoBehaviour
{
    [SerializeField] WorldInfoDisplay worldInfoDisplay;
    [SerializeField] Text firstPlaceScoreText;

    [SerializeField] GameObject scorePanel;
    [SerializeField] Transform allScoresContainer;

    LinkedList<GameObject> scorePanels;

    private User[] allUsers;
    private string playerId;
    private int playerIndex;
    private int currCenterIndex;

    // Use this for initialization
    public async void SetLeaderboard()
    {
        scorePanels = new LinkedList<GameObject>();
        var allUsersTask = DatabaseManager.GetAllUsersInfo(worldInfoDisplay.worldInformation.WorldName);
        allUsers = await allUsersTask;

        if (allUsers == null)
            return;
        for (int i = 0; i < allUsers.Length; i++)
        {
            print("id: " + allUsers[i].id + " name: " + allUsers[i].username + " score " + allUsers[i].score);
        }
        string playerId = PlayGamesPlatform.Instance.localUser.id;
        playerIndex = GetPlayerIndex(allUsers, playerId);
        currCenterIndex = playerIndex;

        if (playerIndex > 0)
        {
            SpawnScoresNearPlayer();
        }
        SetFirstPlace();
        SpawnScoresNearPlayer();
    }

    public void ClearLeaderboard()
    {
        if (scorePanels.Count > 0)
            scorePanels.Clear();
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
        else if (playerIndex > 0 && playerIndex >= allUsers.Length - 1)
        {
            startIndex = playerIndex - 2;
            endIndex = playerIndex;
        }
        else if (playerIndex == 0 && playerIndex <= allUsers.Length - 1)
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

    void SetFirstPlace()
    {
        firstPlaceScoreText.text = "\t<color=red>#" + 1 + "</color>  |  " + allUsers[0].username
        + "  |  " + allUsers[0].score + "\n\n";
    }

    public void SpawnScorePanelUp()
    {
        if (currCenterIndex - 1 <= 0)
        {
            return;
        }
        currCenterIndex--;
        GameObject scorePanelClone = SpawnScorePanel(allUsers[currCenterIndex - 1], currCenterIndex);
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
        // rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, allScoresContainerHeight + scorePanelHeight + 15);
        // rectTransform.position = new Vector2(allScoresContainer.position.x, allScoresContainer.position.y - 14.5f);
    }
    public void SpawnScorePanelDown()
    {
        if (currCenterIndex + 2 >= allUsers.Length)
        {
            return;
        }
        currCenterIndex++;
        GameObject scorePanelClone = SpawnScorePanel(allUsers[currCenterIndex + 1], currCenterIndex);
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
        // rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, allScoresContainerHeight + scorePanelHeight + 15);
        // rectTransform.position = new Vector2(allScoresContainer.position.x, allScoresContainer.position.y + 14.5f);
    }

    GameObject SpawnScorePanel(User user, int index)
    {
        GameObject go = Instantiate(scorePanel, allScoresContainer);

        Text scoreText = go.GetComponentInChildren<Text>();

        scoreText.text = "\t#" + (index + 1) + "  |  " + user.username
        + "  |  " + user.score + "\n\n";

        //SetRandomText(scoreText);

        return go;
    }
    string[] maleNames = new string[]
    { "aaron", "abdul", "abe", "abel", "abraham", "adam", "adan", "adolfo", "adolph",
        "adrian", "abby", "abigail", "adele", "adrian" };
    void SetRandomText(Text text)
    {
        int nameIndex = Random.Range(0, maleNames.Length);
        text.text = "#" + Random.Range(0, 100) + "\t|\t" + maleNames[nameIndex] + "\t|\t" + Random.Range(0, 10000);
    }
}