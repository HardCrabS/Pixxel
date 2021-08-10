using System.Collections;
using UnityEngine;

public class Earthquake : BoostBase
{
    int blockToMakeFiredUp = 3; //sets initial blocks 3, (3,4,5,6)
    GameObject rockburst; // burst fx on block
    GameObject rocks; //rocks shaking for earthquake fx
    AudioClip FiredSFX; //block turning fired-up SFX
    AudioClip ShakeSFX; //earthquake SFX
    GridA grid;

    public override void ExecuteBonus()
    {
        if (rocks == null)
        {
            rocks = Resources.Load<GameObject>(RESOURCES_FOLDER + "Flash Field/Lightning");
            rockburst = Resources.Load<GameObject>(RESOURCES_FOLDER + "Earthquake/explode");
            ShakeSFX = Resources.Load<AudioClip>(RESOURCES_FOLDER + "Earthquake/sfx_boost_earthquakeshake2");
            FiredSFX = Resources.Load<AudioClip>(RESOURCES_FOLDER + "Earthquake/sfx_boost_earthquake_block");
        }
        audioSource.PlayOneShot(ShakeSFX);
        Camera.main.GetComponent<CameraShake>().ShakeCam(1f, 2f);
       // StartCoroutine(DetonateSpecialBlocks());
        StartCoroutine(MakeAllFired());  //STARTS ROUTINE OF MAKING BLOCKS FIRED

    }

    /* IEnumerator DetonateSpecialBlocks()
    {
        //ACTIVATE ALL SPECIAL BLOCKS HERE
    }
   */


    IEnumerator MakeAllFired()
    {
        yield return new WaitForSeconds(2f); //wait 2 seconds before continuing
        for (int i = 0; i < blockToMakeFiredUp; i++) // set which blocks to make fired up
        {
            int randX = Random.Range(0, grid.width);
            int randY = Random.Range(0, grid.hight);
            if (grid.allBoxes[randX, randY] != null)
                MakeBlockFiredUp(grid.allBoxes[randX, randY].GetComponent<Box>(), new Vector2(randX, randY)); //make block fired up
            yield return new WaitForSeconds(0.5f);
        }
        finished = true;
    }

    void MakeBlockFiredUp(Box box, Vector2 pos)
    {
      
       GameObject go = Instantiate(rockburst, pos, transform.rotation);
       audioSource.PlayOneShot(FiredSFX);

        // Camera.main.GetComponent<CameraShake>().ShakeCam(0.1f, 1f);
        Destroy(go, 1f);
       grid.SetBlockFiredUp(box);
    }
    public override void SetBoostLevel(int lvl)
    {
        base.SetBoostLevel(lvl);
        grid = GridA.Instance;
        if(lvl <= 3)
        {
            blockToMakeFiredUp = 3;
        }
        else if(lvl <= 6)
        {
            blockToMakeFiredUp = 4;
        }
        else if(lvl <= 9)
        {
            blockToMakeFiredUp = 5;
        }
        else
        {
            blockToMakeFiredUp = 6;
        }
    }
}