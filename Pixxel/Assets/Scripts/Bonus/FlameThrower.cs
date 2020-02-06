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
    GameObject leadFirePrefab;
    GameObject shineParticles;
    GridA grid;
    Vector3 firstPos;
    Vector3 curr;
    Vector3 target;
    int lineIndexPos = 0;
    private bool boostActivated = false;
    private int linesToDestroy = 1;
    private int spriteIndex = 0;

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

        line.material = Resources.Load<Material>("Materials/Fire Line");
        //line.SetColors(Color.yellow, Color.red);

        firstPos = new Vector3(10, 0, -5);
        curr = firstPos;
        target = new Vector3(-2, 0, -5);
        lineIndexPos = 0;

        line.SetPosition(lineIndexPos, firstPos);
        leadFirePrefab = Resources.Load<GameObject>("Sprites/BoostSprites/Flamethrower/Lead Fire");
        shineParticles = Resources.Load<GameObject>("Sprites/BoostSprites/Flamethrower/Shine Effect");
        leadFirePrefab = Instantiate(leadFirePrefab, new Vector3(firstPos.x, firstPos.y+0.5f, firstPos.z), Quaternion.Euler(0, 0, -90));
        shineParticles = Instantiate(shineParticles, new Vector3(firstPos.x, firstPos.y + 0.5f, firstPos.z), Quaternion.identity);
        boostActivated = true;
    }

    void CreateLine()
    {
        leadFirePrefab.transform.position = Vector3.MoveTowards(curr, target, lineSpeed * Time.deltaTime);
        shineParticles.transform.position = Vector3.MoveTowards(shineParticles.transform.position, target, lineSpeed * Time.deltaTime);
        curr = Vector3.MoveTowards(curr, target, lineSpeed * Time.deltaTime);
        if (Mathf.Abs(curr.x - (int)curr.x) <= 0.2 && (int)curr.x >= 0 && (int)curr.x < 8)
            grid.FlameThrower((int)curr.x, (int)curr.y);

        line.SetPosition(lineIndexPos + 1, curr);

        if (curr == target)
        {
            if (firstPos.y == linesToDestroy - 1)
            {
                boostActivated = false;
                Destroy(line);
                Destroy(leadFirePrefab);
                Destroy(shineParticles);
                StartCoroutine(grid.MoveBoxesDown());
                grid.currState = GameState.move;
            }
            else
            {
                line.positionCount += 2;
                firstPos.y++;
                leadFirePrefab.transform.position = new Vector3(leadFirePrefab.transform.position.x, leadFirePrefab.transform.position.y + 1f, firstPos.z);
                shineParticles.transform.position = new Vector3(shineParticles.transform.position.x, shineParticles.transform.position.y + 1, firstPos.z);
                leadFirePrefab.transform.rotation = Quaternion.Euler(0, leadFirePrefab.transform.rotation.eulerAngles.y + 180, -90);
                shineParticles.transform.rotation = Quaternion.Euler(0, shineParticles.transform.rotation.eulerAngles.y + 180, 0);
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
                    linesToDestroy = 2;
                    timeForBonusReload = 40;
                    spriteIndex = 1;
                    break;
                }
            case 5:
                {
                    linesToDestroy = 2;
                    timeForBonusReload = 38;
                    spriteIndex = 1;
                    break;
                }
            case 6:
                {
                    linesToDestroy = 2;
                    timeForBonusReload = 36;
                    spriteIndex = 1;
                    break;
                }
            case 7:
                {
                    linesToDestroy = 3;
                    timeForBonusReload = 34;
                    spriteIndex = 2;
                    break;
                }
            case 8:
                {
                    linesToDestroy = 3;
                    timeForBonusReload = 32;
                    spriteIndex = 2;
                    break;
                }
            case 9:
                {
                    linesToDestroy = 3;
                    timeForBonusReload = 30;
                    spriteIndex = 2;
                    break;
                }
            case 10:
                {
                    linesToDestroy = 4;
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