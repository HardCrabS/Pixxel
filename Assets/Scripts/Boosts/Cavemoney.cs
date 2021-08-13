using System.Collections;
using UnityEngine;
using DG.Tweening; //uses fade tween



public class Cavemoney : BoostBase
{
    int blockToMakeGolden = 5;
    GameObject caveman;
   // GameObject burst;
   // GameObject burstsingle;
    AudioClip cavemanMove;
    AudioClip sparkle;
    GridA grid;
    Vector3 firstPos;
    Vector3 currPos;
    Vector3 target;

    public override void ExecuteBonus()
    {
        if (caveman == null)
        {
            caveman = Resources.Load<GameObject>(RESOURCES_FOLDER + "Cavemoney/ugh-sprite_0");
            cavemanMove = Resources.Load<AudioClip>(RESOURCES_FOLDER + "Cavemoney/sfx_boost_caveman_wheels");
            sparkle = Resources.Load<AudioClip>(RESOURCES_FOLDER + "Cavemoney/chimes");

        }

        StartCoroutine(MakeGoldenRocks()); 
        audioSource.PlayOneShot(sparkle); //play chimes sfx
        StartCoroutine(CavemanBoom());

    }

    IEnumerator CavemanBoom()
    {

        GameObject CaveMan = Instantiate(caveman, new Vector2(3.37f, 9.52f), transform.rotation); //spawns caveman at location
        CaveMan.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0); //set caveman opacity to 0

        CaveMan.GetComponent<SpriteRenderer>().DOFade(1, 1); //FADE IN CAVEMAN

        audioSource.PlayOneShot(cavemanMove); //play squeeky wheels sfx
        yield return CaveMan.transform.DOMove(new Vector2(3.37f, -7.5f), 8). WaitForCompletion(); //moves him downwards in 8 seconds

        //yield return CaveMan.GetComponent<SpriteRenderer>().DOMove.WaitForCompletion(); //wait till movement of caveman has finished // 
        Destroy(CaveMan, 1f); //destroy caveman  //WONT WORK AS DOESNT KNOW CaveMan
    }

    IEnumerator MakeGoldenRocks()
    {
        GridA.Instance.currState = GameState.wait; //disallows moving of blocks

        float startVolume = PlayerPrefsController.GetMasterVolume(); //gets volume LVL
        AudioController.Instance.StartFade(0.5f, 0); //fades out music

        for (int y = grid.hight -1; y >= 0; y--)
        {
            for (int x = 3; x < 5; x++)
            {
                if (grid.allBoxes[x, y])
                {

                   // GameObject burstsingle = Instantiate(burst, grid.Box, transform.rotation); // CREATE BURST FX AT BOX
                    grid.SetBlockGoldenRock(grid.allBoxes[x, y].GetComponent<Box>()); // turns block golden
                    
                    yield return new WaitForSeconds(0.2f); //waits 
                }
            }
        } yield return new WaitForSeconds(1f);
        //  DESTROY THE OBJECT CaveMan HERE, AFTER HE HAS FINISHED MOVING        <-------------------------------------------------------------------------------------------
        AudioController.Instance.StartFade(1, startVolume); //fade in music 
        GridA.Instance.currState = GameState.move;
        finished = true;



    }


    void MakeBlockGolden(Box box, Vector2 pos)
    {
        //audioSource.PlayOneShot(flashStrike);
       // GameObject go = Instantiate(lightning, pos, transform.rotation);
        Camera.main.GetComponent<CameraShake>().ShakeCam(0.1f, 1f);
        //Destroy(go, 0.4f);

        grid.SetBlockGoldenRock(box);

    }


    public override void SetBoostLevel(int lvl)
    {
        base.SetBoostLevel(lvl);
        grid = GridA.Instance;
        if(lvl <= 3)
        {
            blockToMakeGolden = 1;//5% of all blocks
        }
        else if(lvl <= 6)
        {
            blockToMakeGolden = 2;//10% of all blocks
        }
        else if(lvl <= 9)
        {
            blockToMakeGolden = 3;//15% of all blocks
        }
        else
        {
            blockToMakeGolden = 4;//20% of all blocks
        }
    }
}