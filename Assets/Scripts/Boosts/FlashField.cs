﻿using System.Collections;
using UnityEngine;

public class FlashField : BoostBase
{
    int blockToMakeFiredUp = 5;
    GameObject lightning;
    AudioClip flashStart, flashStrike;
    GridA grid;

    public override void ExecuteBonus()
    {
        base.ExecuteBonus();
        if (lightning == null)
        {
            lightning = Resources.Load<GameObject>(RESOURCES_FOLDER + "Flash Field/Lightning");
            flashStart = Resources.Load<AudioClip>(RESOURCES_FOLDER + "Flash Field/sfx_boost_flashf");
            flashStrike = Resources.Load<AudioClip>(RESOURCES_FOLDER + "Flash Field/sfx_boost_flashf1");
        }
        audioSource.PlayOneShot(flashStart);
        GridA.Instance.currState = GameState.wait; //disallow block movement
        StartCoroutine(MakeAllFiredUp());
    }

    IEnumerator MakeAllFiredUp()
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < blockToMakeFiredUp; i++)
        {
            int randX = Random.Range(0, grid.width);
            int randY = Random.Range(0, grid.hight);
            if (grid.allBoxes[randX, randY] != null)
                MakeBlockFiredUp(grid.allBoxes[randX, randY].GetComponent<Box>(), new Vector2(randX, randY));
            yield return new WaitForSeconds(0.5f);
        }
        GridA.Instance.currState = GameState.move;
        finished = true;
    }

    void MakeBlockFiredUp(Box box, Vector2 pos)
    {
        audioSource.PlayOneShot(flashStrike);
        GameObject go = Instantiate(lightning, pos, transform.rotation);
        Camera.main.GetComponent<CameraShake>().ShakeCam(0.1f, 1f);
        Destroy(go, 0.4f);
        grid.SetBlockFiredUp(box);
    }
    public override void SetBoostLevel(int lvl)
    {
        base.SetBoostLevel(lvl);
        grid = GridA.Instance;
        if(lvl <= 3)
        {
            blockToMakeFiredUp = 5;//5% of all blocks
        }
        else if(lvl <= 6)
        {
            blockToMakeFiredUp = 7;//10% of all blocks
        }
        else if(lvl <= 9)
        {
            blockToMakeFiredUp = 9;//15% of all blocks
        }
        else
        {
            blockToMakeFiredUp = 10;//20% of all blocks
        }
    }
}