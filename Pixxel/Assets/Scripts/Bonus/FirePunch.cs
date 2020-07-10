using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirePunch : MonoBehaviour, IConcreteBonus
{
    [SerializeField] string uniqueAbility;

    float fistSpeed = 6;

    private int boostLevel = 1;
    private int spriteIndex = 0;

    private int timeToBonusLast = 4;
    private int maxClicks = 5;
    private float startTime;
    private GameObject punchPanel;
    private GameObject fistPrefab;
    private GameObject punchParticle;
    private GameObject selectHand;
    private Text timeText;
    GridA grid;
    ScrollBackground scrollBackground;
    CameraShake cameraShake;

    List<Vector2> clickedBlocks;
    private int totalClicksMade = 0;

    delegate void OnFistPunch(Vector2 target);
    OnFistPunch OnPunch;

    void Start()
    {
        clickedBlocks = new List<Vector2>();
    }

    public void ExecuteBonus()
    {
        grid = GridA.Instance;
        if (cameraShake == null)
            Camera.main.GetComponent<CameraShake>();
        grid.currState = GameState.wait;

        scrollBackground = FindObjectOfType<ScrollBackground>();
        if (scrollBackground != null)
        {
            scrollBackground.StopScrolling();
        }
        if (punchPanel == null)
        {
            punchPanel = Resources.Load<GameObject>("Sprites/BoostSprites/Fire Punch/Punch Time Panel");
            fistPrefab = Resources.Load<GameObject>("Sprites/BoostSprites/Fire Punch/Fist");
            punchParticle = Resources.Load<GameObject>("Sprites/BoostSprites/Fire Punch/Punch Particle");
            selectHand = Resources.Load<GameObject>("Sprites/BoostSprites/Fire Punch/Select_Hand");
        }

        startTime = timeToBonusLast;

        SpawnTimePanel();
        StartCoroutine(TimePanel());
        AssignBlockDelegates();
    }

    void SpawnFist(Vector2 blockPos)
    {
        float width = grid.width;
        float height = grid.hight;

        Vector2 spawnPos = Vector2.zero;

        if (blockPos.x > width - blockPos.x) // closer to the right
        {
            int random = Random.Range(0, 100);
            if (random < 33)
            {
                spawnPos = new Vector2(width + 2, blockPos.y + 4);
            }
            else if (random < 66)
            {
                spawnPos = new Vector2(width + 2, blockPos.y + 1);
            }
            else if (random < 100)
            {
                spawnPos = new Vector2(width + 2, blockPos.y - 5);
            }
        }
        else // closer to left
        {
            int random = Random.Range(0, 100);
            if (random < 33)
            {
                spawnPos = new Vector2(-2, blockPos.y + 4);
            }
            else if (random < 66)
            {
                spawnPos = new Vector2(-2, blockPos.y + 1);
            }
            else if (random < 100)
            {
                spawnPos = new Vector2(-2, blockPos.y - 5);
            }
        }

        GameObject fist = Instantiate(fistPrefab, spawnPos, transform.rotation);

        Vector3 diff = blockPos - (Vector2)fist.transform.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        fist.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);

        StartCoroutine(MoveFistToTarget(fist, blockPos));
    }

    IEnumerator MoveFistToTarget(GameObject fist, Vector2 target)
    {
        while (true)
        {
            fist.transform.position = Vector2.MoveTowards(fist.transform.position, target, fistSpeed * Time.deltaTime);

            if (fist.transform.position == (Vector3)target)
            {
                StartCoroutine(Camera.main.GetComponent<CameraShake>().Shake(.1f, .15f));
                OnPunch(target);
                GameObject particle = Instantiate(punchParticle, target, transform.rotation);
                Destroy(particle, 0.8f);
                Destroy(fist);
                yield break;
            }

            yield return null;
        }
    }

    private void DestroyPunchedBlock(Vector2 target)
    {
        if (grid.allBoxes[(int)target.x, (int)target.y] == null)
            return;
        Destroy(grid.allBoxes[(int)target.x, (int)target.y]);
        grid.allBoxes[(int)target.x, (int)target.y] = null;
    }

    private void MakePunchedFiredUp(Vector2 target)
    {
        if (grid.allBoxes[(int)target.x, (int)target.y] != null)
            StartCoroutine(grid.FiredUpBlock(grid.allBoxes[(int)target.x, (int)target.y].GetComponent<Box>()));
    }

    Vector2[] directions = new Vector2[] { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
    private void DestroyAllInCross(Vector2 target)
    {
        if (grid.allBoxes[(int)target.x, (int)target.y] == null)
            return;

        Destroy(grid.allBoxes[(int)target.x, (int)target.y]);
        foreach (Vector2 dir in directions)
        {
            StartCoroutine(DestroyLineOfBlocks(target, dir));
        }
    }

    IEnumerator DestroyLineOfBlocks(Vector2 center, Vector2 dir)
    {
        int x = (int)(center.x + dir.x), y = (int)(center.y + dir.y);
        if (x < 0 || x >= grid.width || y < 0 || y >= grid.hight)
            yield break;

        while (grid.allBoxes[x, y] != null)
        {
            Destroy(grid.allBoxes[x, y]);
            grid.allBoxes[x, y] = null;
            grid.SpawnBlockParticles(new Vector2(x, y));

            x += (int)(dir.x);
            y += (int)(dir.y);
            if (x < 0 || x >= grid.width || y < 0 || y >= grid.hight)
                yield break;

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void SpawnTimePanel()
    {
        GameObject canvas = GameObject.FindGameObjectWithTag("Main Canvas");
        RectTransform bonusPanelRectTransform = punchPanel.GetComponent<RectTransform>();

        Vector2 spawnPos = new Vector2(0, -bonusPanelRectTransform.rect.height / 2);
        GameObject panel = Instantiate(punchPanel, spawnPos, transform.rotation, canvas.transform);
        panel.GetComponent<RectTransform>().anchoredPosition = spawnPos;
        Destroy(panel, timeToBonusLast + 1f);
        timeText = panel.transform.GetChild(0).GetComponent<Text>();
    }

    IEnumerator TimePanel()
    {
        while (true)
        {
            if (startTime > 0)
            {
                timeText.text = string.Format("{0:0.00}", startTime);
                startTime -= Time.deltaTime;
            }
            else
            {
                timeText.text = "0.00";
                if (scrollBackground != null)
                    scrollBackground.ResumeScrolling();

                if (totalClicksMade < maxClicks)
                {
                    for (int i = totalClicksMade; i < maxClicks; i++) // fill unclicked blocks with random
                    {
                        int randX, randY;
                        do
                        {
                            randX = Random.Range(0, grid.width);
                            randY = Random.Range(0, grid.hight);
                        }
                        while (clickedBlocks.Contains(new Vector2(randX, randY)));
                        clickedBlocks.Add(new Vector2(randX, randY));
                    }
                }
                for (int i = 0; i < clickedBlocks.Count; i++) // destroy clicked blocks
                {
                    SpawnFist(clickedBlocks[i]);
                }
                ClearBlockDelegates();
                clickedBlocks.Clear();
                totalClicksMade = 0;

                yield return new WaitForSeconds(2);
                StartCoroutine(grid.MoveBoxesDown());
                yield break;
            }
            yield return null;
        }
    }

    void AssignBlockDelegates()
    {
        for (int i = 0; i < grid.width; i++)
        {
            for (int j = 0; j < grid.hight; j++)
            {
                if (grid.allBoxes[i, j] != null)
                {
                    grid.allBoxes[i, j].GetComponent<Box>().blockClicked = ClickOnBlock;
                }
            }
        }
    }

    void ClearBlockDelegates()
    {
        for (int i = 0; i < grid.width; i++)
        {
            for (int j = 0; j < grid.hight; j++)
            {
                if (grid.allBoxes[i, j] != null)
                {
                    grid.allBoxes[i, j].GetComponent<Box>().blockClicked = null;
                }
            }
        }
    }

    void ClickOnBlock(int x, int y)
    {
        if (totalClicksMade >= maxClicks || clickedBlocks.Contains(new Vector2(x, y)))
        {
            return;
        }

        totalClicksMade++;
        clickedBlocks.Add(new Vector2(x, y));
        grid.allBoxes[x, y].GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f);
        Instantiate(selectHand, new Vector2(x - 0.8f, y + 0.8f), transform.rotation, grid.allBoxes[x, y].transform);
    }

    public Sprite GetSprite()
    {
        return GetComponent<SpriteRenderer>().sprite;
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
        string currAbility, nextAbility;
        switch (level)
        {
            case 1:
                {
                    currAbility = "Destroys 5 chosen blocks";
                    nextAbility = "Destroys 5 chosen blocks";
                    break;
                }
            case 2:
                {
                    currAbility = "Destroys 5 chosen blocks";
                    nextAbility = "Destroys 5 chosen blocks";
                    break;
                }
            case 3:
                {
                    currAbility = "Destroys 5 chosen blocks";
                    nextAbility = "Makes 5 chosen blocks Fired-Up";
                    break;
                }
            case 4:
                {
                    currAbility = "Makes 5 chosen blocks Fired-Up";
                    nextAbility = "Makes 5 chosen blocks Fired-Up";
                    break;
                }
            case 5:
                {
                    currAbility = "Makes 5 chosen blocks Fired-Up";
                    nextAbility = "Makes 5 chosen blocks Fired-Up";
                    break;
                }
            case 6:
                {
                    currAbility = "Makes 5 chosen blocks Fired-Up";
                    nextAbility = "Makes 5 chosen blocks Fired-Up";
                    break;
                }
            case 7:
                {
                    currAbility = "Makes 5 chosen blocks Fired-Up";
                    nextAbility = "Makes 5 chosen blocks Fired-Up";
                    break;
                }
            case 8:
                {
                    currAbility = "Makes 5 chosen blocks Fired-Up";
                    nextAbility = "Makes 5 chosen blocks Fired-Up";
                    break;
                }
            case 9:
                {
                    currAbility = "Makes 5 chosen blocks Fired-Up";
                    nextAbility = "5 blocks become SUPER BOMBS";
                    break;
                }
            case 10:
                {
                    currAbility = "5 blocks become SUPER BOMBS";
                    nextAbility = "";
                    break;
                }
            default:
                {
                    currAbility = "";
                    nextAbility = "";
                    break;
                }
        }
        return "<color=red>" + currAbility + "</color>" + "|" + "<color=red>" + nextAbility + "</color>";
    }
    public void SetBoostLevel(int lvl)
    {
        boostLevel = lvl;
        if (boostLevel < 4)
        {
            OnPunch = DestroyPunchedBlock;
        }
        else if (boostLevel < 10)
        {
            OnPunch = MakePunchedFiredUp;
        }
        else
        {
            OnPunch = DestroyAllInCross;
        }
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
                    spriteIndex = 2;
                    break;
                }
            case 8:
                {
                    spriteIndex = 2;
                    break;
                }
            case 9:
                {
                    spriteIndex = 2;
                    break;
                }
            case 10:
                {
                    spriteIndex = 3;
                    break;
                }
        }
    }
}