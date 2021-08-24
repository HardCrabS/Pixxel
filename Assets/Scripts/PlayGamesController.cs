using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Events;
using System.Collections;
using System;

public class PlayGamesController : MonoBehaviour
{
    public static PlayGamesController Instance;
    public UnityEvent OnAuthenticated;

    const string DEFAULT_NAME = "Soldier";
    const string DEFAULT_TITLE = "The Beginner";

    void Start()
    {
        if (!GameData.gameData.isAuthentificated)
        {
            StartCoroutine(checkInternetConnection((isConnected) =>
            {
                if (isConnected)
                {
                    WaitForAuthenticate();
                }
                else
                {
                    if (GameData.gameData.saveData.playerInfo == null)
                    {
                        GameData.gameData.saveData.playerInfo = new User("unknown", DEFAULT_NAME, DEFAULT_TITLE, "UI Images/Trinkets/DefaultAvatar", "Sprites/UI images/Banners/DefaultBanner");
                        GameData.Save();
                    }
                }
            }));
        }
    }
    public static IEnumerator checkInternetConnection(Action<bool> action)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)//wifi isn't turned on
        {
            action(false);
        }
        else
        {
            WWW www = new WWW("http://google.com");
            yield return www;
            if (www.error != null)
            {
                action(false);
            }
            else
            {
                action(true);
            }
        }
    }
    private async void WaitForAuthenticate()
    {
        await AuthenticateUser();
        OnAuthenticated.Invoke();
    }
    
    public static void WriteNewUser(string username)
    {
        string playerId = SystemInfo.deviceUniqueIdentifier;
        DatabaseManager.WriteNewUser(playerId, username, DEFAULT_TITLE, "Sprites/UI Images/Trinkets/DefaultAvatar", "Sprites/UI images/Banners/DefaultBanner");
        GameData.gameData.saveData.playerInfo = new User(playerId, username, DEFAULT_TITLE, "Sprites/UI Images/Trinkets/DefaultAvatar", "Sprites/UI images/Banners/DefaultBanner");
        GameData.Save();
        GameData.gameData.isAuthentificated = true;
    }

    static async Task AuthenticateUser()
    {
        string playerId = SystemInfo.deviceUniqueIdentifier;
        Task<User> userTask = DatabaseManager.UserAlreadyInDatabase(playerId);
        User userInDatabase = await userTask;

        if (userInDatabase == null)
        {
            DatabaseManager.WriteNewUser(playerId, DEFAULT_NAME, DEFAULT_TITLE, "Sprites/UI Images/Trinkets/DefaultAvatar", "Sprites/UI images/Banners/DefaultBanner");
            GameData.gameData.saveData.playerInfo = new User(playerId, DEFAULT_NAME, DEFAULT_TITLE, "Sprites/UI Images/Trinkets/DefaultAvatar", "Sprites/UI images/Banners/DefaultBanner");
            GameData.Save();
        }
        else
        {
            GameData.gameData.saveData.playerInfo = userInDatabase;
        }
        GameData.gameData.isAuthentificated = true;
    }

    public static void PostToLeaderboard(string worldId)
    {
        if (!GameData.gameData.isAuthentificated)
        {
            AuthenticateUser();
        }
        string playerId = SystemInfo.deviceUniqueIdentifier;

        int score = GameData.gameData.saveData.worldBestScores[worldId];


        if (!DatabaseManager.ChildExists(playerId, worldId))
        {
            DatabaseManager.WriteNewScore(worldId, playerId, score);
        }
        else
            DatabaseManager.OverwriteTheScore(worldId, playerId, score);
    }
}