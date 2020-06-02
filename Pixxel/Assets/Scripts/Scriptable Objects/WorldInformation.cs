using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "World Info")]
public class WorldInformation : ScriptableObject 
{
    [SerializeField] string worldName;
    [SerializeField] int worldIndex;
    [SerializeField] Sprite backgroundSprite;
    [SerializeField] Sprite[] blockSprites;
    [SerializeField] GameObject[] boxes;
    [SerializeField] LevelTemplate[] trinketLevelTemplates;
    [SerializeField] LevelTemplate leaderboardTemplate;

    public string WorldName { get { return worldName; } }
    public Sprite BackgroundSprite { get { return backgroundSprite; } }
    public Sprite[] BlockSprites { get { return blockSprites; } }
    public GameObject[] Boxes { get { return boxes; } }
    public LevelTemplate[] TrinketLevelTemplates { get { return trinketLevelTemplates; } }
    public LevelTemplate LeaderboardLevelTemplate { get { return leaderboardTemplate; } }
    public int WorldIndex { get { return worldIndex; } }
}
