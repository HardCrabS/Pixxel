using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlameThrower : MonoBehaviour, IConcreteBonus
{
    [SerializeField] float lineSpeed = 5;
    [SerializeField] float timeForBonusReload = 3f;
    [SerializeField] private int cost = 30;

    private string boostInfo = "FlameThrower";
    private string description = "Throws a fire line destroying blocks.";
    private int boostLevel = 1;

    LineRenderer line;
    GridA grid;
    Vector2 firstPos;
    Vector2 curr;
    Vector2 target;
    int lineIndexPos = 0;
    private bool boostActivated = false;
    private int linesToDestroy = 1;

    void Update()
    {
        if (boostActivated)
        {
            CreateLine();
        }
    }

    public void ExecuteBonus()
    {
        grid = FindObjectOfType<GridA>();
        grid.currState = GameState.wait;

        line = gameObject.AddComponent<LineRenderer>();
        line.widthMultiplier = 0.5f;

        line.material = new Material(Shader.Find("Sprites/Default"));
        line.SetColors(Color.yellow, Color.red);

        firstPos = new Vector2(10, 0);
        curr = firstPos;
        target = new Vector2(-2, 0);
        lineIndexPos = 0;

        line.startColor = Color.yellow;
        line.SetPosition(lineIndexPos, firstPos);
        boostActivated = true;
    }

    void CreateLine()
    {
        curr = Vector2.MoveTowards(curr, target, lineSpeed * Time.deltaTime);
        if (Mathf.Abs(curr.x - (int)curr.x) <= 0.2 && (int)curr.x >= 0 && (int)curr.x < 8)
            grid.FlameThrower((int)curr.x, (int)curr.y);

        line.SetPosition(lineIndexPos + 1, curr);

        if (curr == target)
        {
            if (firstPos.y == linesToDestroy - 1)
            {
                boostActivated = false;
                Destroy(line);
                StartCoroutine(grid.MoveBoxesDown());
                grid.currState = GameState.move;
            }
            else
            {
                line.positionCount += 2;
                firstPos.y++;
                target = firstPos;
                lineIndexPos += 2;
                curr.y++;
                firstPos = curr;
                line.SetPosition(lineIndexPos, firstPos);
                CreateLine();
            }
        }
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
                    linesToDestroy = 2;
                    timeForBonusReload = 40;
                    break;
                }
            case 5:
                {
                    linesToDestroy = 2;
                    timeForBonusReload = 38;
                    break;
                }
            case 6:
                {
                    linesToDestroy = 2;
                    timeForBonusReload = 36;
                    break;
                }
            case 7:
                {
                    linesToDestroy = 3;
                    timeForBonusReload = 34;
                    break;
                }
            case 8:
                {
                    linesToDestroy = 3;
                    timeForBonusReload = 32;
                    break;
                }
            case 9:
                {
                    linesToDestroy = 3;
                    timeForBonusReload = 30;
                    break;
                }
            case 10:
                {
                    linesToDestroy = 4;
                    timeForBonusReload = 25;
                    break;
                }
            default:
                timeForBonusReload = 60;
                break;
        }
    }
}