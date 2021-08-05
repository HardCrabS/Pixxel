using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HolyGrail : BoostBase
{
    int percentOfBombstoDestroy = 1; //set amount of bombs to destroy - 1, THEN 2,3,4. I CHANGED THIS!
    GameObject godray; //godray fx 
    GameObject bombshine; //NEW SHINE FX ON BLOCK
    GameObject fullBurst; //full screen effect
    AudioClip choir;

    GridA grid;

    public override void ExecuteBonus()
    {
        base.ExecuteBonus();

        GetResources();

        if(CountTotalBombs() == 0)
        {
            finished = true;
            return;
        }

        StartCoroutine(SpawnGodray());
    }

    private void GetResources()
    {
        if (grid == null)
        {
            grid = GridA.Instance;
            bombshine = Resources.Load<GameObject>(RESOURCES_FOLDER + "Holy Grail/LightCast_96");
            godray = Resources.Load<GameObject>(RESOURCES_FOLDER + "Holy Grail/holylight");
            fullBurst = Resources.Load<GameObject>(RESOURCES_FOLDER + "Holy Grail/GrailBurstFX");
            choir = Resources.Load<AudioClip>(RESOURCES_FOLDER + "Holy Grail/sfx_boost_holygrail");
        }
    }

    IEnumerator SpawnGodray()
    {
        Vector2 pos = new Vector2(3.5f, 3.5f);//where to spawn godray
        Vector2 blockpos = new Vector2(1f, 1f);  //where to spawn block glow FX
        godray = Instantiate(godray, pos, transform.rotation); //spawns godray graphic
        audioSource.PlayOneShot(choir); //plays choir SFX
        godray.GetComponent<SpriteRenderer>().DOFade(1, 1); // FADE IN GODRAY
        GameObject burst = Instantiate(fullBurst, pos, transform.rotation); //plays burst fx
        Destroy(burst, 3);
        GameObject bombShine = Instantiate(bombshine, blockpos, transform.rotation); //spawns godray graphic
        yield return new WaitForSeconds(1f); //wait 1 second
        godray.GetComponent<SpriteRenderer>().DOFade(0, 3); //FADE OUT GODRAY
        yield return StartCoroutine(DestroyBombs());//destroy bombs & wait until bombs destroyed
        StartCoroutine(grid.MoveBoxesDown());//spawn new blocks and move them down
        finished = true;//boost finished, allow other boost activasion
    }

    IEnumerator DestroyBombs()
    {
        int bombsToDestroy = (percentOfBombstoDestroy);

        while (bombsToDestroy > 0 && CountTotalBombs() > 0)
        {
            int randColumn = Random.Range(0, grid.width);//get random column

            int startBlockChance = Random.Range(0, 100);
            int startBlock = startBlockChance < 50 ? 0 : grid.hight - 1;//start from below or above?
            int step = startBlockChance < 50 ? 1 : -1;

            for (int i = startBlock; i >= 0 && i < grid.hight; i+=step)//loop for every block in the column
            {
                if (grid.bombTiles[i, randColumn])//bomb found
                {
                    grid.DestroyBlockAtPosition(i, randColumn, playSound:true);
                    bombsToDestroy--;
                    yield return new WaitForSeconds(0.1f);//wait for time before destroying another bomb
                    break;//exit from loop and continue with another column
                }
            }
        } 
    }

    int CountTotalBombs()
    {
        int bombsCounter = 0;
        foreach (var bomb in grid.bombTiles)
        {
            if (bomb)
                bombsCounter++;
        }
        return bombsCounter;
    }

    public override void SetBoostLevel(int lvl)
    {
        base.SetBoostLevel(lvl);

        if (lvl <= 3)
        {
            percentOfBombstoDestroy = 1; //blows up 50% of bombs
        }
        else if (lvl <= 6)
        {
            percentOfBombstoDestroy = 2; //blows up 60% of bombs
        }
        else if (lvl <= 9)
        {
            percentOfBombstoDestroy = 3; //blows up 70% of bombs
        }
        else
        {
            percentOfBombstoDestroy = 4; //blows up 80% of bombs
        }
    }
}