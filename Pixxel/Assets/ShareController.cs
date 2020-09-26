using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShareController : MonoBehaviour
{
    [SerializeField] Button fbButton;
    [SerializeField] Text fbText;

    [SerializeField] Button twButton;
    [SerializeField] Text twText;

    [SerializeField] int facebookReward = 50;
    [SerializeField] int facebookWorldIndex = 5;

    [SerializeField] int twitterReward = 50;
    [SerializeField] int twitterWorldIndex = 8;

    private const string TWITTER_ADDRESS = "http://twitter.com/intent/tweet";
    private const string TWEET_LANGUAGE = "en";
    public static string descriptionParam = "PiXXel is a match-3 casual game with pretty graphics and nice music!";
    private string appStoreLink = "http://www.PiXXel.com";

    const string SHARE_SUCCESS = "Success! Come back tomorrow for more rewards!";

    void Start()
    {
        CheckTime(twText, twButton, GameData.gameData.saveData.lastTwitterShare);
        CheckTime(fbText, fbButton, GameData.gameData.saveData.lastFacebookShare);
    }

    public void ShareToTW(string linkParameter)
    {
        if(!CheckForInternetConnection())
        {
            twText.text = "Oops! No internet connection found!";
            return;
        }
        //if (!DayPassed(twText, twButton, GameData.gameData.saveData.lastTwitterShare)) { return; };
        GameData.gameData.saveData.lastTwitterShare = DateTime.Now.ToString();
        GameData.gameData.Save();

        string nameParameter = "I'm playing this super game for months!";//this is limited in text length 
        Application.OpenURL(TWITTER_ADDRESS +
           "?text=" + WWW.EscapeURL(nameParameter + "\n" + descriptionParam + "\n" + "Get the Game:\n" + appStoreLink));
        if (GameData.gameData.saveData.worldUnlocked[twitterWorldIndex])  //check if sharing the first time or not
        {
            StartCoroutine(DelayedCoinsReward(twitterReward, twText));
        }
        else
        {
            StartCoroutine(DelayedWorldReward(twitterWorldIndex, twText));
        }
        twButton.interactable = false;
    }

    public void ShareToFacebook()
    {
        if (!CheckForInternetConnection())
        {
            fbText.text = "Oops! No internet connection found!";
            return;
        }
        //if (DayPassed(fbText, fbButton, GameData.gameData.saveData.lastFacebookShare)) { return; };
        GameData.gameData.saveData.lastFacebookShare = (DateTime.Now).ToString();
        GameData.gameData.Save();

        string facebookshare = "https://www.facebook.com/sharer/sharer.php?u=" + Uri.EscapeUriString(appStoreLink);
        Application.OpenURL(facebookshare);
        if (GameData.gameData.saveData.worldUnlocked[facebookWorldIndex])     //check if sharing the first time or not
        {
            StartCoroutine(DelayedCoinsReward(facebookReward, fbText));
        }
        else
        {
            StartCoroutine(DelayedWorldReward(facebookWorldIndex, fbText));
        }
        fbButton.interactable = false;
    }

    IEnumerator DelayedCoinsReward(int amount, Text text)
    {
        yield return new WaitForSeconds(5);
        CoinsDisplay.Instance.AddCoinsAmount(amount);
        text.text = SHARE_SUCCESS + "\n<color=red>" + amount + " coins recived!</color>";
    }

    IEnumerator DelayedWorldReward(int index, Text text)
    {
        yield return new WaitForSeconds(5);
        GameData.gameData.UnlockWorld(index);   //unlock world reward
        text.text = SHARE_SUCCESS + "\n<color=red>The world is unlocked!</color>";
    }

    void CheckTime(Text text, Button button, string lastClaimStr)
    {
        DateTime lastClaim;
        if (string.IsNullOrEmpty(lastClaimStr))
        {
            lastClaim = new DateTime(2017, 2, 20);
        }
        else
        {
            lastClaim = Convert.ToDateTime(lastClaimStr);
        }

        if (DateTime.Now.CompareTo(lastClaim.AddHours(12)) < 0)
        {
            button.interactable = false;
            text.text = SHARE_SUCCESS;
        }
    }
    public static bool CheckForInternetConnection()
    {
        try
        {
            using (var client = new System.Net.WebClient())
            using (client.OpenRead("http://google.com/generate_204"))
                return true;
        }
        catch
        {
            return false;
        }
    }
}