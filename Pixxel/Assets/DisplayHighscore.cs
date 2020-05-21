using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class DisplayHighscore : MonoBehaviour 
{
    [SerializeField] Text firstPlaceScoreText;
    [SerializeField] Text scoresText;

	// Use this for initialization
	void Start () 
    {

	}

    public void LoadScores()
    {
        PlayGamesPlatform.Instance.LoadScores(
            "CgkIlY7Xnp8CEAIQAQ",
            LeaderboardStart.PlayerCentered,
            10,
            LeaderboardCollection.Public,
            LeaderboardTimeSpan.AllTime,
            (data) =>
            {
                LoadUsersAndDisplay(data.Scores);
            });
    }
    internal void LoadUsersAndDisplay(IScore[] allScores)
    {
        // get the user ids
        List<string> userIds = new List<string>();

        int playerPlace = 0;

        foreach (IScore score in allScores)
        {
            userIds.Add(score.userID);
        }
        if(userIds.Count == 0)
        {
            firstPlaceScoreText.text = "No players found =(";
            Debug.LogWarning("0 users in leaderboard");
            return;
        }

        // load the profiles and display
        Social.LoadUsers(userIds.ToArray(), (users) =>
        {
            IUserProfile firstUser = FindUser(users, allScores[0].userID);
            firstPlaceScoreText.text = "#" + 1 + "  |  " + firstUser.userName + "  |  " + allScores[0].formattedValue + "\n\n";
            scoresText.text = "\n";

            for(int i = 1; i < allScores.Length; i++)
            {
                IUserProfile user = FindUser(users, allScores[i].userID);
                if(user.id == Social.localUser.id)
                {
                    scoresText.color = Color.red;
                    scoresText.text += "#" + (i + 1) + "  |  " + user.userName + "  |  " + allScores[i].formattedValue + "\n\n";
                    scoresText.color = Color.black;
                    playerPlace = i + 1;
                    continue;
                }
                scoresText.text += "#" + (i + 1) + "  |  " + user.userName + "  |  " + allScores[i].formattedValue + "\n\n";
            }
        });

        ShiftTextToShowPlayer(playerPlace);
    }

    IUserProfile FindUser(IUserProfile[] users, string userId)
    {
        for (int i = 0; i < users.Length; i++)
        {
            if (users[i].id == userId)
                return users[i];
        }
        return null;
    }

    void ShiftTextToShowPlayer(int playerPlace)
    {
        Vector2 startTextPos = scoresText.transform.position;
        Vector2 shiftedTextPos = new Vector2(startTextPos.x, startTextPos.y + (playerPlace - 1) * 70);

        scoresText.transform.position = shiftedTextPos;
    }
}