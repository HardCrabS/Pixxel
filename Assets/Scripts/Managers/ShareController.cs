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

    [SerializeField] int coinsRewardForAdVideo = 50;

    [SerializeField] int facebookReward = 50;
    [SerializeField] WorldInformation facebookWorldInfo;

    [SerializeField] int twitterReward = 50;
    [SerializeField] WorldInformation twitterWorldInfo;

    [SerializeField] AudioClip sharingSuccessClip;

    private const string TWITTER_ADDRESS = "http://twitter.com/intent/tweet";
    private const string TWEET_LANGUAGE = "en";
    private string appStoreLink = "https://culagames.com/";
    public static string descriptionParam = "PiXXel is a match-3 casual game with pretty graphics and nice music!";

    const string SHARE_SUCCESS = "Success! Come back tomorrow for more rewards!";

    string fbWorldId, twWorldId;

    void Start()
    {
        fbWorldId = facebookWorldInfo.id;
        twWorldId = twitterWorldInfo.id;

        if (!GameData.gameData.saveData.worldIds.Contains(fbWorldId))
        {
            fbText.text = "<size=250><color=red>New World!</color></size>\nShare to unlock a world!";
        }
        else
        {
            fbWorldId = null; // null to check world is already claimed
        }
        if (!GameData.gameData.saveData.worldIds.Contains(twWorldId))
        {
            twText.text = "<size=250><color=red>New World!</color></size>\nShare to unlock a world!";
        }
        else
        {
            twWorldId = null;
        }
        CheckTime(twText, twButton, GameData.gameData.saveData.nextPossibleTwitterShare);
        CheckTime(fbText, fbButton, GameData.gameData.saveData.nextPossibleFacebookShare);
    }

    public void RewardForAdVideo()
    {
        CoinsDisplay.Instance.AddCoinsAmount(coinsRewardForAdVideo);
    }

    public void ShareToTW(string linkParameter)
    {
        StartCoroutine(PlayGamesController.checkInternetConnection((isConnected) =>
        {
            if (isConnected)
            {
                //if (!DayPassed(twText, twButton, GameData.gameData.saveData.lastTwitterShare)) { return; };
                GameData.gameData.saveData.nextPossibleTwitterShare = DateTime.Now.ToString();
                GameData.Save();

                string nameParameter = "I'm playing this super game for months!";//this is limited in text length 
                Application.OpenURL(TWITTER_ADDRESS +
                   "?text=" + WWW.EscapeURL(nameParameter + "\n" + descriptionParam + "\n" + "Get the Game:\n" + appStoreLink));
                if (twWorldId == null)  //check if sharing the first time or not
                {
                    StartCoroutine(DelayedCoinsReward(twitterReward, twText));
                }
                else
                {
                    StartCoroutine(DelayedWorldReward(twWorldId, twText));
                }
                twButton.interactable = false;
            }
            else
            {
                twText.text = "Oops! No internet connection found!";
            }
        }));
    }

    public void ShareToFacebook()
    {
        StartCoroutine(PlayGamesController.checkInternetConnection((isConnected) =>
        {
            if (isConnected)
            {
                //if (DayPassed(fbText, fbButton, GameData.gameData.saveData.lastFacebookShare)) { return; };
                GameData.gameData.saveData.nextPossibleFacebookShare = (DateTime.Now).ToString();
                GameData.Save();

                string facebookshare = "https://www.facebook.com/sharer/sharer.php?u=" + Uri.EscapeUriString(appStoreLink);
                Application.OpenURL(facebookshare);
                if (fbWorldId == null)     //check if sharing the first time or not
                {
                    StartCoroutine(DelayedCoinsReward(facebookReward, fbText));
                }
                else
                {
                    StartCoroutine(DelayedWorldReward(fbWorldId, fbText));
                }
                fbButton.interactable = false;
            }
            else
            {
                fbText.text = "Oops! No internet connection found!";
            }
        }));
    }

    IEnumerator DelayedCoinsReward(int amount, Text text)
    {
        yield return new WaitForSeconds(5);
        CoinsDisplay.Instance.AddCoinsAmount(amount);
        text.text = SHARE_SUCCESS + "\n<color=red>" + amount + " coins recived!</color>";
        GetComponent<AudioSource>().PlayOneShot(sharingSuccessClip);
    }

    IEnumerator DelayedWorldReward(string worldId, Text text)
    {
        yield return new WaitForSeconds(5);
        GameData.gameData.UnlockWorld(worldId);   //unlock world reward
        text.text = SHARE_SUCCESS + "\n<color=red>The world is unlocked!</color>";
        GetComponent<AudioSource>().PlayOneShot(sharingSuccessClip);
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
}