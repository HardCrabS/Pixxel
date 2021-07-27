using System.Collections;
using UnityEngine;
using DG.Tweening; //uses fade tween

public class NinjaSlash : BoostBase
{
    //[SerializeField] float lineSpeed = 38;

    GameObject thunderAnimPrefab;
    GameObject thunder1;
    GameObject thunder2;
    GameObject thunder3;
    GameObject thunder4;

    AudioClip fireClip;
    GridA grid;
    Vector3 firstPos;
    Vector3 currPos;
    Vector3 target;

    private bool boostActivated = false;
    private int linesToDestroy = 1;

    const string FOLDER_NAME = "Ninja Slash/";

   /* void SOMESCRIPT
    {
        transform.DoMoveX(100, 1);
    } */

    public override void ExecuteBonus() // STARTS WHEN BOOST TAPPED
    {

        grid = GridA.Instance;
        grid.currState = GameState.wait;

        if (thunderAnimPrefab == null)
        {
            thunderAnimPrefab = Resources.Load<GameObject>(RESOURCES_FOLDER + FOLDER_NAME + "thunderstrikeanim_0 1");
            fireClip = Resources.Load<AudioClip>(RESOURCES_FOLDER + FOLDER_NAME + "sfx_boost_fire");
        }

      
        boostActivated = true;

        // spaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaace //
        // 

        if(linesToDestroy == 1)
            {

            thunder1 = Instantiate(thunderAnimPrefab, new Vector3(3.5f, 4, 0), Quaternion.Euler(0, 0, 0)); //spawns thunder n position/rot
            DestroyLineOfBlocks(0, 3, 7, 3);


            }


        if(linesToDestroy == 2)
            {
            thunder1 = Instantiate(thunderAnimPrefab, new Vector3(3.5f, 4, 0), Quaternion.Euler(0, 0, 0));
            thunder2 = Instantiate(thunderAnimPrefab, new Vector3(3.5f, 3, 0), Quaternion.Euler(0, 0, 0));

            DestroyLineOfBlocks(0, 3, 7, 3);
            DestroyLineOfBlocks(0, 4, 7, 4);
            }

        if(linesToDestroy == 3)
            {
            thunder1 = Instantiate(thunderAnimPrefab, new Vector3(3.5f, 4, 0), Quaternion.Euler(0, 0, 0));
            // WAIT .3SECONDS HERE   <---
            thunder2 = Instantiate(thunderAnimPrefab, new Vector3(3.5f, 3, 0), Quaternion.Euler(0, 0, 0));
            thunder3 = Instantiate(thunderAnimPrefab, new Vector3(3.25f, 3.3f, 0), Quaternion.Euler(0, 0, 45));
            thunder3.transform.localScale *= 1 * 1.3f;
            thunder3.GetComponent<SpriteRenderer>().DoColor(0, 60);
            DestroyLineOfBlocks(0, 3, 7, 3);
            DestroyLineOfBlocks(0, 4, 7, 4);
            DestroyDiagonalLineOfBlocks(0, 0, 7, 7);
            }



        if(linesToDestroy == 4)
            {
            thunder1 = Instantiate(thunderAnimPrefab, new Vector3(3.5f, 4, 0), Quaternion.Euler(0, 0, 0));
            thunder2 = Instantiate(thunderAnimPrefab, new Vector3(3.5f, 3, 0), Quaternion.Euler(0, 0, 0));
            thunder3 = Instantiate(thunderAnimPrefab, new Vector3(3.5f, 4, 0), Quaternion.Euler(0, 0, 90));
            thunder4 = Instantiate(thunderAnimPrefab, new Vector3(3.25f, 3.3f, 0), Quaternion.Euler(0, 0, -180));
            DestroyLineOfBlocks(0, 3, 7, 3);
            DestroyLineOfBlocks(0, 4, 7, 4);
            DestroyDiagonalLineOfBlocks(0, 0, 7, 7);
            DestroyDiagonalLineOfBlocks(7, 0, 7, 7);
            }



        audioSource.PlayOneShot(fireClip); //play sound on start
    }


  /* IEnumerator LaunchLine()
    {
        float totalDist = Vector3.Distance(currPos, target);
        float distBeforeAcceleration = totalDist * 0.1f;
        float lowSpeed = lineSpeed * 0.3f;
        float highSpeed = lineSpeed;
        StartCoroutine(DestroyLineOfBlocks(0, 5, 0));
        while (boostActivated)
        {
            if (Vector3.Distance(currPos, target) > distBeforeAcceleration)
                lineSpeed = lowSpeed;
            else
                lineSpeed = highSpeed;
            CreateLine(lineSpeed);

            yield return null;
        }
        finished = true;
    }

    void CreateLine(float speed)
    {

        currPos = Vector3.MoveTowards(currPos, target, speed * Time.deltaTime);
        //if (Mathf.Abs(currPos.x - (int)currPos.x) <= 0.2 && (int)currPos.x >= 0 && (int)currPos.x < 8)
        //grid.DestroyBlockAtPosition((int)currPos.x, (int)currPos.y);

        if (currPos == target)
        {
            if (firstPos.y == linesToDestroy - 1)
            {
                boostActivated = false;
                Destroy(thunder);
                StartCoroutine(grid.MoveBoxesDown());
                grid.currState = GameState.move;
            }
            else
            {
                firstPos.y++;
                thunder.transform.position = new Vector3(thunder.transform.position.x, thunder.transform.position.y - 1f, firstPos.z);
              //  thunder.transform.rotation = Quaternion.Euler(0, thunder.transform.rotation.eulerAngles.y + 180, -90);
                target = firstPos;

                currPos.y++;
                firstPos = currPos;
                CreateLine(speed);
                int startX = Mathf.Clamp((int)currPos.x, 0, 7);
                int targetX = Mathf.Clamp((int)target.x, 0, 7);
                StartCoroutine(DestroyLineOfBlocks((int)currPos.y, startX, targetX));
                thunder.GetComponent<Animator>().Play("Fire stream", -1, 0);
                audioSource.PlayOneShot(fireClip); //play sound on new line
            }
        }
    }
  /*  IEnumerator DestroyBlocksInRow(int row, int startX, int targetX)
    {
        if (targetX > startX)
        {
            for (int i = startX; i <= targetX; i++)
            {
                float timeToWait = 1 / lineSpeed;
                yield return new WaitForSeconds(timeToWait);
                grid.DestroyBlockAtPosition(i, row);
            }
        }
        else
        {
            for (int i = startX; i >= targetX; i--)
            {
                float timeToWait = 1 / lineSpeed;
                yield return new WaitForSeconds(timeToWait);
                grid.DestroyBlockAtPosition(i, row);
            }
        }
    }*/




    void DestroyLineOfBlocks(int startX, int startY, int endX, int endY)
    {
        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                if (grid.allBoxes[x, y])
                    grid.DestroyBlockAtPosition(x, y);
            }
        }
    }
    void DestroyDiagonalLineOfBlocks(int startX, int startY, int endX, int endY)
    {
        int x = startX;
        int y = startY;
        int xIncrement = startX < endX ? 1 : -1;
        int yIncrement = startY < endY ? 1 : -1;
        for (; x <= endX && y <= endY; x += xIncrement, y += yIncrement)
        {
            if (grid.allBoxes[x, y])
                grid.DestroyBlockAtPosition(x, y);
        }
    }

   public override void SetBoostLevel(int lvl)
    {
        base.SetBoostLevel(lvl);

        if (lvl >= 4 && lvl <= 6)
        {
            linesToDestroy = 2;
        }
        else if (lvl >= 7 && lvl <= 9)
        {
            linesToDestroy = 3;
        }
        else if (lvl == 10)
        {
            linesToDestroy = 4;
        }
    }
}
