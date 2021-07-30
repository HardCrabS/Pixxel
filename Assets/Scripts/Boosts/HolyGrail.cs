using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HolyGrail : BoostBase
{
    int percentOfBombstoDestroy = 50; //set amount of bombs to destroy at 50% - this changes with higher levels
    Sprite godray;
    AudioClip choir;
    GridA grid;

    public override void ExecuteBonus()
    {
        base.ExecuteBonus();

        if (grid == null)
        {
            grid = GridA.Instance;
            godray = Resources.Load<Sprite>(RESOURCES_FOLDER + "Holy Grail/holylightFX");
            choir = Resources.Load<AudioClip>(RESOURCES_FOLDER + "Holy Grail/sfx_boost_holygrail");
        }

        StartCoroutine(SpawnGodray());
    }

    IEnumerator SpawnGodray()
    {
        Vector2 pos = new Vector2(3.5f, 3.5f);//where to spawn godray
        //GameObject go = Instantiate(godray, pos, transform.rotation); //spawns godray graphic
        audioSource.PlayOneShot(choir); //plays choir SFX
        yield return new WaitForSeconds(1f); //wait 1 second
        yield return StartCoroutine(DestroyBombs());//wait until bombs destroyed
        StartCoroutine(grid.MoveBoxesDown());//spawn new blocks and move them down
        finished = true;//boost finished, allow other boost activasion
    }

    IEnumerator DestroyBombs()
    {
        int bombsToDestroy = (int)(CountTotalBombs() * (percentOfBombstoDestroy * 0.01f));

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
                    grid.DestroyBlockAtPosition(i, randColumn);
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
            percentOfBombstoDestroy = 50; //blows up 50% of bombs
        }
        else if (lvl <= 6)
        {
            percentOfBombstoDestroy = 60; //blows up 60% of bombs
        }
        else if (lvl <= 9)
        {
            percentOfBombstoDestroy = 70; //blows up 70% of bombs
        }
        else
        {
            percentOfBombstoDestroy = 80; //blows up 80% of bombs
        }
    }
}