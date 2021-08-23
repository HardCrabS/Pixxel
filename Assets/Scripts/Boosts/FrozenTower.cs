using System.Collections;
using UnityEngine;

public class FrozenTower : BoostBase
{
    int columnsToDestroy = 1;
    float timeBetwColumnsFreeze = 0.2f;
    float timeBetwBlocksFreeze = 0.1f;
    float ballSpeed = 0.25f;

    Sprite frozenBlock;
    GameObject frozenBallPrefab, destroyVFX;
    AudioClip iceStart, iceFreeze, iceBreak;

    GridA grid;
    int[] randColumns;

    const string FOLDER_NAME = "Frozen Tower/";

    public override void ExecuteBonus()
    {
        base.ExecuteBonus();
        if (frozenBlock == null)
        {
            grid = GridA.Instance;
            frozenBlock = Resources.Load<Sprite>(RESOURCES_FOLDER + FOLDER_NAME + "Frozen Block");
            frozenBallPrefab = Resources.Load<GameObject>(RESOURCES_FOLDER + FOLDER_NAME + "Frozen ball");
            destroyVFX = Resources.Load<GameObject>(RESOURCES_FOLDER + FOLDER_NAME + "Frozen Destroyed VFX");

            iceStart = Resources.Load<AudioClip>(RESOURCES_FOLDER + FOLDER_NAME + "sfx_boost_icestart");
            iceFreeze = Resources.Load<AudioClip>(RESOURCES_FOLDER + FOLDER_NAME + "sfx_boost_icefreeze");
            iceBreak = Resources.Load<AudioClip>(RESOURCES_FOLDER + FOLDER_NAME + "sfx_boost_icebreak");
        }
        randColumns = new int[columnsToDestroy];
        GridA.Instance.currState = GameState.wait; //disallow block movement
        StartCoroutine(FreezeAllColumns());

        StartCoroutine(DestroyAllBlockColumns());
    }

    IEnumerator FreezeAllColumns()
    {
        for (int j = 0; j < columnsToDestroy; j++)
        {
            do
            {
                randColumns[j] = Random.Range(0, grid.width);
            }
            while (IsTheSame(randColumns, randColumns[j], j));

            audioSource.PlayOneShot(iceStart);
            //GameObject go = Instantiate(frozenBall, new Vector2(randColumns[j], 0), Quaternion.EulerAngles(-90, 0, 0));
            GameObject frozenBallClone = Instantiate(frozenBallPrefab, new Vector2(randColumns[j], 8), transform.rotation);
            StartCoroutine(MoveFrozenBall(frozenBallClone.transform, new Vector2(randColumns[j], 1.2f)));
            Destroy(frozenBallClone, 10);
            StartCoroutine(FreezeBlockColumn(randColumns[j]));
            yield return new WaitForSeconds(timeBetwColumnsFreeze);
        }
    }
    IEnumerator MoveFrozenBall(Transform ball, Vector2 target)
    {
        float t = 0;

        while (t < 1)
        {
            t += ballSpeed * Time.deltaTime;
            ball.position = Vector3.Lerp(ball.position, target, t);
            yield return null;
        }
    }
    IEnumerator FreezeBlockColumn(int column)
    {
        for (int i = grid.hight - 1; i >= 0; i--)
        {
            GameObject box = grid.allBoxes[column, i];
            if (box != null)
            {
                if (box.GetComponent<BombTile>())
                {
                    box.GetComponentInChildren<Canvas>().enabled = false;
                    Destroy(box.GetComponentInChildren<ParticleSystem>());
                }
                box.GetComponent<SpriteRenderer>().sprite = frozenBlock;
                audioSource.PlayOneShot(iceFreeze);
            }
            yield return new WaitForSeconds(timeBetwBlocksFreeze);
        }
    }

    IEnumerator DestroyAllBlockColumns()
    {
        yield return new WaitForSeconds(timeBetwColumnsFreeze * columnsToDestroy + 8 * timeBetwBlocksFreeze);
        for (int j = 0; j < columnsToDestroy; j++)
        {
            for (int i = 0; i < grid.hight; i++)
            {
                var part = Instantiate(destroyVFX, new Vector2(randColumns[j], i), transform.rotation);
                Destroy(part, 2);
                Destroy(grid.allBoxes[randColumns[j], i]);
            }
        }
        audioSource.PlayOneShot(iceBreak);
        Camera.main.GetComponent<CameraShake>().ShakeCam(0.1f, 1f);
        StartCoroutine(grid.MoveBoxesDown());
        GridA.Instance.currState = GameState.move;
        finished = true;
    }

    bool IsTheSame(int[] arr, int x, int a)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (i == a)
                continue;
            if (arr[i] == x)
                return true;
        }
        return false;
    }
    public override void SetBoostLevel(int lvl)
    {
        base.SetBoostLevel(lvl);

        if (lvl <= 3)
        {
            columnsToDestroy = 1;
        }
        else if (lvl <= 6)
        {
            columnsToDestroy = 2;
        }
        else if (lvl <= 9)
        {
            columnsToDestroy = 3;
        }
        else
        {
            columnsToDestroy = 4;
        }
    }
}