using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Switcheroo : MonoBehaviour, IConcreteBonus
{
    [SerializeField] float timeForBonusReload = 3f;
    [SerializeField] private int cost = 30;

    private string boostInfo = "Switcheroo";
    private string description = "Switches all blocks off type to another type.";
    private int boostLevel = 1;

    List<string> tagsToSwitch = new List<string>(); 
    private int spriteIndex = 0;
    GridA grid;
    AnimationClip clip;

    public void ExecuteBonus()
    {
        if (clip == null)
            clip = Resources.Load<AnimationClip>("Sprites/BoostSprites/Switcheroo/Switch Anim");
        string tag;
        do
        {
            tag = grid.boxPrefabs[Random.Range(0, grid.boxPrefabs.Length)].tag;
        } while (tagsToSwitch.Contains(tag));
        tagsToSwitch.Add(tag);
        string finalTag;

        do
        {
            finalTag = grid.boxPrefabs[Random.Range(0, grid.boxPrefabs.Length)].tag;
        } while (tagsToSwitch.Contains(finalTag));

        clip.AddEvent(new AnimationEvent()
        {
            time = 0.5f,
            functionName = "ChangeBlockSprite",
            stringParameter = finalTag
        });

        SwitchBlocksType(tagsToSwitch);
        StartCoroutine(CheckForMatchesDelayed());
    }

    private void SwitchBlocksType(List<string> tags)
    {
        for (int i = 0; i < grid.width; i++)
        {
            for (int j = 0; j < grid.hight; j++)
            {
                if (grid.allBoxes[i, j] != null && tags.Contains(grid.allBoxes[i, j].tag))
                {
                    float speed = Random.Range(0.8f, 1.2f);
                    if (grid.allBoxes[i, j].GetComponent<Animation>() == null)
                        grid.allBoxes[i, j].AddComponent<Animation>();
                    grid.allBoxes[i, j].GetComponent<Animation>().AddClip(clip, "");
                    grid.allBoxes[i, j].GetComponent<Animation>()[""].speed = speed;
                    grid.allBoxes[i, j].GetComponent<Animation>().Play("");
                }
            }
        }
    }

    IEnumerator CheckForMatchesDelayed()
    {
        yield return new WaitForSeconds(1f);
        FindObjectOfType<MatchFinder>().FindAllMatches();
        grid.DestroyAllMatches();
    }

    public Sprite GetSprite()
    {
        return GetComponent<SpriteRenderer>().sprite;
    }
    public float TimeToReload()
    {
        return timeForBonusReload;
    }
    public string GetBoostTitle()
    {
        return boostInfo;
    }
    public int GetBoostLevel()
    {
        return boostLevel;
    }
    public string GetBoostDescription()
    {
        return description;
    }
    public Sprite GetSpriteFromImage()
    {
        return GetComponent<Image>().sprite;
    }
    public int GetBoostLevelUpCost()
    {
        return cost;
    }
    public void LevelUpBoost()
    {
        boostLevel++;
    }
    public int GetSpiteIndex()
    {
        return spriteIndex;
    }
    public void SetBoostLevel(int lvl)
    {
        grid = FindObjectOfType<GridA>();
        boostLevel = lvl;
        switch (boostLevel)
        {
            case 1:
                {
                    timeForBonusReload = 5;
                    break;
                }
            case 2:
                {
                    timeForBonusReload = 50;
                    break;
                }
            case 3:
                {
                    timeForBonusReload = 45;
                    break;
                }
            case 4:
                {
                    timeForBonusReload = 40;
                    spriteIndex = 1;
                    break;
                }
            case 5:
                {
                    timeForBonusReload = 38;
                    spriteIndex = 1;
                    break;
                }
            case 6:
                {
                    timeForBonusReload = 36;
                    spriteIndex = 1;
                    break;
                }
            case 7:
                {
                    if (grid != null)
                    {
                        string tag;
                        do
                        {
                            tag = grid.boxPrefabs[Random.Range(0, grid.boxPrefabs.Length)].tag;
                        } while (tagsToSwitch.Contains(tag));
                        tagsToSwitch.Add(tag);
                    }
                    timeForBonusReload = 34;
                    spriteIndex = 2;
                    break;
                }
            case 8:
                {
                    if (grid != null)
                    {
                        string tag;
                        do
                        {
                            tag = grid.boxPrefabs[Random.Range(0, grid.boxPrefabs.Length)].tag;
                        } while (tagsToSwitch.Contains(tag));
                        tagsToSwitch.Add(tag);
                    }
                    timeForBonusReload = 32;
                    spriteIndex = 2;
                    break;
                }
            case 9:
                {
                    if (grid != null)
                    {
                        string tag;
                        do
                        {
                            tag = grid.boxPrefabs[Random.Range(0, grid.boxPrefabs.Length)].tag;
                        } while (tagsToSwitch.Contains(tag));
                        tagsToSwitch.Add(tag);
                    }
                    timeForBonusReload = 30;
                    spriteIndex = 2;
                    break;
                }
            case 10:
                {
                    if (grid != null)
                    {
                        string tag;
                        do
                        {
                            tag = grid.boxPrefabs[Random.Range(0, grid.boxPrefabs.Length)].tag;
                        } while (tagsToSwitch.Contains(tag));
                        tagsToSwitch.Add(tag);
                        do
                        {
                            tag = grid.boxPrefabs[Random.Range(0, grid.boxPrefabs.Length)].tag;
                        } while (tagsToSwitch.Contains(tag));
                        tagsToSwitch.Add(tag);
                    }
                    timeForBonusReload = 25;
                    spriteIndex = 3;
                    break;
                }
            default:
                timeForBonusReload = 60;
                break;
        }
    }
}
