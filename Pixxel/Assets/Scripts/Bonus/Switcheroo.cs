using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Switcheroo : MonoBehaviour, IConcreteBonus
{
    [SerializeField] string uniqueAbility;
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
    public int GetBoostLevel()
    {
        return boostLevel;
    }
    public Sprite GetSpriteFromImage()
    {
        return GetComponent<Image>().sprite;
    }
    public void LevelUpBoost()
    {
        boostLevel++;
    }
    public int GetSpiteIndex()
    {
        return spriteIndex;
    }
    public string GetUniqueAbility(int level)
    {
        int currBlocksToSwitch, nextBlocksToSwitch;
        switch (level)
        {
            case 1:
                {
                    currBlocksToSwitch = 1;
                    nextBlocksToSwitch = 1;
                    break;
                }
            case 2:
                {
                    currBlocksToSwitch = 1;
                    nextBlocksToSwitch = 1;
                    break;
                }
            case 3:
                {
                    currBlocksToSwitch = 1;
                    nextBlocksToSwitch = 2;
                    break;
                }
            case 4:
                {
                    currBlocksToSwitch = 2;
                    nextBlocksToSwitch = 2;
                    break;
                }
            case 5:
                {
                    currBlocksToSwitch = 2;
                    nextBlocksToSwitch = 2;
                    break;
                }
            case 6:
                {
                    currBlocksToSwitch = 2;
                    nextBlocksToSwitch = 3;
                    break;
                }
            case 7:
                {
                    currBlocksToSwitch = 3;
                    nextBlocksToSwitch = 3;
                    break;
                }
            case 8:
                {
                    currBlocksToSwitch = 3;
                    nextBlocksToSwitch = 3;
                    break;
                }
            case 9:
                {
                    currBlocksToSwitch = 3;
                    nextBlocksToSwitch = 4;
                    break;
                }
            case 10:
                {
                    currBlocksToSwitch = 4;
                    nextBlocksToSwitch = -1;
                    break;
                }
            default:
                {
                    currBlocksToSwitch = -1;
                    nextBlocksToSwitch = -1;
                    break;
                }
        }
        return uniqueAbility + "<color=red>" + currBlocksToSwitch + "</color>" + "|" + uniqueAbility + "<color=red>" + nextBlocksToSwitch + "</color>";
    }
    public void SetBoostLevel(int lvl)
    {
        grid = FindObjectOfType<GridA>();
        boostLevel = lvl;
        switch (boostLevel)
        {
            case 4:
                {
                    spriteIndex = 1;
                    break;
                }
            case 5:
                {
                    spriteIndex = 1;
                    break;
                }
            case 6:
                {
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
                    spriteIndex = 3;
                    break;
                }
        }
    }
}
