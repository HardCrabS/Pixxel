using UnityEngine;

[CreateAssetMenu(menuName = "Reward/CardSet")]
public class CardSet : RewardTemplate
{
    [SerializeField] Sprite cardSetSprite;
    [SerializeField] Card[] cardsInSet;

    public Card[] CardsInSet => cardsInSet;

    public override Sprite GetRewardSprite()
    {
        return cardSetSprite;
    }
}