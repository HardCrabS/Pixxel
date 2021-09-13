using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Threading.Tasks;

public class DatabaseWindow : EditorWindow
{
    string username = "";
    string id = "";
    User user = null;
    string newPlayerName = "";

    [MenuItem("Tools/Players Database")]
    public static void ShowWindow()
    {
        GetWindow<DatabaseWindow>("Players Database");
    }

    void OnGUI()
    {
        DisplayUserInfo();

        GUILayout.Space(20);

        GUILayout.Label("Enter a username of the player", EditorStyles.boldLabel);
        username = EditorGUILayout.TextField("username", username);

        EditorGUI.BeginChangeCheck();

        if(GUILayout.Button("Find player by name"))
        {
            FindUsersWithNameAsync(username);
        }

        GUILayout.Space(10);
        GUILayout.Label("Enter an id of the player", EditorStyles.boldLabel);
        id = EditorGUILayout.TextField("id", id);


        if (GUILayout.Button("Find player by id"))
        {
            FindUserWithId(id);
        }


        GUILayout.Space(20);
        EditUserButtons();

        EditorGUI.EndChangeCheck();
    }

    void DisplayUserInfo()
    {
        GUILayout.Label("Current user info:", EditorStyles.largeLabel);
        GUILayout.Label("id: " + (user != null ? user.id : " "), EditorStyles.boldLabel);
        GUILayout.Label("Username: " + (user != null ? user.username : " "), EditorStyles.boldLabel);
        GUILayout.Label("Avatar: " + (user != null ? user.spritePath : " "), EditorStyles.boldLabel);
        GUILayout.Label("Banner: " + (user != null ? user.bannerPath : " "), EditorStyles.boldLabel);
        GUILayout.Label("Title: " + (user != null ? user.titleText : " "), EditorStyles.boldLabel);
    }

    async void FindUsersWithNameAsync(string name)
    {
        User[] usersWithName = await DatabaseManager.FindUsersWithName(name);

        foreach (var user in usersWithName)
        {
            Debug.Log("Found user: " + user.username + "    id: " + user.id);
        }
        if(usersWithName.Length == 0)
        {
            ShowNotification(new GUIContent("No users found"));
            Debug.Log("No users with such username found.");
            user = null;
        }
        else if(usersWithName.Length == 1)
        {
            user = usersWithName[0];
        }
    }

    async void FindUserWithId(string id)
    {
        user = await DatabaseManager.UserAlreadyInDatabase(id);
        if (user == null)
        {
            ShowNotification(new GUIContent("User not found"));
            Debug.Log("User with id not found.");
        }
        else
        {
            Debug.Log("Found user with name: " + user.username);
        }
    }

    void EditUserButtons()
    {
        EditorGUI.BeginDisabledGroup(user == null);

        newPlayerName = EditorGUILayout.TextField("New player name: ", newPlayerName);
        if(GUILayout.Button("Rename player"))
        {
            if(!string.IsNullOrEmpty(newPlayerName))
            {
                DatabaseManager.ChangeName(newPlayerName, user.id);
                user.username = newPlayerName;
            }
        }

        EditorGUI.EndDisabledGroup();
    }
}