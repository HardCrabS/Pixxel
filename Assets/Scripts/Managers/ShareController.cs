using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShareController : MonoBehaviour
{
    [SerializeField] ShareButton shareButton;
    [SerializeField] Text shareText;

	[SerializeField] int coinsRewardForAdVideo = 50;
    [SerializeField] int coinsReward = 50;
    [SerializeField] WorldInformation rewardWorldInfo;
    [SerializeField] Boost boostInfo;

    [SerializeField] AudioClip sharingSuccessClip;

    const string SHARE_SUCCESS = "Success! Come back tomorrow for more rewards!";

    string worldId;

    void Start()
    {
        worldId = rewardWorldInfo.id;

        if (!GameData.gameData.saveData.worldIds.Contains(worldId))
        {
            shareText.text = "<size=250><color=red>New World & Boost!</color></size>\nShare to unlock the exclusive <color=orange>Beansprout Archipelago</color> World & <color=orange>Earthquake</color> Boost!";
        }
        else
        {
            shareText.text = "<size=300><color=red>FREE GOLD!</color></size>\n<size=200>Share to receive:</size>\n\n<color=orange><size=250>25 GOLD!</size></color>";
            worldId = null;
        }
        shareButton.OnShareSuccess.AddListener(RewardForSharing);
        CheckTime(GameData.gameData.saveData.nextPossibleTwitterShare);
    }

    public void RewardForAdVideo()
    {
        CoinsDisplay.Instance.AddCoinsAmount(coinsRewardForAdVideo, punch:true);
    }

    void RewardForSharing()
    {
        StartCoroutine(PlayGamesController.checkInternetConnection((isConnected) =>
        {
            if (isConnected)
            {
                if (worldId == null)  //check if sharing the first time or not
                {
                    CoinsReward(coinsReward);
                }
                else
                {
                    WorldReward(worldId);
                }

                GameData.gameData.saveData.nextPossibleTwitterShare = DateTime.Now.ToString();
                GameData.Save();

                shareButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                shareText.text = "Oops! No internet connection found!";
            }
        }));
    }
    
    void CoinsReward(int amount)
    {
        CoinsDisplay.Instance.AddCoinsAmount(amount, punch:true);
        shareText.text = SHARE_SUCCESS + "\n<color=red>" + amount + " coins recived!</color>";
        GetComponent<AudioSource>().PlayOneShot(sharingSuccessClip);
    }

    void WorldReward(string worldId)
    {
        GameData.gameData.UnlockWorld(worldId);   //unlock world reward
        GameData.gameData.UnlockBoost(boostInfo.id); //unlock boost
        shareText.text = SHARE_SUCCESS + "\n<color=red>The World & Boost are unlocked!</color>";
        GetComponent<AudioSource>().PlayOneShot(sharingSuccessClip);
    }

    void CheckTime(string lastClaimStr)
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
            shareButton.GetComponent<Button>().interactable = false;
            shareText.text = SHARE_SUCCESS;
        }
    }
}