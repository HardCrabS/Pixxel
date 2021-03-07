using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GoldRush : BoostBase
{
    private int timeToBonusLast = 5;
    private float startTime;
    private Sprite goldenRockSprite;
    private GameObject particleCoin;
    private GameObject goldRushPanel;
    private Text timeText;
    GridA grid;
    AudioClip goldStart, turnGold, goldTapped;

    IEnumerator StartGoldRushTimer()
    {
        while (startTime > 0)
        {
            timeText.text = string.Format("{0:0.00}", startTime);
            startTime -= Time.deltaTime;

            yield return null;
        }
        timeText.text = "0.00";
        grid.onGoldRushMatch -= ChangeSpriteOnMatch;
        finished = true;
    }

    public override void ExecuteBonus()
    {
        if (grid == null)
        {
            grid = GridA.Instance;
            goldenRockSprite = Resources.Load<Sprite>(RECOURSES_FOLDER + "Gold Rush/GoldenRock");
            particleCoin = Resources.Load<GameObject>(RECOURSES_FOLDER + "Gold Rush/Coins Particle");
            goldRushPanel = Resources.Load<GameObject>(RECOURSES_FOLDER + "Gold Rush/Gold Rush Panel");

            goldStart = Resources.Load<AudioClip>(RECOURSES_FOLDER + "Gold Rush/sfx_boost_alert");
            turnGold = Resources.Load<AudioClip>(RECOURSES_FOLDER + "Gold Rush/sfx_boost_goldr1");
            goldTapped = Resources.Load<AudioClip>(RECOURSES_FOLDER + "Gold Rush/sfx_boost_goldr2");
        }
        startTime = timeToBonusLast;
        grid.onGoldRushMatch += ChangeSpriteOnMatch;

        GameObject canvas = GameObject.FindGameObjectWithTag("Main Canvas");
        RectTransform bonusPanelRectTransform = goldRushPanel.GetComponent<RectTransform>();

        Vector2 spawnPos = new Vector2(0, -bonusPanelRectTransform.rect.height / 2);
        GameObject panel = Instantiate(goldRushPanel, spawnPos, transform.rotation, canvas.transform);
        panel.GetComponent<RectTransform>().anchoredPosition = spawnPos;
        Destroy(panel, timeToBonusLast + 1f);
        timeText = panel.transform.GetChild(0).GetComponent<Text>();

        StartCoroutine(StartGoldRushTimer());
        audioSource.PlayOneShot(goldStart);
    }
    public void RockTapped()
    {
        audioSource.PlayOneShot(goldTapped);
    }
    void ChangeSpriteOnMatch(int column, int row)
    {
        audioSource.PlayOneShot(turnGold);
        grid.allBoxes[column, row].GetComponent<SpriteRenderer>().sprite = goldenRockSprite;
        grid.allBoxes[column, row].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        var goldenRock = grid.allBoxes[column, row].AddComponent<GoldenRock>();
        goldenRock.paticleCoin = particleCoin;
        goldenRock.SetValues(column, row);
        goldenRock.goldRush = this;
        Destroy(grid.allBoxes[column, row].GetComponent<Box>());
        grid.allBoxes[column, row] = null;
        grid.SetBlankSpace(column, row, true);
    }
    public override void SetBoostLevel(int lvl)
    {
        base.SetBoostLevel(lvl);

        if (lvl <= 3)
        {
            timeToBonusLast = 5;
        }
        else if (lvl <= 6)
        {
            timeToBonusLast = 8;
        }
        else if (lvl <= 9)
        {
            timeToBonusLast = 10;
        }
        else
        {
            timeToBonusLast = 13;
        }
    }
}