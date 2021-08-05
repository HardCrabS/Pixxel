using System.Collections;
using UnityEngine;
using DG.Tweening; //uses fade tween


public class Cavemoney : BoostBase
{
    int blockToMakeFiredUp = 5;
    GameObject caveman;
    AudioClip cavemanMove;
    GridA grid;

    public override void ExecuteBonus()
    {
        if (caveman == null)
        {
            caveman = Resources.Load<GameObject>(RESOURCES_FOLDER + "Cavemoney/ugh-sprite_0");
            cavemanMove = Resources.Load<AudioClip>(RESOURCES_FOLDER + "Cavemoney/sfx_boost_caveman_wheels");
        }

        caveman.transform.DOMove(new Vector2(-200, 50), 4);


        audioSource.PlayOneShot(cavemanMove);
        //StartCoroutine(MakeAllFiredUp());
    }





    /*
    IEnumerator MakeAllFiredUp()
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < blockToMakeFiredUp; i++)
        {
            int randX = Random.Range(0, grid.width);
            int randY = Random.Range(0, grid.hight);
            if (grid.allBoxes[randX, randY] != null)
                MakeBlockFiredUp(grid.allBoxes[randX, randY].GetComponent<Box>(), new Vector2(randX, randY));
            yield return new WaitForSeconds(1f);
        }
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
    */



    public override void SetBoostLevel(int lvl)
    {
        base.SetBoostLevel(lvl);
        grid = GridA.Instance;
        if(lvl <= 3)
        {
            blockToMakeFiredUp = 1;//5% of all blocks
        }
        else if(lvl <= 6)
        {
            blockToMakeFiredUp = 2;//10% of all blocks
        }
        else if(lvl <= 9)
        {
            blockToMakeFiredUp = 3;//15% of all blocks
        }
        else
        {
            blockToMakeFiredUp = 4;//20% of all blocks
        }
    }
}