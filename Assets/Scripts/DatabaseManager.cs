using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using System.Threading.Tasks;

public class DatabaseManager : MonoBehaviour
{
    public static void WriteNewUser(string userId, string name, string extraText, string spritePath, string bannerPath)
    {
        DatabaseReference mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        User user = new User(userId, name, extraText, spritePath, bannerPath);
        string json = JsonUtility.ToJson(user);

        mDatabaseRef.Child("users").Child(userId).SetRawJsonValueAsync(json);
    }

    public static void ChangeAvatar(string spritePath)
    {
        string playerId = SystemInfo.deviceUniqueIdentifier;
        /*if (string.IsNullOrEmpty(playerId))
        {
            Debug.LogError("Not authentificated to google, can't upload a score");
            return;
        }*/

        DatabaseReference mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        mDatabaseRef.Child("users").Child(playerId).Child("spritePath").SetValueAsync(spritePath);
    }

    public static void ChangeName(string name)
    {
        string playerId = SystemInfo.deviceUniqueIdentifier;
        //TODO uncomment 
        /*if (string.IsNullOrEmpty(playerId))
        {
            Debug.LogError("Not authentificated to google, can't upload a score");
            return;
        }*/

        DatabaseReference mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        mDatabaseRef.Child("users").Child(playerId).Child("username").SetValueAsync(name);
    }

    public static void ChangeTitle(string title)
    {
        string playerId = SystemInfo.deviceUniqueIdentifier;
        //TODO uncomment 
        /*if (string.IsNullOrEmpty(playerId))
        {
            Debug.LogError("Not authentificated to google, can't upload a score");
            return;
        }*/

        DatabaseReference mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        mDatabaseRef.Child("users").Child(playerId).Child("titleText").SetValueAsync(title);
    }
    public static void ChangeBanner(string spritePath)
    {
        string playerId = SystemInfo.deviceUniqueIdentifier;
        //string playerId = "editor12345";
        /*if (string.IsNullOrEmpty(playerId))
        {
            Debug.LogError("Not authentificated to google, can't upload a score");
            return;
        }*/

        DatabaseReference mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        mDatabaseRef.Child("users").Child(playerId).Child("bannerPath").SetValueAsync(spritePath);
    }
    public static void OverwriteTheScore(string worldName, string userId, int score)
    {
        // Set up the Editor before calling into the realtime database.
        //FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://pixxel-89933121.firebaseio.com/");

        // Get the root reference location of the database.
        DatabaseReference mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        string json = JsonUtility.ToJson(score);

        int currBestScore = System.Convert.ToInt32
            (mDatabaseRef.Child(worldName + "/user-scores/" + userId).Child("score").GetValueAsync().Result.Value);
        if (score > currBestScore)
            mDatabaseRef.Child(worldName + "/user-scores/" + userId).Child("score").SetRawJsonValueAsync(json);
    }

    public static void WriteNewScore(string worldName, string userId, int score)
    {
        // Get the root reference location of the database.
        DatabaseReference mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        // Create new entry at /user-scores/$userid/$scoreid and at /leaderboard/$scoreid simultaneously
        //string key = mDatabaseRef.Child(worldName + "/scores").Push().Key;

        LeaderboardEntry entry = new LeaderboardEntry(userId, score);
        Dictionary<string, object> entryValues = entry.ToDictionary();

        Dictionary<string, object> childUpdates = new Dictionary<string, object>();
        childUpdates[worldName + "/user-scores/" + userId] = entryValues;

        mDatabaseRef.UpdateChildrenAsync(childUpdates);
    }

    public static async Task<User[]> GetAllUsersInfo(string worldName)
    {
        User[] allUsers = null;
        //FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://pixxel-89933121.firebaseio.com/");

        await FirebaseDatabase.DefaultInstance.GetReference("users").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                int i = 0;
                allUsers = new User[snapshot.ChildrenCount];
                foreach (DataSnapshot child in snapshot.Children)
                {
                    allUsers[i] = JsonUtility.FromJson<User>(child.GetRawJsonValue());
                    i++;
                }
            }
        });

        User[] usersInLeaderboard = null;
        await FirebaseDatabase.DefaultInstance.GetReference(worldName + "/user-scores").OrderByChild("score")
            .GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                int i = 0;
                usersInLeaderboard = new User[snapshot.ChildrenCount];

                foreach (DataSnapshot child in snapshot.Children)
                {
                    usersInLeaderboard[i] = FindUserWithId(allUsers, child.Child("uid").Value as string);
                    int score = System.Convert.ToInt32(child.Child("score").Value);

                    usersInLeaderboard[i].score = score;
                    i++;
                }
            }
        });

        return usersInLeaderboard;
    }

    static User FindUserWithId(User[] users, string id)
    {
        for (int i = 0; i < users.Length; i++)
        {
            if (users[i].id.CompareTo(id) == 0)
            {
                return users[i];
            }
        }
        return null;
    }

    public static bool ChildExists(string id, string worldName)
    {
        bool childExists = false;

        //FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://pixxel-89933121.firebaseio.com/");

        FirebaseDatabase.DefaultInstance.GetReference(worldName + "/user-scores").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot.HasChild(id))
                {
                    childExists = true;
                }
            }
        });

        return childExists;
    }

    public static async Task<bool> UserAlreadyInDatabase(string id)
    {
        bool foundUser = false;

        await FirebaseDatabase.DefaultInstance.GetReference("users/" + id).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                foundUser = true;
                Debug.LogError("Failed to access database");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    foundUser = true;
                    print("USER EXISTS");
                }
            }
        });

        return foundUser;
    }
}

[System.Serializable]
public class User
{
    public string id;
    public string username;
    public string titleText;
    public string spritePath;
    public string bannerPath;
    public int score;

    public User()
    {
    }

    public User(string id, string username, string titleText, string spritePath, string bannerSpritePath)
    {
        this.id = id;
        this.username = username;
        this.titleText = titleText;
        this.spritePath = spritePath;
        this.bannerPath = bannerSpritePath;
    }
}

public class LeaderboardEntry
{
    public string uid;
    public int score = 0;

    public LeaderboardEntry()
    {

    }

    public LeaderboardEntry(string uid, int score)
    {
        this.uid = uid;
        this.score = score;
    }

    public Dictionary<string, object> ToDictionary()
    {
        Dictionary<string, object> result = new Dictionary<string, object>();
        result["uid"] = uid;
        result["score"] = score;

        return result;
    }
}