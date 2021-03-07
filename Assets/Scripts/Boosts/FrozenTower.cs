using System.Collections;
using UnityEngine;

public class FrozenTower : BoostBase
{
    private int columnsToDestroy = 1;
    private float timeBetwColumnsFreeze = 0.2f;
    private Sprite frozenBlock;
    private GameObject freezeParticle;
    AudioClip iceStart, iceFreeze, iceBreak;

    GridA grid;
    int[] randColumns;

    public override void ExecuteBonus()
    {
        if (frozenBlock == null)
        {
            grid = GridA.Instance;
            frozenBlock = Resources.Load<Sprite>(RECOURSES_FOLDER + "Frozen Tower/Frozen Block");
            freezeParticle = Resources.Load<GameObject>(RECOURSES_FOLDER + "Frozen Tower/Freeze Particle");

            iceStart = Resources.Load<AudioClip>(RECOURSES_FOLDER + "Frozen Tower/sfx_boost_icestart");
            iceFreeze = Resources.Load<AudioClip>(RECOURSES_FOLDER + "Frozen Tower/sfx_boost_icefreeze");
            iceBreak = Resources.Load<AudioClip>(RECOURSES_FOLDER + "Frozen Tower/sfx_boost_icebreak");
        }
        randColumns = new int[columnsToDestroy];
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
            GameObject go = Instantiate(freezeParticle, new Vector2(randColumns[j], 0), Quaternion.EulerAngles(-90, 0, 0));
            Destroy(go, 10);
            StartCoroutine(FreezeBlockColumn(randColumns[j]));
            yield return new WaitForSeconds(timeBetwColumnsFreeze);
        }
    }

    IEnumerator FreezeBlockColumn(int column)
    {
        for (int i = 0; i < grid.hight; i++)
        {
            if (grid.allBoxes[column, i] != null)
            {
                grid.allBoxes[column, i].GetComponent<SpriteRenderer>().sprite = frozenBlock;
                audioSource.PlayOneShot(iceFreeze);
            }
            yield return new WaitForSeconds(0.4f);
        }
    }

    IEnumerator DestroyAllBlockColumns()
    {
        yield return new WaitForSeconds(timeBetwColumnsFreeze * columnsToDestroy + 8 * 0.4f);
        for (int j = 0; j < columnsToDestroy; j++)
        {
            for (int i = 0; i < grid.hight; i++)
            {
                Destroy(grid.allBoxes[randColumns[j], i]);
            }
        }
        audioSource.PlayOneShot(iceBreak);
        StartCoroutine(Camera.main.GetComponent<CameraShake>().Shake(0.1f, 0.2f));
        StartCoroutine(grid.MoveBoxesDown());
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