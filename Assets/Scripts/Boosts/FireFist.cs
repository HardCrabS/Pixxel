using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireFist : BoostBase
{
    float fistSpeed = 6;

    private int timeToBonusLast = 4;
    private int maxClicks = 5;
    private float startTime;
    private GameObject punchPanel;
    private GameObject fistPrefab;
    private GameObject punchParticle;
    private GameObject selectHand;
    private Text timeText;
    GridA grid;
    CameraShake cameraShake;
    AudioClip sfxStart, selectBoxSFX, fistFadingIn, fistHit;

    List<Box> clickedBlocks;
    private int totalClicksMade = 0;

    delegate void OnFistPunch(Vector2Int target);
    OnFistPunch OnPunch;

    const string FOLDER_NAME = "Fire Fist/";

    public override void Start()
    {
        base.Start();

        clickedBlocks = new List<Box>();
    }

    public override void ExecuteBonus()
    {
        grid = GridA.Instance;
        if (cameraShake == null)
            cameraShake = Camera.main.GetComponent<CameraShake>();
        grid.currState = GameState.wait;

        if (punchPanel == null)
        {
            punchPanel = Resources.Load<GameObject>(RESOURCES_FOLDER + FOLDER_NAME + "Punch Time Panel");
            fistPrefab = Resources.Load<GameObject>(RESOURCES_FOLDER + FOLDER_NAME + "Fist");
            punchParticle = Resources.Load<GameObject>(RESOURCES_FOLDER + FOLDER_NAME + "Punch Particle");
            selectHand = Resources.Load<GameObject>(RESOURCES_FOLDER + FOLDER_NAME + "Select_Hand");

            sfxStart = Resources.Load<AudioClip>(RESOURCES_FOLDER + FOLDER_NAME + "sfx_boosts_blackh");
            selectBoxSFX = Resources.Load<AudioClip>(RESOURCES_FOLDER + FOLDER_NAME + "sfx_boost_ironf_1");
            fistFadingIn = Resources.Load<AudioClip>(RESOURCES_FOLDER + FOLDER_NAME + "sfx_boost_ironf_2");
            fistHit = Resources.Load<AudioClip>(RESOURCES_FOLDER + FOLDER_NAME + "sfx_boost_ironf_3");
        }

        startTime = timeToBonusLast;

        audioSource.PlayOneShot(sfxStart);
        SpawnTimePanel();
        StartCoroutine(TimePanel());
        AssignBlockDelegates();
    }

    void SpawnFist(Box box)
    {
        float width = grid.width;

        Vector2 spawnPos = Vector2.zero;

        if (box.row > width - box.row) //block is closer to the right
        {
            int random = Random.Range(0, 100);
            if (random < 33)
            {
                spawnPos = new Vector2(width + 2, box.column + 4);
            }
            else if (random < 66)
            {
                spawnPos = new Vector2(width + 2, box.column + 1);
            }
            else if (random < 100)
            {
                spawnPos = new Vector2(width + 2, box.column - 5);
            }
        }
        else //block is closer to the left
        {
            int random = Random.Range(0, 100);
            if (random < 33)
            {
                spawnPos = new Vector2(-2, box.column + 4);
            }
            else if (random < 66)
            {
                spawnPos = new Vector2(-2, box.column + 1);
            }
            else if (random < 100)
            {
                spawnPos = new Vector2(-2, box.column - 5);
            }
        }

        GameObject fist = Instantiate(fistPrefab, spawnPos, transform.rotation);

        Vector3 diff = box.transform.position - fist.transform.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        fist.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);

        StartCoroutine(MoveFistToTarget(fist, box.transform));
        audioSource.PlayOneShot(fistFadingIn, audioSource.volume - 0.4f);
    }

    IEnumerator MoveFistToTarget(GameObject fist, Transform target)
    {
        float currFistSpeed = fistSpeed * 0.5f;
        float totalDistance = Vector3.Distance(fist.transform.position, target.position);
        float acceleratedFistDistance = totalDistance * 0.6f;
        Vector2 lastTargetPosition = target.position;
        while (true)
        {
            if (target != null)
            {
                //save target last position, so if target destroyed fist will move to it's last position
                lastTargetPosition = target.position;
            }
            fist.transform.position = Vector2.MoveTowards(fist.transform.position, lastTargetPosition, currFistSpeed * Time.deltaTime);

            if (Vector3.Distance(fist.transform.position, lastTargetPosition) < acceleratedFistDistance)
            {
                currFistSpeed = fistSpeed * 2;
            }
            if (fist.transform.position == (Vector3)lastTargetPosition)
            {
                audioSource.PlayOneShot(fistHit);
                cameraShake.ShakeCam(.1f, .9f);
                OnPunch(Vector2Int.RoundToInt(lastTargetPosition));
                GameObject particle = Instantiate(punchParticle, lastTargetPosition, transform.rotation);
                Destroy(particle, 0.8f);
                Destroy(fist);
                yield break;
            }

            yield return null;
        }
    }

    private void DestroyPunchedBlock(Vector2Int target)
    {
        if (grid.allBoxes[target.x, target.y] != null)
            grid.DestroyBlockAtPosition(target.x, target.y);
    }

    private void MakePunchedFiredUp(Vector2Int target)
    {
        if (grid.allBoxes[target.x, target.y] != null)
        {
            grid.allBoxes[target.x, target.y].GetComponent<Box>().currState = BoxState.FiredUp;
            grid.DestroyBlockAtPosition(target.x, target.y);
        }
    }

    Vector2[] directions = new Vector2[] { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
    private void DestroyAllInCross(Vector2Int target)
    {
        if (grid.allBoxes[target.x, target.y] == null)
            return;

        grid.DestroyBlockAtPosition(target.x, target.y);
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
            grid.DestroyBlockAtPosition(x, y);

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
                /*foreach (var bg in scrollBackgrounds)
                {
                    bg.ResumeScrolling();
                }*/
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
                        while (clickedBlocks.Contains(grid.allBoxes[randX, randY].GetComponent<Box>()));
                        clickedBlocks.Add(grid.allBoxes[randX, randY].GetComponent<Box>());
                    }
                }
                for (int i = 0; i < clickedBlocks.Count; i++) //spawn fist for every clicked block
                {
                    SpawnFist(clickedBlocks[i]);
                }
                ClearBlockDelegates();
                clickedBlocks.Clear();
                totalClicksMade = 0;

                yield return new WaitForSeconds(2);
                StartCoroutine(grid.MoveBoxesDown());
                finished = true;
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
                    grid.allBoxes[i, j].GetComponent<Box>().blockClicked += ClickOnBlock;
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
                    grid.allBoxes[i, j].GetComponent<Box>().blockClicked -= ClickOnBlock;
                }
            }
        }
    }

    void ClickOnBlock(int x, int y)
    {
        Box box = grid.allBoxes[x, y].GetComponent<Box>();
        if (totalClicksMade >= maxClicks || clickedBlocks.Contains(box))
        {
            return;
        }
        audioSource.PlayOneShot(selectBoxSFX);
        totalClicksMade++;
        clickedBlocks.Add(box);
        grid.allBoxes[x, y].GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f);
        Instantiate(selectHand, new Vector2(x - 0.8f, y + 0.8f), transform.rotation, grid.allBoxes[x, y].transform);
    }
    public override void SetBoostLevel(int lvl)
    {
        base.SetBoostLevel(lvl);

        if (lvl <= 3)
        {
            OnPunch = DestroyPunchedBlock;
        }
        else if (lvl <= 9)
        {
            OnPunch = MakePunchedFiredUp;
        }
        else
        {
            OnPunch = DestroyAllInCross;
        }
    }
}