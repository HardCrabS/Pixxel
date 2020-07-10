using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashField : MonoBehaviour, IConcreteBonus
{
    [SerializeField] string uniqueAbility;
    private int boostLevel = 1;

    private int spriteIndex = 0;
    private int blockToMakeFiredUp = 5;
    GameObject lightning;
    private GridA grid;

    public void ExecuteBonus()
    {
        if (lightning == null)
        {
            lightning = Resources.Load<GameObject>("Sprites/BoostSprites/Flash Field/Lightning");
        }
        StartCoroutine(MakeAllFiredUp());
    }

    private IEnumerator MakeAllFiredUp()
    {
        for (int i = 0; i < blockToMakeFiredUp; i++)
        {
            int randX = Random.Range(0, grid.width);
            int randY = Random.Range(0, grid.hight);
            if (grid.allBoxes[randX, randY] != null)
                MakeBlockFiredUp(grid.allBoxes[randX, randY].GetComponent<Box>(), new Vector2(randX, randY));
            yield return new WaitForSeconds(1f);
        }
    }

    private void MakeBlockFiredUp(Box box, Vector2 pos)
    {
        GameObject go = Instantiate(lightning, pos, transform.rotation);
        StartCoroutine(Camera.main.GetComponent<CameraShake>().Shake(0.1f, 0.2f));
        Destroy(go, 0.4f);
        StartCoroutine(grid.FiredUpBlock(box));
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
        int currBlocksFiredUp, nextBlocksFiredUp;
        switch (level)
        {
            case 1:
                {
                    currBlocksFiredUp = 5;
                    nextBlocksFiredUp = 5;
                    break;
                }
            case 2:
                {
                    currBlocksFiredUp = 5;
                    nextBlocksFiredUp = 5;
                    break;
                }
            case 3:
                {
                    currBlocksFiredUp = 5;
                    nextBlocksFiredUp = 10;
                    break;
                }
            case 4:
                {
                    currBlocksFiredUp = 10;
                    nextBlocksFiredUp = 10;
                    break;
                }
            case 5:
                {
                    currBlocksFiredUp = 10;
                    nextBlocksFiredUp = 10;
                    break;
                }
            case 6:
                {
                    currBlocksFiredUp = 10;
                    nextBlocksFiredUp = 15;
                    break;
                }
            case 7:
                {
                    currBlocksFiredUp = 15;
                    nextBlocksFiredUp = 15;
                    break;
                }
            case 8:
                {
                    currBlocksFiredUp = 15;
                    nextBlocksFiredUp = 15;
                    break;
                }
            case 9:
                {
                    currBlocksFiredUp = 15;
                    nextBlocksFiredUp = 20;
                    break;
                }
            case 10:
                {
                    currBlocksFiredUp = 20;
                    nextBlocksFiredUp = -1;
                    break;
                }
            default:
                {
                    currBlocksFiredUp = -1;
                    nextBlocksFiredUp = -1;
                    break;
                }
        }
        return uniqueAbility + "<color=red>" + currBlocksFiredUp + "%</color>" + "|" + uniqueAbility + "<color=red>" + nextBlocksFiredUp + "%</color>";
    }
    public void SetBoostLevel(int lvl)
    {
        boostLevel = lvl;
        grid = GridA.Instance;
        switch (boostLevel)
        {
            case 1:
                {
                    if (grid != null)
                    {
                        blockToMakeFiredUp = (int)(grid.width * grid.hight * 0.05f);
                    }
                    break;
                }
            case 2:
                {
                    if (grid != null)
                    {
                        blockToMakeFiredUp = (int)(grid.width * grid.hight * 0.05f);
                    }
                    break;
                }
            case 3:
                {
                    if (grid != null)
                    {
                        blockToMakeFiredUp = (int)(grid.width * grid.hight * 0.05f);
                    }
                    break;
                }
            case 4:
                {
                    if (grid != null)
                    {
                        blockToMakeFiredUp = (int)(grid.width * grid.hight * 0.1f);
                    }
                    spriteIndex = 1;
                    break;
                }
            case 5:
                {
                    if (grid != null)
                    {
                        blockToMakeFiredUp = (int)(grid.width * grid.hight * 0.1f);
                    }
                    spriteIndex = 1;
                    break;
                }
            case 6:
                {
                    if (grid != null)
                    {
                        blockToMakeFiredUp = (int)(grid.width * grid.hight * 0.1f);
                    }
                    spriteIndex = 1;
                    break;
                }
            case 7:
                {
                    if (grid != null)
                    {
                        blockToMakeFiredUp = (int)(grid.width * grid.hight * 0.15f);
                    }
                    spriteIndex = 2;
                    break;
                }
            case 8:
                {
                    if (grid != null)
                    {
                        blockToMakeFiredUp = (int)(grid.width * grid.hight * 0.15f);
                    }
                    spriteIndex = 2;
                    break;
                }
            case 9:
                {
                    if (grid != null)
                    {
                        blockToMakeFiredUp = (int)(grid.width * grid.hight * 0.15f);
                    }
                    spriteIndex = 2;
                    break;
                }
            case 10:
                {
                    if (grid != null)
                    {
                        blockToMakeFiredUp = (int)(grid.width * grid.hight * 0.2f);
                    }
                    spriteIndex = 3;
                    break;
                }
        }
    }
}
