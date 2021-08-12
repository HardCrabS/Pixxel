using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Collection/World Info")]
public class WorldInformation : RewardTemplate 
{
    [Header("World Specific")]
    [SerializeField] string blocksName;
    [SerializeField] int bombSpawnChance;
    [SerializeField] float musicStartDelay = 0;
    [SerializeField] GameObject[] boxes;
    public AssetReference worldLoadInfoRef;

    [Header("World Select Info")]
    [SerializeField] string worldStyle;
    [SerializeField] string musicTitle;
    [SerializeField] string musicCreator;
    [SerializeField] Sprite backgroundSprite;

    public string BlocksName => blocksName;
    public int BombSpawnChance => bombSpawnChance;
    public float MusicStartDelay { get { return musicStartDelay; } }
    public string Style { get { return worldStyle; } }
    public string MusicTitle { get { return musicTitle; } }
    public string MusicCreator { get { return musicCreator; } }
    public Sprite BackgroundSprite { get { return backgroundSprite; } }
    public GameObject[] Boxes { get { return boxes; } }

    public override Sprite GetRewardSprite()
    {
        return backgroundSprite;
    }
}