using System.Collections;
using UnityEngine;

public class FlashField : BoostBase
{
    int blockToMakeFiredUp = 5;
    GameObject lightning;
    AudioClip flashStart, flashStrike;
    GridA grid;

    public override void ExecuteBonus()
    {
        if (lightning == null)
        {
            lightning = Resources.Load<GameObject>(RESOURCES_FOLDER + "Flash Field/Lightning");
            flashStart = Resources.Load<AudioClip>(RESOURCES_FOLDER + "Flash Field/sfx_boost_flashf");
            flashStrike = Resources.Load<AudioClip>(RESOURCES_FOLDER + "Flash Field/sfx_boost_flashf1");
        }
        audioSource.PlayOneShot(flashStart);
        StartCoroutine(MakeAllFiredUp());
    }

    IEnumerator MakeAllFiredUp()
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < blockToMakeFiredUp; i++)
        {
            int randX = Random.Range(0, grid.width);
            int randY = Random.Range(0, grid.hight);
            if (grid.allBoxes[randX, randY] != null)
                MakeBlockFiredUp(grid.allBoxes[randX, randY].GetComponent<Box>(), new Vector2(randX, randY));
            yield return new WaitForSeconds(1f);
        }
        finished = true;
    }

    void MakeBlockFiredUp(Box box, Vector2 pos)
    {
        audioSource.PlayOneShot(flashStrike);
        GameObject go = Instantiate(lightning, pos, transform.rotation);
        StartCoroutine(Camera.main.GetComponent<CameraShake>().Shake(0.1f, 0.2f));
        Destroy(go, 0.4f);
        StartCoroutine(grid.FiredUpBlock(box));
    }
    public override void SetBoostLevel(int lvl)
    {
        base.SetBoostLevel(lvl);
        grid = GridA.Instance;
        if(lvl <= 3)
        {
            blockToMakeFiredUp = (int)(grid.width * grid.hight * 0.05f);
        }
        else if(lvl <= 6)
        {
            blockToMakeFiredUp = (int)(grid.width * grid.hight * 0.1f);
        }
        else if(lvl <= 9)
        {
            blockToMakeFiredUp = (int)(grid.width * grid.hight * 0.15f);
        }
        else
        {
            blockToMakeFiredUp = (int)(grid.width * grid.hight * 0.2f);
        }
    }
}