using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class UsernameCheck : MonoBehaviour
{
    [SerializeField] TextAsset badWordTxt;
    public TMP_InputField inputField;

    public UnityEvent onUsernameConfirm = new UnityEvent();
    
    string[] badWords;

    private void Start()
    {
        inputField = GetComponentInChildren<TMP_InputField>();
        inputField.onEndEdit.AddListener(delegate { ConfirmUsername(inputField); });

        char[] archDelim = new char[] { '\r', '\n' };
        badWords = badWordTxt.text.Split(archDelim, System.StringSplitOptions.RemoveEmptyEntries);
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
            string lowerUsername = username.ToLower();
            if (lowerUsername.Contains(word))
            {
                //bad word is short and can be just a part of normal long word
                if (word.Length == 3 && lowerUsername != word)
                    return true;
                //bad word
                return false;
            }
        }
        return true;
    }
}