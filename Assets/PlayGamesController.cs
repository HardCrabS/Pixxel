using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Events;

public class PlayGamesController : MonoBehaviour
{
    public static PlayGamesController Instance;
    public UnityEvent OnAuthenticated;

    void Start()
    {
        if (!GameData.gameData.isAuthentificated)
        {
            WaitForAuthenticate();
        }
    }

    private async void WaitForAuthenticate()
    {
        await AuthenticateUser();
        OnAuthenticated.Invoke();
    }

    static async Task AuthenticateUser()
    {
        if (ShareController.CheckForInternetConnection())
        {
            string playerId = SystemInfo.deviceUniqueIdentifier;
            Task<User> userTask = DatabaseManager.UserAlreadyInDatabase(playerId);
            User userInDatabase = await userTask;

            if (userInDatabase == null)
            {
                string playerName = "Soldier";
                DatabaseManager.WriteNewUser(playerId, playerName, "Noobe", "Sprites/UI Images/Trinkets/DefaultAvatar", "Sprites/UI images/Banners/DefaultBanner");
                GameData.gameData.saveData.playerInfo = new User(playerId, playerName, "Noobe", "Sprites/UI Images/Trinkets/DefaultAvatar", "Sprites/UI images/Banners/DefaultBanner");
                GameData.Save();
            }
            else
            {
                GameData.gameData.saveData.playerInfo = userInDatabase;
            }
            GameData.gameData.isAuthentificated = true;
        }
        else
        {
            if (GameData.gameData.saveData.playerInfo == null)
            {
                GameData.gameData.saveData.playerInfo = new User("unknown", "Warior", "Noobe", "UI Images/Trinkets/DefaultAvatar", "Sprites/UI images/Banners/DefaultBanner");
                GameData.Save();
            }
        }
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