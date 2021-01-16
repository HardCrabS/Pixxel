using Firebase;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.Events;

public class FirebaseInit : MonoBehaviour 
{
    public UnityEvent OnFirebaseInitialized = new UnityEvent();
    // Use this for initialization

    void Start () 
    {
        if (GameData.gameData.isAuthentificated)
        {
            Destroy(gameObject);
        }
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if(task.Exception != null)
            {
                Debug.LogError("Failed to initialize Firebase with: " + task.Exception);
                return;
            }

            OnFirebaseInitialized.Invoke();
        });
    }
}
