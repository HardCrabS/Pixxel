using UnityEngine;

[CreateAssetMenu(menuName = "Collection/Banner")]
public class Banner : RewardTemplate 
{
    [Header("Banner Specific")]
    [SerializeField] Sprite sprite;

    [SerializeField] Material material;
    public MatAnimatorValues animatorValues;

    public Sprite Sprite { get { return sprite; } }
    public Material Material { get { return material; } }

    public override Sprite GetRewardSprite()
    {
        return sprite;
    }
}