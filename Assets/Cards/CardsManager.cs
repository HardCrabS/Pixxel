using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardsManager : MonoBehaviour
{
    [SerializeField] GameObject exclamationBubble; //shows if card unclaimed
    [SerializeField] GameObject cardPanel;
    [SerializeField] GameObject earnedCardPanel;
    [SerializeField] Text titleText;
    [SerializeField] Text descriptionText;
    [SerializeField] Image cardImage;

    [SerializeField] CardSet cardSet;
    [SerializeField] Button[] cardButtons;

    const string FIRST_DRAW_CARD = "FirstDraw";
    const string FOOL_CARD = "Fool";
    const string KING_CARD = "King";

    private void Start()
    {
        DateTime lastClaim;
        if (string.IsNullOrEmpty(GameData.gameData.saveData.nextPossibleCardClaime))
        {
            lastClaim = DateTime.Now;
        }
        else
        {
            lastClaim = Convert.ToDateTime(GameData.gameData.saveData.nextPossibleCardClaime);
        }

        if (DateTime.Now.CompareTo(lastClaim) >= 0)
        {
            lastClaim = new DateTime(2017, 2, 20);
            GameData.gameData.UpdateCardClaim(lastClaim, null);//remove last claimed card
            exclamationBubble.SetActive(true);
        }
        else
            exclamationBubble.SetActive(false);
    }

    void SetCardSets()
    {
        for (int i = 0; i < cardButtons.Length; i++)
        {
            int index = i;
            cardButtons[i].onClick.AddListener(delegate () { OnCardPressed(index); });
        }
    }

    void OnCardPressed(int index)
    {
        //disallow pressing other cards
        foreach (var card in cardButtons)
        {
            card.interactable = false;
        }

        Animator animator = cardButtons[index].GetComponent<Animator>();
        animator.enabled = true;

        float clipLength = animator.GetCurrentAnimatorStateInfo(0).length;
        DisplayCardsInSet(index);
        StartCoroutine(CardPanelDelayed(clipLength));
        exclamationBubble.SetActive(false);
    }

    IEnumerator CardPanelDelayed(float time)    //card reveal
    {
        yield return new WaitForSeconds(time + 0.5f);
        //gameObject.AddComponent<AudioSource>().PlayOneShot(cardRevealClip);
        earnedCardPanel.SetActive(true);
    }

    public void DisplayCardsInSet(int cardSetIndex)
    {
        Card[] cardsInSet = cardSet.CardsInSet;
        int randIndex = UnityEngine.Random.Range(0, cardsInSet.Length);

        if(PlayerPrefs.GetInt(FIRST_DRAW_CARD, 1) == 1)//first time card is drawn
        {
            //ensure card is not FOOL(worst one)
            while(cardsInSet[randIndex].Title.CompareTo(FOOL_CARD) == 0)
            {
                randIndex = UnityEngine.Random.Range(0, cardsInSet.Length);
            }
            PlayerPrefs.SetInt(FIRST_DRAW_CARD, 1);
        }
        if (cardsInSet[randIndex].Title.CompareTo(FOOL_CARD) == 0)//card is a fool
        {
            //add animated coin
            StartCoroutine(CardTossCoin(1));
        }
        else if (cardsInSet[randIndex].Title.CompareTo(KING_CARD) == 0)//card is king
        {
            StartCoroutine(CardTossCoin(50));
        }
        DisplayCardInfo(cardsInSet[randIndex]);
        GameData.gameData.UpdateCardClaim(System.DateTime.Now.AddHours(12), cardsInSet[randIndex].CardType);
        cardButtons[cardSetIndex].GetComponent<SpriteChanger>().SetSprite(cardsInSet[randIndex].Sprite);

        MobileNotificationManager notification = new MobileNotificationManager();
        notification.SendNotification("Your Reward is READY!",
            "Don't forget to claim your Daily Reward in Pixxel!", 12);
    }

    public void ActivateCardPanel()
    {
        System.DateTime lastClaim;
        if (string.IsNullOrEmpty(GameData.gameData.saveData.nextPossibleCardClaime))
        {
            lastClaim = System.DateTime.Now;
        }
        else
        {
            lastClaim = System.Convert.ToDateTime(GameData.gameData.saveData.nextPossibleCardClaime);
        }

        if (System.DateTime.Now.CompareTo(lastClaim) >= 0)
        {
            SetCardSets();
            cardPanel.SetActive(true);
        }
        else
        {
            DisplayCardInfo(GetCardInArray(GameData.gameData.saveData.cardType));
            earnedCardPanel.SetActive(true);
        }
    }

    public void DisplayCardInfo(Card card)
    {
        titleText.text = card.Title;
        descriptionText.text = card.Description;
        cardImage.sprite = card.Sprite;
    }

    IEnumerator CardTossCoin(int amount)
    {
        yield return new WaitUntil(() => earnedCardPanel.activeInHierarchy);
        CoinsDisplay.Instance.AddCoinsWithCoinAnim(amount, cardImage.rectTransform);
    }

    Card GetCardInArray(string cardType)
    {
        var allCards = cardSet.CardsInSet;
        for (int i = 0; i < allCards.Length; i++)
        {
            if (allCards[i].CardType.CompareTo(cardType) == 0)
            {
                return allCards[i];
            }
        }
        return null;
    }
}
