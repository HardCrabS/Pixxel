using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldRush : MonoBehaviour, IConcreteBonus
{
    [SerializeField] string uniqueAbility;
    private int boostLevel = 1;

    private int timeToBonusLast = 5;
    private int spriteIndex = 0;
    private bool activated = false;
    private float startTime;
    private Sprite goldenRockSprite;
    private GameObject particleCoin;
    private GameObject goldRushPanel;
    private Text timeText;
    GridA grid;

    void Update()
    {
        if (activated)
        {
            if (startTime > 0)
            {
                timeText.text = string.Format("{0:0.00}", startTime);
                startTime -= Time.deltaTime;
            }
            else
            {
                timeText.text = "0.00";
                activated = false;
                grid.onMatchedBlock -= ChangeSpriteOnMatch;
            }
        }
    }

    public void ExecuteBonus()
    {
        if (grid == null)
            grid = GridA.Instance;
        if (goldenRockSprite == null)
            goldenRockSprite = Resources.Load<Sprite>("Sprites/BoostSprites/Gold Rush/GoldenRock");
        if (particleCoin == null)
            particleCoin = Resources.Load<GameObject>("Sprites/BoostSprites/Gold Rush/Coins Particle");
        if (goldRushPanel == null)
            goldRushPanel = Resources.Load<GameObject>("Sprites/BoostSprites/Gold Rush/Gold Rush Panel");
        activated = true;
        startTime = timeToBonusLast;
        grid.onMatchedBlock += ChangeSpriteOnMatch;

        GameObject canvas = GameObject.FindGameObjectWithTag("Main Canvas");
        RectTransform bonusPanelRectTransform = goldRushPanel.GetComponent<RectTransform>();

        Vector2 spawnPos = new Vector2(0, -bonusPanelRectTransform.rect.height / 2);
        GameObject panel = Instantiate(goldRushPanel, spawnPos, transform.rotation, canvas.transform);
        panel.GetComponent<RectTransform>().anchoredPosition = spawnPos;
        Destroy(panel, timeToBonusLast + 1f);
        timeText = panel.transform.GetChild(0).GetComponent<Text>();
    }

    void ChangeSpriteOnMatch(int column, int row)
    {
        grid.allBoxes[column, row].GetComponent<SpriteRenderer>().sprite = goldenRockSprite;
        grid.allBoxes[column, row].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        grid.allBoxes[column, row].AddComponent<GoldenRock>().paticleCoin = particleCoin;
        grid.allBoxes[column, row].GetComponent<GoldenRock>().SetValues(column, row);
        Destroy(grid.allBoxes[column, row].GetComponent<Box>());
        grid.allBoxes[column, row] = null;
        grid.SetBlankSpace(column, row, true);
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
        int currBoostTime, nextBoostTime;
        switch (level)
        {
            case 1:
                {
                    currBoostTime = 5;
                    nextBoostTime = 5;
                    break;
                }
            case 2:
                {
                    currBoostTime = 5;
                    nextBoostTime = 5;
                    break;
                }
            case 3:
                {
                    currBoostTime = 5;
                    nextBoostTime = 7;
                    break;
                }
            case 4:
                {
                    currBoostTime = 7;
                    nextBoostTime = 7;
                    break;
                }
            case 5:
                {
                    currBoostTime = 7;
                    nextBoostTime = 7;
                    break;
                }
            case 6:
                {
                    currBoostTime = 7;
                    nextBoostTime = 10;
                    break;
                }
            case 7:
                {
                    currBoostTime = 10;
                    nextBoostTime = 10;
                    break;
                }
            case 8:
                {
                    currBoostTime = 10;
                    nextBoostTime = 10;
                    break;
                }
            case 9:
                {
                    currBoostTime = 10;
                    nextBoostTime = 12;
                    break;
                }
            case 10:
                {
                    currBoostTime = 12;
                    nextBoostTime = -1;
                    break;
                }
            default:
                {
                    currBoostTime = -1;
                    nextBoostTime = -1;
                    break;
                }
        }
        return uniqueAbility + "<color=red>" + currBoostTime + "</color>" + "|" + uniqueAbility + "<color=red>" + nextBoostTime + "</color>";
    }
    public void SetBoostLevel(int lvl)
    {
        boostLevel = lvl;
        switch (boostLevel)
        {
            case 4:
                {
                    timeToBonusLast = 8;
                    spriteIndex = 1;
                    break;
                }
            case 5:
                {
                    timeToBonusLast = 8;
                    spriteIndex = 1;
                    break;
                }
            case 6:
                {
                    timeToBonusLast = 8;
                    spriteIndex = 1;
                    break;
                }
            case 7:
                {
                    timeToBonusLast = 10;
                    spriteIndex = 2;
                    break;
                }
            case 8:
                {
                    timeToBonusLast = 10;
                    spriteIndex = 2;
                    break;
                }
            case 9:
                {
                    timeToBonusLast = 10;
                    spriteIndex = 2;
                    break;
                }
            case 10:
                {
                    timeToBonusLast = 13;
                    spriteIndex = 3;
                    break;
                }
        }
    }
}
