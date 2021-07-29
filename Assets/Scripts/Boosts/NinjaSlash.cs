using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; //uses fade tween
using UnityEngine.Events;

public class NinjaSlash : BoostBase
{
    GameObject thunderAnimPrefab;
    AudioClip fireClip;
    AudioClip swordSlash;
    AudioClip swordSlash2;

    GridA grid;

    List<UnityAction> thunderActions = new List<UnityAction>();

    const string FOLDER_NAME = "Ninja Slash/";

    public override void ExecuteBonus() // STARTS WHEN BOOST TAPPED
    {
        base.ExecuteBonus();

        grid = GridA.Instance;
        grid.currState = GameState.wait;

        GetResources();

        StartCoroutine(RunThunderActions());

        audioSource.PlayOneShot(fireClip); //play sound on start
    }

    private void GetResources()
    {
        if (thunderAnimPrefab == null)
        {
            thunderAnimPrefab = Resources.Load<GameObject>(RESOURCES_FOLDER + FOLDER_NAME + "thunderstrikeanim_0 1");
            fireClip = Resources.Load<AudioClip>(RESOURCES_FOLDER + FOLDER_NAME + "sfx_boost_fire");
            swordSlash = Resources.Load<AudioClip>(RESOURCES_FOLDER + FOLDER_NAME + "sfx_boost_ninjaslash");
            swordSlash2 = Resources.Load<AudioClip>(RESOURCES_FOLDER + FOLDER_NAME + "sfx_boost_ninjaslash2");

        }
    }

    IEnumerator RunThunderActions()
    {
        for (int i = 0; i < thunderActions.Count; i++)
        {
            thunderActions[i].Invoke();
            audioSource.PlayOneShot(swordSlash); //play sound on start
            audioSource.PlayOneShot(swordSlash2); //play sound on start

            Camera.main.GetComponent<CameraShake>().ShakeCam(0.1f, 1f);
            yield return new WaitForSeconds(0.3f);
        }
        yield return new WaitForSeconds(0.5f);
        finished = true;
        SetBoardAfterBoost();
    }
    void SetBoardAfterBoost()
    {
        StartCoroutine(grid.MoveBoxesDown());
        grid.currState = GameState.move;
    }
    void ThunderStrikeLine(Vector2Int start, Vector2Int end, float rot = 0, float scale = 1)
    {
        DestroyLineOfBlocks(start, end);
        //position thunder in the middle between start and end
        Vector2 thunderPos = new Vector2((start.x + end.x) * 0.5f, (start.y + end.y) * 0.5f);
        StartCoroutine(SpawnThunder(thunderPos, rot, scale));
    }
    void ThunderStrikeDiagonal(Vector2Int start, Vector2Int end, float rot = 0, float scale = 1)
    {
        DestroyDiagonalLineOfBlocks(start, end);
        Vector2 thunderPos = new Vector2((start.x + end.x) * 0.5f, (start.y + end.y) * 0.5f);
        StartCoroutine(SpawnThunder(thunderPos, rot, scale));
    }

    IEnumerator SpawnThunder(Vector2 pos, float rot, float scale = 1)
    {
        GameObject thunder = Instantiate(thunderAnimPrefab, pos, Quaternion.Euler(0, 0, rot));//spawns thunder n position/rot
        thunder.transform.localScale *= scale;
        thunder.GetComponent<SpriteRenderer>().DOFade(1, 0.5f);//fade in
        yield return new WaitForSeconds(0.5f);
        yield return thunder.GetComponent<SpriteRenderer>().DOFade(0, 0.5f).WaitForCompletion();//fade out
        Destroy(thunder);
    }
    void DestroyLineOfBlocks(Vector2Int start, Vector2Int end)
    {
        for (int x = start.x; x <= end.x; x++)
        {
            for (int y = start.y; y <= end.y; y++)
            {
                if (grid.allBoxes[x, y])
                    grid.DestroyBlockAtPosition(x, y);
            }
        }
    }
    void DestroyDiagonalLineOfBlocks(Vector2Int start, Vector2Int end)
    {
        int x = start.x;
        int y = start.y;
        int xIncrement = start.x < end.x ? 1 : -1;
        int yIncrement = start.y < end.y ? 1 : -1;
        for (; x != end.x + xIncrement && y != end.y + yIncrement; x += xIncrement, y += yIncrement)
        {
            if (grid.allBoxes[x, y])
                grid.DestroyBlockAtPosition(x, y);
        }
    }

    public override void SetBoostLevel(int lvl)
    {
        base.SetBoostLevel(lvl);

        UnityAction thunderAction = new UnityAction(() => ThunderStrikeLine(new Vector2Int(0, 3), new Vector2Int(7, 3)));
        thunderActions.Add(thunderAction);
        //continiously add new actions to avoid repetitiveness
        if (lvl >= 4)
        {
            UnityAction thunderAction2 = new UnityAction(() => ThunderStrikeLine(new Vector2Int(0, 4), new Vector2Int(7, 4)));
            thunderActions.Add(thunderAction2);
        }
        if (lvl >= 7)
        {
            UnityAction thunderAction3 = new UnityAction(() => ThunderStrikeDiagonal(new Vector2Int(0, 0), new Vector2Int(7, 7), 45, 1.3f));
            thunderActions.Add(thunderAction3);
        }
        if (lvl == 10)
        {
            UnityAction thunderAction4 = new UnityAction(() => ThunderStrikeDiagonal(new Vector2Int(0, 7), new Vector2Int(7, 0), -45, 1.3f));
            thunderActions.Add(thunderAction4);
        }
    }
}