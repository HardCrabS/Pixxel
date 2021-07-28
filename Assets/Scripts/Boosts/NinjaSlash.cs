using System.Collections;
using UnityEngine;
using DG.Tweening; //uses fade tween
using System;
using UnityEngine.Events;

public class NinjaSlash : BoostBase
{
    GameObject thunderAnimPrefab;
    AudioClip fireClip;

    GridA grid;
    UnityEvent OnBoostStart = new UnityEvent();

    const string FOLDER_NAME = "Ninja Slash/";

    public override void ExecuteBonus() // STARTS WHEN BOOST TAPPED
    {
        base.ExecuteBonus();

        grid = GridA.Instance;
        grid.currState = GameState.wait;

        GetResources();

        OnBoostStart.Invoke();
        StartCoroutine(SetBoardAfterBoost());

        audioSource.PlayOneShot(fireClip); //play sound on start
    }

    private void GetResources()
    {
        if (thunderAnimPrefab == null)
        {
            thunderAnimPrefab = Resources.Load<GameObject>(RESOURCES_FOLDER + FOLDER_NAME + "thunderstrikeanim_0 1");
            fireClip = Resources.Load<AudioClip>(RESOURCES_FOLDER + FOLDER_NAME + "sfx_boost_fire");
        }
    }

    IEnumerator SetBoardAfterBoost()
    {
        yield return new WaitUntil(() => finished);
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
        yield return new WaitForSeconds(2f);
        yield return thunder.GetComponent<SpriteRenderer>().DOFade(0, 0.5f).WaitForCompletion();//fade out
        Destroy(thunder);
        finished = true;//boost finished executing after thunder is destroyed
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
        if(lvl <= 3)
        {
            OnBoostStart.AddListener(() => ThunderStrikeLine(new Vector2Int(0, 3), new Vector2Int(7, 3)));
        }
        else if (lvl >= 4 && lvl <= 6)
        {
            OnBoostStart.AddListener(() =>
            {
                ThunderStrikeLine(new Vector2Int(0, 3), new Vector2Int(7, 3));
                ThunderStrikeLine(new Vector2Int(0, 4), new Vector2Int(7, 4));
            }
            );
        }
        else if (lvl >= 7 && lvl <= 9)
        {
            OnBoostStart.AddListener(() =>
            {
                ThunderStrikeLine(new Vector2Int(0, 3), new Vector2Int(7, 3));
                ThunderStrikeLine(new Vector2Int(0, 4), new Vector2Int(7, 4));
                ThunderStrikeDiagonal(new Vector2Int(0, 0), new Vector2Int(7, 7), 45, 1.3f);
            }
            );
        }
        else if (lvl == 10)
        {
            OnBoostStart.AddListener(() =>
            {
                ThunderStrikeLine(new Vector2Int(0, 3), new Vector2Int(7, 3));
                ThunderStrikeLine(new Vector2Int(0, 4), new Vector2Int(7, 4));
                ThunderStrikeDiagonal(new Vector2Int(0, 0), new Vector2Int(7, 7), 45, 1.3f);
                ThunderStrikeDiagonal(new Vector2Int(0, 7), new Vector2Int(7, 0), -45, 1.3f);
            }
            );
        }
    }
}