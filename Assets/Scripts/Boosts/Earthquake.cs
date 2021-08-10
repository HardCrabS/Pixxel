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
            rockburst = Resources.Load<GameObject>(RESOURCES_FOLDER + "Earthquake/explosion");
            ShakeSFX = Resources.Load<AudioClip>(RESOURCES_FOLDER + "Earthquake/sfx_boost_earthquakeshake2");
            FiredSFX = Resources.Load<AudioClip>(RESOURCES_FOLDER + "Earthquake/sfx_boost_earthquake_block");
            grid = GridA.Instance;
        }
        audioSource.PlayOneShot(ShakeSFX);
        Camera.main.GetComponent<CameraShake>().ShakeCam(2f, 3f);
       // StartCoroutine(DetonateSpecialBlocks());
        StartCoroutine(MakeAllFired());  //STARTS ROUTINE OF MAKING BLOCKS FIRED
        StartCoroutine(DetonateSpecialBlocks());
    }

    IEnumerator DetonateSpecialBlocks()
    {
        foreach (var block in grid.allBoxes)
        {
            if (block)
            {
                var boxComp = block.GetComponent<Box>();
                if (boxComp)
                {
                    if (boxComp.currState == BoxState.FiredUp || boxComp.currState == BoxState.Warped || boxComp.currState == BoxState.Golden)
                    {
                        grid.DestroyBlockAtPosition(boxComp.row, boxComp.column);
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }
        }
        StartCoroutine(grid.MoveBoxesDown());
    }

    IEnumerator MakeAllFired()
    {
        yield return new WaitForSeconds(1.5f); //wait 2 seconds before continuing
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