using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardsManager : MonoBehaviour 
{
    [SerializeField] GameObject exclamationBubble; //shows if card card unclaimed
    [SerializeField] GameObject cardPanel;
    [SerializeField] GameObject earnedCardPanel;
    [SerializeField] Text titleText;
    [SerializeField] Text descriptionText;
    [SerializeField] Image cardImage;
    [SerializeField] AudioClip cardRevealClip;

    [SerializeField] Button[] cardButtons;
    [SerializeField] CardSet[] cardSets;
    [SerializeField] Card[] allCards;

    public static CardsManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
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
            exclamationBubble.SetActive(true);
        }
    }

    void SetCardSets()
    {
        Material blackWhiteMat = Resources.Load("Materials/B&W mat", typeof(Material)) as Material;
        for (int i = 0; i < cardButtons.Length; i++)
        {
            string cardSetId = cardSets[i].id;
            if (GameData.gameData.saveData.cardIds.Contains(cardSetId))
            {
                int index = i;
                cardButtons[i].onClick.AddListener(delegate () { OnCardPressed(index); });
            }
            else
            {
                cardButtons[i].interactable = false;
                cardButtons[i].GetComponent<Image>().material = blackWhiteMat;

                int rankToUnlock = RewardForLevel.Instance.GetRankFromRewards(LevelReward.CardSet, cardSetId);
                Text text = cardButtons[i].transform.GetChild(0).GetComponent<Text>();
                text.text = "RANK\n" + rankToUnlock;
                text.gameObject.SetActive(true);
            }
        }
    }

    void OnCardPressed(int index)
    {
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
        Card[] cardsInSet = cardSets[cardSetIndex].CardsInSet;
        int randIndex = Random.Range(0, cardsInSet.Length);
        DisplayCardInfo(cardsInSet[randIndex]);
        GameData.gameData.UpdateCardClaim(System.DateTime.Now.AddHours(12), cardsInSet[randIndex].CardType);
        cardButtons[cardSetIndex].GetComponent<SpriteChanger>().SetSprite(cardsInSet[randIndex].Sprite);
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

    Card GetCardInArray(string cardType)
    {
        for (int i = 0; i < allCards.Length; i++)
        {
            if(allCards[i].CardType.CompareTo(cardType) == 0)
            {
                return allCards[i];
            }
        }
        return null;
    }
}
