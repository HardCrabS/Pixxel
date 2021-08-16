using System.Collections;
using UnityEngine;

public class FlameThrower : BoostBase
{
    [SerializeField] float lineSpeed = 38;

    GameObject leadFirePrefab;
    GameObject shineParticles;
    GameObject leadFire, shinePartVFX;
    AudioClip fireClip;
    GridA grid;
    Vector3 firstPos;
    Vector3 currPos;
    Vector3 target;

    private bool boostActivated = false;
    private int linesToDestroy = 1;

    const string FOLDER_NAME = "Flamethrower/";

    public override void ExecuteBonus()
    {
        base.ExecuteBonus();
        grid = GridA.Instance;
        grid.currState = GameState.wait;

        firstPos = new Vector3(8, 0, -5);
        currPos = firstPos;
        target = new Vector3(-1, 0, -5);

        if (leadFirePrefab == null)
        {
            leadFirePrefab = Resources.Load<GameObject>(RESOURCES_FOLDER + FOLDER_NAME + "Fire stream");
            shineParticles = Resources.Load<GameObject>(RESOURCES_FOLDER + FOLDER_NAME + "Shine Effect");
            fireClip = Resources.Load<AudioClip>(RESOURCES_FOLDER + FOLDER_NAME + "sfx_boost_fire");
        }
        leadFire = Instantiate(leadFirePrefab, new Vector3(firstPos.x, firstPos.y + 0.5f, firstPos.z), Quaternion.Euler(0, 0, -90));
        shinePartVFX = Instantiate(shineParticles, new Vector3(firstPos.x, firstPos.y + 0.5f, firstPos.z), Quaternion.identity);
        boostActivated = true;
        StartCoroutine(LaunchLine());

        audioSource.PlayOneShot(fireClip); //play sound on start
    }

    IEnumerator LaunchLine()
    {
        float totalDist = Vector3.Distance(currPos, target);
        float distBeforeAcceleration = totalDist * 0.2f;
        float lowSpeed = lineSpeed * 0.3f;
        float highSpeed = lineSpeed;
        StartCoroutine(DestroyBlocksInRow(0, 7, 0));
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
        leadFire.transform.position = Vector3.MoveTowards(currPos, target, speed * Time.deltaTime);
        shinePartVFX.transform.position = Vector3.MoveTowards(shinePartVFX.transform.position, target, speed * Time.deltaTime);
        currPos = Vector3.MoveTowards(currPos, target, speed * Time.deltaTime);
        //if (Mathf.Abs(currPos.x - (int)currPos.x) <= 0.2 && (int)currPos.x >= 0 && (int)currPos.x < 8)
            //grid.DestroyBlockAtPosition((int)currPos.x, (int)currPos.y);

        if (currPos == target)
        {
            if (firstPos.y == linesToDestroy - 1)
            {
                boostActivated = false;
                Destroy(leadFire);
                Destroy(shinePartVFX);
                StartCoroutine(grid.MoveBoxesDown());
                grid.currState = GameState.move;
            }
            else
            {
                firstPos.y++;
                leadFire.transform.position = new Vector3(leadFire.transform.position.x, leadFire.transform.position.y + 1f, firstPos.z);
                shinePartVFX.transform.position = new Vector3(shinePartVFX.transform.position.x, shinePartVFX.transform.position.y + 1, firstPos.z);
                leadFire.transform.rotation = Quaternion.Euler(0, leadFire.transform.rotation.eulerAngles.y + 180, -90);
                shinePartVFX.transform.rotation = Quaternion.Euler(0, shinePartVFX.transform.rotation.eulerAngles.y + 180, 0);
                target = firstPos;

                currPos.y++;
                firstPos = currPos;
                CreateLine(speed);
                int startX = Mathf.Clamp((int)currPos.x, 0, 7);
                int targetX = Mathf.Clamp((int)target.x, 0, 7);
                StartCoroutine(DestroyBlocksInRow((int)currPos.y, startX, targetX));
                leadFire.GetComponent<Animator>().Play("Fire stream", -1, 0);
                audioSource.PlayOneShot(fireClip); //play sound on new line
            }
        }
    }
    IEnumerator DestroyBlocksInRow(int row, int startX, int targetX)
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