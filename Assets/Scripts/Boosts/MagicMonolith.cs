using System.Collections;
using UnityEngine;
using DG.Tweening;

public class MagicMonolith : BoostBase
{
    int blockToMakeSpecial = 3;
    GameObject magicBarrier;
    GameObject monolith;
    AudioClip flashStart, flashStrike;
    AudioClip monostart;
    GridA grid;

    public override void ExecuteBonus()
    {
        if (magicBarrier == null)
        {
            magicBarrier = Resources.Load<GameObject>(RESOURCES_FOLDER + "Magic Monolith/MagicBarrierBlast");
            flashStrike = Resources.Load<AudioClip>(RESOURCES_FOLDER + "Magic Monolith/sfx_boost_monolithnoise2");
            monolith = Resources.Load<GameObject>(RESOURCES_FOLDER + "Magic Monolith/Obelisk_effects_0");
            monostart = Resources.Load<AudioClip>(RESOURCES_FOLDER + "Magic Monolith/sfx_boost_mono1");
           
        }

        StartCoroutine(MakeAllSpecial());

    }

    IEnumerator MakeAllSpecial()
    {
        audioSource.PlayOneShot(flashStart);
        GameObject monolithgo = Instantiate(monolith, new Vector2(3.59f, -3.51f), transform.rotation); // spawn monolith
        monolithgo.transform.DOMove(new Vector2(3.59f, 6.32f), 1); //move monolith to middle
        audioSource.PlayOneShot(monostart); //play audio sfx

        yield return new WaitForSeconds(1f);
        Camera.main.GetComponent<CameraShake>().ShakeCam(0.5f, 0.3f); //shake screen effect

        for (int i = 0; i < blockToMakeSpecial; i++) //Get number of Blocks to make Special
        {

            int randX = Random.Range(0, grid.width);
            int randY = Random.Range(0, grid.hight);
            if (grid.allBoxes[randX, randY] != null)
                MakeBlockSpecial(grid.allBoxes[randX, randY].GetComponent<Box>(), new Vector2(randX, randY)); //Runs MakeBlockFiredUp Script below on random blocks
            yield return new WaitForSeconds(0.1f);
        }

        monolithgo.GetComponent<SpriteRenderer>().DOFade(0, 0.3f); //FADE OUT MONOLITH HERE
        Camera.main.GetComponent<CameraShake>().ShakeCam(1f, 0.3f); //shake screen effect

        finished = true;
    }

    void MakeBlockSpecial(Box box, Vector2 pos)
    {
        audioSource.PlayOneShot(flashStrike); //play audio sfx
        GameObject go = Instantiate(magicBarrier, pos, transform.rotation); // make magic barrier
        //Camera.main.GetComponent<CameraShake>().ShakeCam(0.5f, 0.3f); //shake screen effect
        Destroy(go, 2f); //destroy instance of magic barrier

        int specialBlockChance = Random.Range(0, 100);
        if (specialBlockChance < 33)
        {
            grid.SetBlockFiredUp(box);
        }
        else if(specialBlockChance < 66)
        {
            grid.SetBlockWarped(box);
        }
        else
        {
            grid.SetBlockGoldenRock(box);
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