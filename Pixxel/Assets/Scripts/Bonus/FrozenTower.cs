using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrozenTower : MonoBehaviour, IConcreteBonus
{
    [SerializeField] float timeForBonusReload = 3f;
    [SerializeField] private int cost = 30;

    private string boostInfo = "Frozen Tower";
    private string description = "Columns of blocks are frozen then destroyed.";
    private int boostLevel = 1;

    private int columnsToDestroy = 1;
    private float timeBetwColumnsFreeze = 0.2f;
    private int spriteIndex = 0;
    private Sprite frozenBlock;
    private GameObject freezeParticle;
    GridA grid;
    int[] randColumns;

    public void ExecuteBonus()
    {
        grid = FindObjectOfType<GridA>();
        frozenBlock = Resources.Load<Sprite>("Sprites/BoostSprites/Frozen Tower/Frozen Block");
        freezeParticle = Resources.Load<GameObject>("Sprites/BoostSprites/Frozen Tower/Freeze Particle");

        randColumns = new int[columnsToDestroy];
        StartCoroutine(FreezeAllColumns());

        StartCoroutine(DestroyAllBlockColumns());
    }

    IEnumerator FreezeAllColumns()
    {
        for (int j = 0; j < columnsToDestroy; j++)
        {
            do
            {
                randColumns[j] = Random.Range(0, grid.width);
            }
            while (IsTheSame(randColumns, randColumns[j], j));

            GameObject go = Instantiate(freezeParticle, new Vector2(randColumns[j], 0), Quaternion.EulerAngles(-90, 0, 0));
            Destroy(go, 10);
            StartCoroutine(FreezeBlockColumn(randColumns[j]));
            yield return new WaitForSeconds(timeBetwColumnsFreeze);
        }
    }

    IEnumerator FreezeBlockColumn(int column)
    {
        for (int i = 0; i < grid.hight; i++)
        {
            if (grid.allBoxes[column, i] != null)
                grid.allBoxes[column, i].GetComponent<SpriteRenderer>().sprite = frozenBlock;
            yield return new WaitForSeconds(0.4f);
        }
    }

    IEnumerator DestroyAllBlockColumns()
    {
        yield return new WaitForSeconds(timeBetwColumnsFreeze * columnsToDestroy + 8 * 0.4f);
        for (int j = 0; j < columnsToDestroy; j++)
        {
            for (int i = 0; i < grid.hight; i++)
            {
                Destroy(grid.allBoxes[randColumns[j], i]);
            }
        }
        StartCoroutine(grid.MoveBoxesDown());
    }

    bool IsTheSame(int[] arr, int x, int a)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (i == a)
                continue;
            if (arr[i] == x)
                return true;
        }
        return false;
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
        switch (boostLevel)
        {
            case 1:
                {
                    timeForBonusReload = 60;
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
                    columnsToDestroy = 2;
                    timeForBonusReload = 40;
                    spriteIndex = 1;
                    break;
                }
            case 5:
                {
                    columnsToDestroy = 2;
                    timeForBonusReload = 38;
                    spriteIndex = 1;
                    break;
                }
            case 6:
                {
                    columnsToDestroy = 2;
                    timeForBonusReload = 36;
                    spriteIndex = 1;
                    break;
                }
            case 7:
                {
                    columnsToDestroy = 3;
                    timeForBonusReload = 34;
                    spriteIndex = 2;
                    break;
                }
            case 8:
                {
                    columnsToDestroy = 3;
                    timeForBonusReload = 32;
                    spriteIndex = 2;
                    break;
                }
            case 9:
                {
                    columnsToDestroy = 3;
                    timeForBonusReload = 30;
                    spriteIndex = 2;
                    break;
                }
            case 10:
                {
                    columnsToDestroy = 4;
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