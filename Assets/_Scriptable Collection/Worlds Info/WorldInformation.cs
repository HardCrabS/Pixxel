using UnityEngine;

[CreateAssetMenu(menuName = "Collection/World Info")]
public class WorldInformation : RewardTemplate 
{
    [Header("World Specific")]
    [SerializeField] int bombSpawnChance;
    [SerializeField] float musicStartDelay = 0;
    [SerializeField] string worldStyle;
    [SerializeField] string musicTitle;
    [SerializeField] string musicCreator;
    [SerializeField] Sprite backgroundSprite;
    [SerializeField] AudioClip song;
    [SerializeField] GameObject[] boxes;
    [SerializeField] LevelTemplate[] trinketLevelTemplates;
    [SerializeField] LevelTemplate leaderboardTemplate;

    public int BombSpawnChance => bombSpawnChance;
    public float MusicStartDelay { get { return musicStartDelay; } }
    public string Style { get { return worldStyle; } }
    public string MusicTitle { get { return musicTitle; } }
    public string MusicCreator { get { return musicCreator; } }
    public Sprite BackgroundSprite { get { return backgroundSprite; } }
    public AudioClip Song { get { return song; } }
    public GameObject[] Boxes { get { return boxes; } }
    public LevelTemplate[] TrinketLevelTemplates { get { return trinketLevelTemplates; } }
    public LevelTemplate LeaderboardLevelTemplate { get { return leaderboardTemplate; } }

    public override Sprite GetRewardSprite()
    {
        return backgroundSprite;
    }
}