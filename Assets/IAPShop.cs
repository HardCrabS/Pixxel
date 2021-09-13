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
    [SerializeField] CanvasGroup consumableButtons;
    [SerializeField] CanvasGroup nonConsumableButtons;

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
        if (GameData.gameData.saveData.adsRemoved)//ads removed (no-ads purchase already made)
        {
            //show consumable
            consumableButtons.alpha = 1;
            consumableButtons.interactable = true;
            consumableButtons.blocksRaycasts = true;

            //hide non-consumable
            nonConsumableButtons.alpha = 1;
            nonConsumableButtons.interactable = false;
            nonConsumableButtons.blocksRaycasts = false;
        }
        else
        {
            //ads aren't removed

            //hide consumable
            consumableButtons.alpha = 0;
            consumableButtons.interactable = false;
            consumableButtons.blocksRaycasts = false;

            //show non-consumable
            nonConsumableButtons.alpha = 1;
            nonConsumableButtons.interactable = true;
            nonConsumableButtons.blocksRaycasts = true;
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
        ActivateButtonSet();//activate consumable buttons after purchase

        for (int i = 0; i < rewardsForPurchase.Length; i++)
        {
            switch (rewardsForPurchase[i].reward)
            {
                case LevelReward.Banner:
                    {
                        GameData.gameData.UnlockBanner(rewardsForPurchase[i].id);
                        break;
                    }
                case LevelReward.Title:
                    {
                        GameData.gameData.UnlockTitle(rewardsForPurchase[i].id);
                        break;
                    }
                case LevelReward.World:
                    {
                        GameData.gameData.UnlockWorld(rewardsForPurchase[i].id);
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