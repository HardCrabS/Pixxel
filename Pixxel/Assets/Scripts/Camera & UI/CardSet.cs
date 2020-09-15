using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSet : MonoBehaviour 
{
    [SerializeField] CardsManager cardsManager;
    [SerializeField] Card[] cardsInSet;
	
    public void DisplayCardsInSet()
    {
        int randIndex = Random.Range(0, cardsInSet.Length);
        cardsManager.DisplayCardInfo(cardsInSet[randIndex]);
        GameData.gameData.UpdateCardClaim(System.DateTime.Now.AddHours(12), randIndex, cardsInSet[randIndex].CardType);
    }
}