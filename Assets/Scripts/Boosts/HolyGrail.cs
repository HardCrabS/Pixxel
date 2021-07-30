using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HolyGrail : BoostBase
{
    int bombstoDestroy = 50%; //set amount of bombs to destroy at 50% - this changes with higher levels
    GameObject godray;
    AudioClip choir;
    GridA grid;

    public override void ExecuteBonus()
    {
        if (grid == null)
        {
            godray = Resources.Load<Sprite>(RESOURCES_FOLDER + "Holy Grail/holylightFX");
            choir = Resources.Load<AudioClip>(RESOURCES_FOLDER + "Holy Grail/sfx_boost_holygrail");
        }

        GameObject go = Instantiate(godray, pos, transform.rotation); //spawns godray graphic
        audioSource.PlayOneShot(choir); //plays choir SFX
        yield return new WaitForSeconds(1f); //wait 1 second
        

        // DESTROY.BOMBS = bombstoDestroy HEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEERE

    }


    public override void SetBoostLevel(int lvl)
    {
        base.SetBoostLevel(lvl);

        if (lvl <= 3)
        {
            bombstoDestroy = 50%; //blows up 50% of bombs
        }
        else if (lvl <= 6)
        {
            bombstoDestroy = 60%; //blows up 60% of bombs
        }
        else if (lvl <= 9)
        {
            bombstoDestroy = 70%; //blows up 70% of bombs
        }
        else
        {
            bombstoDestroy = 80%; //blows up 80% of bombs
        }
    }
}