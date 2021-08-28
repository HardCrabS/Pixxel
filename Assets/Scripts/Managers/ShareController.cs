using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShareController : MonoBehaviour
{
    [SerializeField] ShareButton shareButton;
    [SerializeField] Text shareText;

    [SerializeField] int coinsReward = 50;
    [SerializeField] WorldInformation rewardWorldInfo;

    [SerializeField] AudioClip sharingSuccessClip;

    const string SHARE_SUCCESS = "Success! Come back tomorrow for more rewards!";

    string worldId;

    void Start()
    {
        worldId = rewardWorldInfo.id;

        if (!GameData.gameData.saveData.worldIds.Contains(worldId))
        {
            shareText.text = "<size=250><color=red>New World!</color></size>\nShare to unlock a world!";
        }
        else
        {
            worldId = null;
        }
        shareButton.OnShareSuccess.AddListener(RewardForSharing);
        CheckTime(GameData.gameData.saveData.nextPossibleTwitterShare);
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
        CoinsDisplay.Instance.AddCoinsAmount(amount);
        shareText.text = SHARE_SUCCESS + "\n<color=red>" + amount + " coins recived!</color>";
        GetComponent<AudioSource>().PlayOneShot(sharingSuccessClip);
    }

    void WorldReward(string worldId)
    {
        GameData.gameData.UnlockWorld(worldId);   //unlock world reward
        shareText.text = SHARE_SUCCESS + "\n<color=red>The world is unlocked!</color>";
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