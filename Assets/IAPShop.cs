using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IAPShop : MonoBehaviour
{
    [SerializeField] GameObject exclusiveItems;
    [SerializeField] GameObject restoreButton;
    [SerializeField] AudioClip coinsPurchasedClip;
    
    [SerializeField] RewardTemplate[] rewardsForPurchase;

    [Header("Button sets")]
    [SerializeField] GameObject consumableButtons;
    [SerializeField] GameObject nonConsumableButtons;

    [Header("Old man")]
    [SerializeField] Button oldManBubble;
    [SerializeField] string[] thingsToSay;

    // Start is called before the first frame update
    void Start()
    {
        ActivateButtonSet();
        DisableRestoreButton();
        InitOldManBubble();
        DisableExclusiveRewardsText();
    }

    void ActivateButtonSet()
    {
        if (GameData.gameData.saveData.adsRemoved)
        {
            consumableButtons.SetActive(true);
            nonConsumableButtons.SetActive(false);
        }
        else
        {
            consumableButtons.SetActive(false);
            nonConsumableButtons.SetActive(true);
        }
    }

    void DisableExclusiveRewardsText()
    {
        if (GameData.gameData.saveData.adsRemoved)
        {
            exclusiveItems.SetActive(false);
        }
    }

    void InitOldManBubble()
    {
        if(GameData.gameData.saveData.adsRemoved)
        {
            oldManBubble.interactable = false;
            oldManBubble.GetComponentInChildren<Text>().text 
                = thingsToSay[Random.Range(0, thingsToSay.Length)];
        }
    }

    public void AddRewardsForPurchace()
    {
        GetComponent<AudioSource>().PlayOneShot(coinsPurchasedClip);

        //ads removed, all rewards already received
        if(GameData.gameData.saveData.adsRemoved)
        {
            return;
        }

        GameData.gameData.RemoveAds();
        DisableExclusiveRewardsText();
        InitOldManBubble();

        for (int i = 0; i < rewardsForPurchase.Length; i++)
        {
            switch (rewardsForPurchase[i].reward)
            {
                case LevelReward.Banner:
                    {
                        GameData.gameData.saveData.bannerIds.Add(rewardsForPurchase[i].id);
                        break;
                    }
                case LevelReward.Title:
                    {
                        GameData.gameData.saveData.titleIds.Add(rewardsForPurchase[i].id);
                        break;
                    }
                case LevelReward.World:
                    {
                        GameData.gameData.saveData.worldIds.Add(rewardsForPurchase[i].id);
                        break;
                    }
            }
        }
    }

    void DisableRestoreButton()
    {
        if(Application.platform != RuntimePlatform.IPhonePlayer)
        {
            restoreButton.SetActive(false);
        }
    }
}