using System.Collections;
using UnityEngine;

public class FlameThrower : BoostBase
{
    [SerializeField] float lineSpeed = 5;

    GameObject leadFirePrefab;
    GameObject shineParticles;
    AudioClip fireClip;
    GridA grid;
    Vector3 firstPos;
    Vector3 curr;
    Vector3 target;

    private bool boostActivated = false;
    private int linesToDestroy = 1;

    public override void ExecuteBonus()
    {
        grid = GridA.Instance;
        grid.currState = GameState.wait;

        firstPos = new Vector3(10, 0, -5);
        curr = firstPos;
        target = new Vector3(-2, 0, -5);

        if (leadFirePrefab == null)
        {
            leadFirePrefab = Resources.Load<GameObject>(resourcesFolder + "Flamethrower/Lead Fire");
            shineParticles = Resources.Load<GameObject>(resourcesFolder + "Flamethrower/Shine Effect");
            fireClip = Resources.Load<AudioClip>(resourcesFolder + "Flamethrower/sfx_boost_fire");
        }
        leadFirePrefab = Instantiate(leadFirePrefab, new Vector3(firstPos.x, firstPos.y + 0.5f, firstPos.z), Quaternion.Euler(0, 0, -90));
        shineParticles = Instantiate(shineParticles, new Vector3(firstPos.x, firstPos.y + 0.5f, firstPos.z), Quaternion.identity);
        boostActivated = true;
        StartCoroutine(LaunchLine());

        audioSource.PlayOneShot(fireClip); //play sound on start
    }

    IEnumerator LaunchLine()
    {
        while (boostActivated)
        {
            CreateLine();

            yield return null;
        }
    }

    void CreateLine()
    {
        leadFirePrefab.transform.position = Vector3.MoveTowards(curr, target, lineSpeed * Time.deltaTime);
        shineParticles.transform.position = Vector3.MoveTowards(shineParticles.transform.position, target, lineSpeed * Time.deltaTime);
        curr = Vector3.MoveTowards(curr, target, lineSpeed * Time.deltaTime);
        if (Mathf.Abs(curr.x - (int)curr.x) <= 0.2 && (int)curr.x >= 0 && (int)curr.x < 8)
            grid.FlameThrower((int)curr.x, (int)curr.y);

        if (curr == target)
        {
            if (firstPos.y == linesToDestroy - 1)
            {
                boostActivated = false;
                Destroy(leadFirePrefab);
                Destroy(shineParticles);
                StartCoroutine(grid.MoveBoxesDown());
                grid.currState = GameState.move;
            }
            else
            {
                firstPos.y++;
                leadFirePrefab.transform.position = new Vector3(leadFirePrefab.transform.position.x, leadFirePrefab.transform.position.y + 1f, firstPos.z);
                shineParticles.transform.position = new Vector3(shineParticles.transform.position.x, shineParticles.transform.position.y + 1, firstPos.z);
                leadFirePrefab.transform.rotation = Quaternion.Euler(0, leadFirePrefab.transform.rotation.eulerAngles.y + 180, -90);
                shineParticles.transform.rotation = Quaternion.Euler(0, shineParticles.transform.rotation.eulerAngles.y + 180, 0);
                target = firstPos;

                curr.y++;
                firstPos = curr;
                CreateLine();
                audioSource.PlayOneShot(fireClip); //play sound on new line
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