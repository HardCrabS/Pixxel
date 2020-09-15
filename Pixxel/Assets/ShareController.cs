using System;
using UnityEngine;

public class ShareController : MonoBehaviour
{
    private const string TWITTER_ADDRESS = "http://twitter.com/intent/tweet";
    private const string TWEET_LANGUAGE = "en";
    public static string descriptionParam = "PiXXel is a match-3 casual game with pretty graphics and nice music!";
    private string appStoreLink = "http://www.PiXXel.com";

    public void ShareToTW(string linkParameter)
    {
        string nameParameter = "I'm playing this super game for months!";//this is limited in text length 
        Application.OpenURL(TWITTER_ADDRESS +
           "?text=" + WWW.EscapeURL(nameParameter + "\n" + descriptionParam + "\n" + "Get the Game:\n" + appStoreLink));
    }

    public void ShareToFacebook()
    {
        string facebookshare = "https://www.facebook.com/sharer/sharer.php?u=" + Uri.EscapeUriString(appStoreLink);
        Application.OpenURL(facebookshare);
    }
}