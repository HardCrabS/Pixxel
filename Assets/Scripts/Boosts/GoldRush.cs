using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GoldRush : BoostBase
{
    private int timeToBonusLast = 5;
    private float startTime;
    private GameObject goldRushPanel;
    private Text timeText;
    GridA grid;
    AudioClip goldStart, turnGold;

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
        LivesManager.Instance.BombCounterState = BombCounterState.ticking;//resume bombs counter
        finished = true;
    }

    public override void ExecuteBonus()
    {
        base.ExecuteBonus();
        GetResources();

        LivesManager.Instance.BombCounterState = BombCounterState.waiting;//stop bombs counters
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

    private void GetResources()
    {
        if (grid == null)
        {
            grid = GridA.Instance;
            goldRushPanel = Resources.Load<GameObject>(RESOURCES_FOLDER + "Gold Rush/Gold Rush Panel");

            goldStart = Resources.Load<AudioClip>(RESOURCES_FOLDER + "Gold Rush/sfx_boost_alert");
            turnGold = Resources.Load<AudioClip>(RESOURCES_FOLDER + "Gold Rush/sfx_boost_goldr1");
        }
    }

    void ChangeSpriteOnMatch(int row, int column)
    {
        audioSource.PlayOneShot(turnGold);
        grid.allBoxes[row, column].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        grid.SetBlockGoldenRock(grid.allBoxes[row, column].GetComponent<Box>());
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
            timeToBonusLast = 6;
        }
        else if (lvl <= 9)
        {
            timeToBonusLast = 7;
        }
        else
        {
            timeToBonusLast = 8;
        }
    }
}