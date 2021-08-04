using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class UsernameCheck : MonoBehaviour
{
    [SerializeField] string[] badWords;

    public UnityEvent onUsernameConfirm = new UnityEvent();

    public TMP_InputField inputField;

    private void Start()
    {
        inputField = GetComponentInChildren<TMP_InputField>();
        inputField.onEndEdit.AddListener(delegate { ConfirmUsername(inputField); });
    }

    void ConfirmUsername(TMP_InputField input)
    {
        string username = input.text;
        if(UsernameIsClear(username))
        {
            //save username
            GameData.gameData.saveData.playerInfo.username = username;
            GameData.Save();

            onUsernameConfirm.Invoke();
        }
        else
        {
            inputField.text = "";
            inputField.placeholder.GetComponent<TextMeshProUGUI>().text = "Username is unacceptable!";
        }
    }

    bool UsernameIsClear(string username)
    {
        foreach (var word in badWords)
        {
            if (username.ToLower().Contains(word))
                return false;
        }
        return true;
    }
}