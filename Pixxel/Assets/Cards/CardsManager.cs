using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardsManager : MonoBehaviour 
{
    [SerializeField] GameObject cardPanel;
    [SerializeField] GameObject earnedCardPanel;
    [SerializeField] Text titleText;
    [SerializeField] Text descriptionText;
    [SerializeField] Image cardImage;

    [SerializeField] Card[] allCards;

    public void ActivateCardPanel()
    {
        System.DateTime lastClaim;
        if (string.IsNullOrEmpty(GameData.gameData.saveData.lastTimeCardClaimed))
        {
            lastClaim = System.DateTime.Now;
        }
        else
        {
            lastClaim = System.Convert.ToDateTime(GameData.gameData.saveData.lastTimeCardClaimed);
        }

        if (System.DateTime.Now.CompareTo(lastClaim) >= 0)
        {
            cardPanel.SetActive(true);
        }
        else
        {
            DisplayCardInfo(allCards[GameData.gameData.saveData.cardInfoIndex]);
            earnedCardPanel.SetActive(true);
        }
    }

    public void DisplayCardInfo(Card card)
    {
        titleText.text = card.Title;
        descriptionText.text = card.Description;
        cardImage.sprite = card.Sprite;
    }
}
