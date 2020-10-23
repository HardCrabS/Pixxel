using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Rewards/World Info")]
public class WorldInformation : RewardTemplate 
{
    [Header("World Specific")]
    [SerializeField] WorldName worldName;
    [TextArea(2, 4)] [SerializeField] string description;
    [SerializeField] string worldStyle;
    [SerializeField] string musicTitle;
    [SerializeField] string musicCreator;
    [SerializeField] Sprite backgroundSprite;
    [SerializeField] AudioClip song;
    [SerializeField] Sprite[] blockSprites;
    [SerializeField] GameObject[] boxes;
    [SerializeField] LevelTemplate[] trinketLevelTemplates;
    [SerializeField] LevelTemplate leaderboardTemplate;

    public string WorldName { get { return SplitCamelCase(worldName.ToString()); } }
    public string Description { get { return description; } }
    public string Style { get { return worldStyle; } }
    public string MusicTitle { get { return musicTitle; } }
    public string MusicCreator { get { return musicCreator; } }
    public Sprite BackgroundSprite { get { return backgroundSprite; } }
    public AudioClip Song { get { return song; } }
    public Sprite[] BlockSprites { get { return blockSprites; } }
    public GameObject[] Boxes { get { return boxes; } }
    public LevelTemplate[] TrinketLevelTemplates { get { return trinketLevelTemplates; } }
    public LevelTemplate LeaderboardLevelTemplate { get { return leaderboardTemplate; } }

    public override Sprite GetRewardSprite()
    {
        return backgroundSprite;
    }
    public override string GetRewardId()
    {
        return worldName.ToString();
    }
}

public enum WorldName
{
    TwilightCity,
    OakyPlatformer,
    Skyscraper,
    Mountains,
}