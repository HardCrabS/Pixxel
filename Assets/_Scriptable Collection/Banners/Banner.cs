using UnityEngine;

[CreateAssetMenu(menuName = "Collection/Banner")]
public class Banner : RewardTemplate 
{
    [Header("Banner Specific")]
    [SerializeField] Sprite sprite;
    public Sprite Sprite { get { return sprite; } }

    public override Sprite GetRewardSprite()
    {
        return sprite;
    }
}