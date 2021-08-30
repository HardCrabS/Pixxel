using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switcheroo : BoostBase
{
    List<string> tagsToSwitch = new List<string>();
    GridA grid;
    AnimationClip clip;
    AudioClip boxComeIn;

    public override void ExecuteBonus()
    {
        base.ExecuteBonus();

        GridA.Instance.currState = GameState.wait; //disallow block movement


        if (clip == null)
        {
            clip = Resources.Load<AnimationClip>(RESOURCES_FOLDER + "Switcheroo/Switch Anim");
            boxComeIn = Resources.Load<AudioClip>(RESOURCES_FOLDER + "Switcheroo/sfx_game_heartgain");
        }

        string finalTag;    //all picked boxes will switch to it
        do
        {
            finalTag = grid.boxPrefabs[Random.Range(0, grid.boxPrefabs.Length)].tag;
        } while (tagsToSwitch.Contains(finalTag));

        clip.AddEvent(new AnimationEvent()
        {
            time = 0.5f,
            functionName = "ChangeBlockSprite",
            stringParameter = finalTag
        });

        StartCoroutine(PlaySoundDelayed(boxComeIn, 0.5f));
        SwitchBlocksType(tagsToSwitch);
        StartCoroutine(CheckForMatchesDelayed());
    }
    IEnumerator PlaySoundDelayed(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.PlayOneShot(clip);
    }
    private void SwitchBlocksType(List<string> tags)
    {
        for (int i = 0; i < grid.width; i++)
        {
            for (int j = 0; j < grid.hight; j++)
            {
                if (grid.allBoxes[i, j] != null && tags.Contains(grid.allBoxes[i, j].tag))
                {
                    float speed = Random.Range(0.8f, 1.2f);
                    if (grid.allBoxes[i, j].GetComponent<Animation>() == null)
                        grid.allBoxes[i, j].AddComponent<Animation>();
                    grid.allBoxes[i, j].GetComponent<Animation>().AddClip(clip, "");
                    grid.allBoxes[i, j].GetComponent<Animation>()[""].speed = speed;
                    grid.allBoxes[i, j].GetComponent<Animation>().Play("");
                }
            }
        }
    }

    IEnumerator CheckForMatchesDelayed()
    {
        yield return new WaitForSeconds(1f);
        MatchFinder.Instance.FindAllMatches();
        grid.DestroyAllMatches();
        GridA.Instance.currState = GameState.move;
        finished = true;
    }
    void AddRandomTagsToList(int amount)
    {
        grid = GridA.Instance;
        for (int i = 0; i < amount; i++)
        {
            string tag;
            do
            {
                tag = grid.boxPrefabs[Random.Range(0, grid.boxPrefabs.Length)].tag;
            } while (tagsToSwitch.Contains(tag));
            tagsToSwitch.Add(tag);
        }
    }
    public override void SetBoostLevel(int lvl)
    {
        base.SetBoostLevel(lvl);
        
        if(lvl <= 6)
        {
            AddRandomTagsToList(1);
        }
        else if(lvl >= 7 && lvl <= 9)
        {
            AddRandomTagsToList(1);
        }
        else if(lvl == 10)
        {
            AddRandomTagsToList(2);
        }
    }
}