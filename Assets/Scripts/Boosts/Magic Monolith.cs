using System.Collections;
using UnityEngine;

public class MagicMonolith : BoostBase
{
    int blockToMakeSpecial = 3;
    GameObject magicBarrier;
    AudioClip flashStart, flashStrike;
    GridA grid;

    public override void ExecuteBonus()
    {
        if (magicBarrier == null)
        {
            magicBarrier = Resources.Load<GameObject>(RESOURCES_FOLDER + "Magic Monolith/MagicBarrierBlast");
            flashStrike = Resources.Load<AudioClip>(RESOURCES_FOLDER + "Magic Monolith/sfx_boost_monolithnoise2");
        }
        audioSource.PlayOneShot(flashStart);
        StartCoroutine(MakeAllFiredUp());
    }

    IEnumerator MakeAllFiredUp()
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < blockToMakeSpecial; i++) //Get number of Blocks to make Special
        {
            int randX = Random.Range(0, grid.width);
            int randY = Random.Range(0, grid.hight);
            if (grid.allBoxes[randX, randY] != null)
                MakeBlockFiredUp(grid.allBoxes[randX, randY].GetComponent<Box>(), new Vector2(randX, randY)); //Runs MakeBlockFiredUp Script below on random blocks
            yield return new WaitForSeconds(0.1f);
        }
        finished = true;
    }

    void MakeBlockFiredUp(Box box, Vector2 pos)
    {
        audioSource.PlayOneShot(flashStrike); //play audio sfx
        GameObject go = Instantiate(magicBarrier, pos, transform.rotation); // make magic barrier
        Camera.main.GetComponent<CameraShake>().ShakeCam(0.1f, 1f); //shake screen effect
        Destroy(go, 0.4f); //destroy instance of magic barrier

        int specialBlockChance = Random.Range(0, 100);
        if (specialBlockChance < 50)
        {
            grid.SetBlockFiredUp(box);
        }
        else
        {
            grid.SetBlockWarped(box);
        }
    }
    public override void SetBoostLevel(int lvl)
    {
        base.SetBoostLevel(lvl);
        grid = GridA.Instance;
        if(lvl <= 3)
        {
            blockToMakeSpecial = 3;//3 Blocks turns special
        }
        else if(lvl <= 6)
        {
            blockToMakeSpecial = 4;//5 Blocks turns special
        }
        else if(lvl <= 9)
        {
            blockToMakeSpecial = 5;//7 Blocks turns special
        }
        else
        {
            blockToMakeSpecial = 6;//10 Blocks turns special
        }
    }
}