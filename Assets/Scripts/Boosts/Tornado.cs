using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Tornado : BoostBase
{
    float tornadoSpeed = 8f;
    float boxMoveSpeed = 1;
    float boostTime = 3f;
    float timeBetweenBoxSwitch = 0.2f;

    int numOfSpecialBoxes = 0;
    int warpedBoxesChance = 0;

    List<GameObject> movingBoxes;
    GameObject blurAndFog;

    GameObject tornadoPrefab, blurAndFogCanvas;
    AudioClip tornadoSFX;

    GridA grid;
    const string FOLDER_NAME = "Tornado/";
    public override void ExecuteBonus()
    {
        base.ExecuteBonus();
        movingBoxes = new List<GameObject>();
        GetResources();

        audioSource.PlayOneShot(tornadoSFX);
        SpawnFogAndBlur();
        StartCoroutine(MoveTornadoAround());
        StartCoroutine(SwitchBoxesPeriodically());
    }

    private void GetResources()
    {
        if (grid == null)
        {
            grid = GridA.Instance;
            tornadoPrefab = Resources.Load<GameObject>(RESOURCES_FOLDER + FOLDER_NAME + "Tornado");
            blurAndFogCanvas = Resources.Load<GameObject>(RESOURCES_FOLDER + FOLDER_NAME + "Blur Blocks Canvas");

            tornadoSFX = Resources.Load<AudioClip>(RESOURCES_FOLDER + FOLDER_NAME + "sfx_boost_tornado2");
        }
    }
    void SpawnFogAndBlur()
    {
        blurAndFog = Instantiate(blurAndFogCanvas);
        blurAndFog.GetComponent<Canvas>().worldCamera = Camera.main;
        //set fog alpha to 0 before fading in
        blurAndFog.transform.GetChild(1).GetComponent<Image>().material.SetFloat("_Color", 0);
        StartCoroutine(FadeCanvas(1, 1f));
    }
    IEnumerator FadeCanvas(float targetValue, float duration)
    {
        blurAndFog.transform.GetChild(0).GetComponent<Image>().material.DOFade(targetValue, duration);
        yield return blurAndFog.transform.GetChild(1).GetComponent<Image>().material.DOFade(targetValue, duration).WaitForCompletion();
    }
    IEnumerator MoveTornadoAround()
    {
        Vector2 startPos = GetRandomBlockPos();
        Transform tornado = Instantiate(tornadoPrefab, startPos, transform.rotation).transform;
        Destroy(tornado.gameObject, boostTime + 5);

        float timer = boostTime;
        while (timer > 0)
        {
            Vector2 targetPos = GetRandomBlockPos();
            while ((Vector2)tornado.position != targetPos)
            {
                tornado.position = Vector2.MoveTowards(tornado.position, targetPos, tornadoSpeed * Time.deltaTime);
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    break;
                }
                yield return null;
            }
        }
        grid.DestroyAllMatches();
        finished = true;

        int rand = Random.Range(0, 2);
        Vector2 finish = new Vector2(rand == 0 ? -3 : grid.width + 3, Random.Range(1, 8));
        StartCoroutine(MoveTo(tornado, finish, tornadoSpeed + 5));

        yield return StartCoroutine(FadeCanvas(0, 1f));
        Destroy(blurAndFog);
    }
    IEnumerator SwitchBoxesPeriodically()
    {
        while (!finished)
        {
            int numToSwitch = Random.Range(1, 5);
            SwitchRandomBlocks(numToSwitch);
            yield return new WaitForSeconds(Random.Range(timeBetweenBoxSwitch, timeBetweenBoxSwitch + 0.1f));
        }
        MakeBoxesSpecial(numOfSpecialBoxes, warpedBoxesChance);
    }
    void SwitchRandomBlocks(int numberToSwitch = 1)
    {
        for (int i = 0; i < numberToSwitch; i++)
        {
            GameObject box1, box2;
            int randX1, randY1, randX2, randY2;
            do
            {
                do
                {
                    randX1 = Random.Range(0, grid.width);
                    randY1 = Random.Range(0, grid.hight);
                    box1 = grid.allBoxes[randX1, randY1];
                }
                while (box1 == null || movingBoxes.Contains(box1));   //get first box which is not moving at the moment

                do
                {
                    randX2 = Random.Range(0, grid.width);
                    randY2 = Random.Range(0, grid.hight);
                    box2 = grid.allBoxes[randX2, randY2];
                }
                while (box2 == null || movingBoxes.Contains(box2));   //get second box
            }
            while (box1 == box2);   //make sure they are not equal

            movingBoxes.Add(box1);
            movingBoxes.Add(box2);
            StartCoroutine(SwitchBoxPositions(randX1, randY1, randX2, randY2));
        }
    }

    IEnumerator SwitchBoxPositions(int x1, int y1, int x2, int y2)
    {
        GameObject box1, box2;
        box1 = grid.allBoxes[x1, y1];
        box2 = grid.allBoxes[x2, y2];
        Box boxComp1 = box1.GetComponent<Box>();
        Box boxComp2 = box2.GetComponent<Box>();

        //switch positions
        boxComp1.UpdatePos(x2, y2, moveBoxInPosition: true);
        boxComp2.UpdatePos(x1, y1, moveBoxInPosition: true);

        yield return new WaitForSeconds(0.5f);

        movingBoxes.Remove(box1);
        movingBoxes.Remove(box2);
    }

    IEnumerator MoveTo(Transform obj, Vector2 targetPos, float speed)
    {
        float t = 0;
        while (t < 1)
        {
            if (obj == null) yield break;
            t += Time.deltaTime * speed;
            obj.position = Vector3.Lerp(obj.position, targetPos, t);
            yield return null;
        }
    }

    Vector2 GetRandomBlockPos()
    {
        int randX, randY;
        do
        {
            randX = Random.Range(0, grid.width);
            randY = Random.Range(0, grid.hight);
        }
        while (grid.allBoxes[randX, randY] == null);

        return grid.allBoxes[randX, randY].transform.position;
    }
    void MakeBoxesSpecial(int numOfSpecialBoxes, int warpedChance)
    {
        List<Box> specialBoxes = new List<Box>();
        int maxNumOfTries = 5;
        for (int i = 0; i < numOfSpecialBoxes; i++)
        {
            Box box = null;
            int tryCounter = 0;
            do
            {
                int randX = Random.Range(0, grid.width);
                int randY = Random.Range(0, grid.hight);
                if (grid.allBoxes[randX, randY] != null)
                {
                    box = grid.allBoxes[randX, randY].GetComponent<Box>();
                    int randWarpedChance = Random.Range(0, 100);
                    if (randWarpedChance < warpedChance)
                    {
                        grid.SetBlockWarped(box);
                    }
                    else
                        grid.SetBlockFiredUp(box);
                }
                tryCounter++;
            }
            while (specialBoxes.Contains(box) && tryCounter < maxNumOfTries);

            if (box != null)
                specialBoxes.Add(box);
        }
    }

    public override void SetBoostLevel(int lvl)
    {
        base.SetBoostLevel(lvl);
        if (lvl >= 4 && lvl <= 6)
        {
            numOfSpecialBoxes = 2;
        }
        else if (lvl <= 9)
        {
            numOfSpecialBoxes = 3;
        }
        else
        {
            numOfSpecialBoxes = 3;
            warpedBoxesChance = 50;
        }
    }
}