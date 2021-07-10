using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpCrystal : BoostBase
{
    int blockToMakeWarped = 1;
    GameObject crystalAnimation;
    AudioClip crystalSFX;
    GridA grid;

    List<GameObject> warpedBoxes;

    public override void ExecuteBonus()
    {
        warpedBoxes = new List<GameObject>();
        if (crystalAnimation == null)
        {
            crystalAnimation = Resources.Load<GameObject>(RESOURCES_FOLDER + "Warp Crystal/Crystal Animation");
            crystalSFX = Resources.Load<AudioClip>(RESOURCES_FOLDER + "Warp Crystal/sfx-boost-warpCrystal");
        }
        StartCoroutine(MakeAllWarped());
    }

    IEnumerator MakeAllWarped()
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < blockToMakeWarped; i++)
        {
            GameObject box;
            //keep finding random different blocks
            do
            {
                int randX = Random.Range(0, grid.width);
                int randY = Random.Range(0, grid.hight);
                box = grid.allBoxes[randX, randY];
            }
            while (box == null || warpedBoxes.Contains(box));
            warpedBoxes.Add(box);

            StartCoroutine(MakeBlockWarped(box.GetComponent<Box>(), box.transform.position));
            yield return new WaitForSeconds(1f);
        }
        MatchFinder.Instance.FindAllMatches();
        grid.DestroyAllMatches();
        finished = true;
    }

    IEnumerator MakeBlockWarped(Box box, Vector2 pos)
    {
        audioSource.PlayOneShot(crystalSFX);
        GameObject go = Instantiate(crystalAnimation, pos, transform.rotation);
        Camera.main.GetComponent<CameraShake>().ShakeCam(0.1f, 0.2f);

        float animationLength = go.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength);
        Destroy(go);
        grid.SetBlockWarped(box);
    }
    public override void SetBoostLevel(int lvl)
    {
        base.SetBoostLevel(lvl);
        grid = GridA.Instance;
        if (lvl <= 3)
        {
            blockToMakeWarped = 1;
        }
        else if (lvl <= 6)
        {
            blockToMakeWarped = 2;
        }
        else if (lvl <= 9)
        {
            blockToMakeWarped = 3;
        }
        else
        {
            blockToMakeWarped = 5;
        }
    }
}