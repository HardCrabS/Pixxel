using UnityEngine;

[CreateAssetMenu(menuName = "Collection/World Info")]
public class WorldInformation : RewardTemplate 
{
    [Header("World Specific")]
    [SerializeField] string blocksName;
    [SerializeField] int bombSpawnChance;
    [SerializeField] float musicStartDelay = 0;
    [SerializeField] AudioClip song;
    [SerializeField] LevelTemplate template;
    [SerializeField] GameObject[] boxes;
    [SerializeField] int[] scoreWorldLevels = new int[10];
    [SerializeField] LevelTemplate[] trinketTemplates;

    [Header("Game UI")]
    [SerializeField] Material scoreTextMaterial;
    [SerializeField] GameObject backgroundCanvas;
    [SerializeField] Sprite blocksPanelSprite;
    [SerializeField] Color blocksPanelColor;
    [SerializeField] Color blocksPanelGlowColor;
    [SerializeField] Color visualizerColor;
    [SerializeField] Material visualizerMaterial;

    [Header("World Select Info")]
    [SerializeField] string worldStyle;
    [SerializeField] string musicTitle;
    [SerializeField] string musicCreator;
    [SerializeField] Sprite backgroundSprite;

    public string BlocksName => blocksName;
    public int BombSpawnChance => bombSpawnChance;
    public int[] ScoreWorldLevels => scoreWorldLevels;
    public float MusicStartDelay { get { return musicStartDelay; } }
    public string Style { get { return worldStyle; } }
    public string MusicTitle { get { return musicTitle; } }
    public string MusicCreator { get { return musicCreator; } }
    public Sprite BackgroundSprite { get { return backgroundSprite; } }
    public AudioClip Song { get { return song; } }
    public GameObject[] Boxes { get { return boxes; } }
    public LevelTemplate[] TrinketLevelTemplates { get { return trinketTemplates; } }
    public LevelTemplate LeaderboardLevelTemplate { get { return template; } }
    public Material ScoreTextMat { get { return scoreTextMaterial; } }
    public GameObject BackgroundCanvas { get { return backgroundCanvas; } }
    public Sprite BlocksPanelSprite { get { return blocksPanelSprite; } }
    public Color BlocksPanelColor { get { return blocksPanelColor; } }
    public Color BlocksPanelGlowColor { get { return blocksPanelGlowColor; } }
    public Color VisualizerColor { get { return visualizerColor; } }
    public Material VisualizerMaterial { get { return visualizerMaterial; } }

    public override Sprite GetRewardSprite()
    {
        return backgroundSprite;
    }
}