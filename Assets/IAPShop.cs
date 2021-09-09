using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IAPShop : MonoBehaviour
{
    [SerializeField] GameObject exclusiveItems;
    [SerializeField] GameObject restoreButton;
    
    [SerializeField] RewardTemplate[] rewardsForPurchase;

    [Header("Old man")]
    [SerializeField] Button oldManBubble;
    [SerializeField] string[] thingsToSay;

    // Start is called before the first frame update
    void Start()
    {
        DisableRestoreButton();
        InitOldManBubble();
        DisableExclusiveRewardsText();
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
        GameData.gameData.RemoveAds();
        DisableExclusiveRewardsText();

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