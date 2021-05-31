using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tornado : BoostBase
{
    float tornadoSpeed = 8f;
    float boxMoveSpeed = 1;
    float boostTime = 3f;
    float timeBetweenBoxSwitch = 0.2f;

    int numOfSpecialBoxes = 0;
    int warpedBoxesChance = 0;

    List<GameObject> movingBoxes;
    GameObject fog, blur;

    GameObject tornadoPrefab, fogPrefab, blurCanvas;
    AudioClip tornadoSFX;

    GridA grid;
    const string FOLDER_NAME = "Tornado/";
    public override void ExecuteBonus()
    {
        base.ExecuteBonus();
        movingBoxes = new List<GameObject>();
        GetResources();

        audioSource.PlayOneShot(tornadoSFX);
        SpawnBlur();
        SpawnFog();
        StartCoroutine(MoveTornadoAround());
        StartCoroutine(SwitchBoxesPeriodically());
    }

    private void GetResources()
    {
        if (grid == null)
        {
            grid = GridA.Instance;
            tornadoPrefab = Resources.Load<GameObject>(RESOURCES_FOLDER + FOLDER_NAME + "Tornado");
            fogPrefab = Resources.Load<GameObject>(RESOURCES_FOLDER + FOLDER_NAME + "Fog");
            blurCanvas = Resources.Load<GameObject>(RESOURCES_FOLDER + FOLDER_NAME + "Blur Blocks Canvas");

            tornadoSFX = Resources.Load<AudioClip>(RESOURCES_FOLDER + FOLDER_NAME + "sfx_boost_tornado2");
        }
    }
    void SpawnBlur()
    {
        blur = Instantiate(blurCanvas);
        StartCoroutine(BlurFade(0, 2));
    }
    IEnumerator BlurFade(float startValue, float targetBlurValue, float blurSpeed = 0.5f)
    {
        var mat = blur.GetComponentInChildren<Image>().material;
        mat.SetFloat("_Size", startValue);
        float t = 0;
        while (t < 1)
        {
            t += blurSpeed * Time.deltaTime;
            float blurValue = Mathf.Lerp(mat.color.a, targetBlurValue, t);
            mat.SetFloat("_Size", blurValue);
            yield return null;
        }
    }
    void SpawnFog()
    {
        fog = Instantiate(fogPrefab, fogPrefab.transform.position, transform.rotation);
        Destroy(fog, boostTime + 1);
        fog.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, 0);
        StartCoroutine(FogFadeAlpha(1, 0.1f));
    }
    IEnumerator FogFadeAlpha(float targetAlpha, float alphaSpeed = 1f)
    {
        var mat = fog.GetComponent<MeshRenderer>().material;
        float t = 0;
        while (t < 1)
        {
            t += alphaSpeed * Time.deltaTime;
            float alpha = Mathf.Lerp(mat.color.a, targetAlpha, t);
            mat.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }
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
        StartCoroutine(FogFadeAlpha(0));
        yield return StartCoroutine(BlurFade(2, 0, 0.5f));
        Destroy(blur);
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

        boxComp1.ChangeBoxPosition(x2, y2);
        boxComp2.ChangeBoxPosition(x1, y1);

        grid.allBoxes[x1, y1] = box2;   //switch boxes
        grid.allBoxes[x2, y2] = box1;

        var bombTile = grid.bombTiles[x1, y1];  //switch bombs
        grid.bombTiles[x1, y1] = grid.bombTiles[x2, y2];
        grid.bombTiles[x2, y2] = bombTile;

        boxComp1.enabled = false;   //turn off box snapping to it's row and column
        boxComp2.enabled = false;

        StartCoroutine(MoveTo(box1.transform, box2.transform.position, boxMoveSpeed));
        yield return StartCoroutine(MoveTo(box2.transform, box1.transform.position, boxMoveSpeed));
        if (boxComp1)
            boxComp1.enabled = true;
        if (boxComp2)
            boxComp2.enabled = true;

        movingBoxes.Remove(box1);
        movingBoxes.Remove(box2);
    }

    IEnumerator MoveTo(Transform obj, Vector2 targetPos, float speed)
    {
        float t = 0;
        while (t < 1)
        {
            if (obj == null) yield break;
            t += Time.deltaTime * boxMoveSpeed;
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
            numOfSpecialBoxes = 3;
        }
        else if (lvl <= 9)
        {
            numOfSpecialBoxes = 5;
        }
        else
        {
            numOfSpecialBoxes = 5;
            warpedBoxesChance = 50;
        }
    }
}