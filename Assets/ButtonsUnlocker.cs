using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsUnlocker : MonoBehaviour
{
    [Header("Quests")]
    [SerializeField] GameObject questsButton;
    [SerializeField] int rankToUnlockQuests;

    [Header("Cards")]
    [SerializeField] GameObject cardsButton;
    [SerializeField] int rankToUnlockCards;

    [Header("Collection")]
    [SerializeField] GameObject collectionButton;
    [SerializeField] GameObject shopButton;
    [SerializeField] int rankToUnlockCollection;

    [Header("Ads")]
    [SerializeField] GameObject adsButton;
    [SerializeField] int rankToUnlockAdsButton;

    private void Start()
    {
        Init();
    }

    void Init()
    {
        int currLevel = GameData.gameData.saveData.currentLevel;

        if(currLevel < rankToUnlockQuests)
        {
            questsButton.SetActive(false);
        }
        if(currLevel < rankToUnlockCards)
        {
            cardsButton.SetActive(false);
        }
        if(currLevel < rankToUnlockCollection)
        {
            collectionButton.SetActive(false);
            shopButton.SetActive(false);
        }
        if(currLevel < rankToUnlockAdsButton)
        {
            adsButton.SetActive(false);
        }
    }
}