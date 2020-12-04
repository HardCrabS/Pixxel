using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Reward/CardSet")]
public class CardSet : RewardTemplate
{
    [SerializeField] CardSetName cardSetName;
    [SerializeField] Sprite cardSetSprite;
    [SerializeField] Card[] cardsInSet;

    public Card[] CardsInSet => cardsInSet;

    public override string GetRewardId()
    {
        return cardSetName.ToString(); 
    }
    public override Sprite GetRewardSprite()
    {
        return cardSetSprite;
    }
}

public enum CardSetName
{
    InitCardSet,
    FighterCardSet,
    MediumCardSet,
    ProCardSet,
    MasterCardSet
}