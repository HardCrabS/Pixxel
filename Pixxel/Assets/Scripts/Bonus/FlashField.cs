using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashField : MonoBehaviour, IConcreteBonus
{
    [SerializeField] float timeForBonusReload = 3f;
    [SerializeField] private int cost = 30;

    private string boostInfo = "Flash Field";
    private string description = "Turns a number of blocks on-screen into 'Fired-Up' blocks.";
    private int boostLevel = 1;

    private int spriteIndex = 0;
    private int blockToMakeFiredUp = 5;
    GameObject lightning;
    float lightningTime;
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
        boostLevel = lvl;
        grid = FindObjectOfType<GridA>();
        switch (boostLevel)
        {
            case 1:
                {
                    if (grid != null)
                    {
                        blockToMakeFiredUp = (int)(grid.width * grid.hight * 0.1f);
                    }
                    timeForBonusReload = 60;
                    break;
                }
            case 2:
                {
                    if (grid != null)
                    {
                        blockToMakeFiredUp = (int)(grid.width * grid.hight * 0.1f);
                    }
                    timeForBonusReload = 50;
                    break;
                }
            case 3:
                {
                    if (grid != null)
                    {
                        blockToMakeFiredUp = (int)(grid.width * grid.hight * 0.1f);
                    }
                    timeForBonusReload = 45;
                    break;
                }
            case 4:
                {
                    if (grid != null)
                    {
                        blockToMakeFiredUp = (int)(grid.width * grid.hight * 0.2f);
                    }
                    timeForBonusReload = 40;
                    spriteIndex = 1;
                    break;
                }
            case 5:
                {
                    if (grid != null)
                    {
                        blockToMakeFiredUp = (int)(grid.width * grid.hight * 0.2f);
                    }
                    timeForBonusReload = 38;
                    spriteIndex = 1;
                    break;
                }
            case 6:
                {
                    if (grid != null)
                    {
                        blockToMakeFiredUp = (int)(grid.width * grid.hight * 0.2f);
                    }
                    timeForBonusReload = 36;
                    spriteIndex = 1;
                    break;
                }
            case 7:
                {
                    if (grid != null)
                    {
                        blockToMakeFiredUp = (int)(grid.width * grid.hight * 0.3f);
                    }
                    timeForBonusReload = 34;
                    spriteIndex = 2;
                    break;
                }
            case 8:
                {
                    if (grid != null)
                    {
                        blockToMakeFiredUp = (int)(grid.width * grid.hight * 0.3f);
                    }
                    timeForBonusReload = 32;
                    spriteIndex = 2;
                    break;
                }
            case 9:
                {
                    if (grid != null)
                    {
                        blockToMakeFiredUp = (int)(grid.width * grid.hight * 0.3f);
                    }
                    timeForBonusReload = 30;
                    spriteIndex = 2;
                    break;
                }
            case 10:
                {
                    if (grid != null)
                    {
                        blockToMakeFiredUp = (int)(grid.width * grid.hight * 0.4f);
                    }
                    timeForBonusReload = 25;
                    spriteIndex = 3;
                    break;
                }
            default:
                timeForBonusReload = 5;
                break;
        }
    }
}
