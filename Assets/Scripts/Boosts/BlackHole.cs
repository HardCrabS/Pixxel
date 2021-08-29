    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : BoostBase
{
    int blocksToDestroy;
    float minBlockSpeed = 0.8f;

    GameObject barrierPrefab, blackHolePrefab;
    GameObject barrier, blackHole;  //clones
    AudioClip holeSFX;

    GridA grid;

    const string FOLDER_NAME = "Black Hole/";
    public override void ExecuteBonus()
    {
        base.ExecuteBonus();
        GetResources();

        SpawnBlackHole();
        StartCoroutine(SuckAllBlocks());
    }

    void GetResources()
    {
        if (barrierPrefab == null)
        {
            barrierPrefab = Resources.Load<GameObject>(RESOURCES_FOLDER + FOLDER_NAME + "Barrier");
            blackHolePrefab = Resources.Load<GameObject>(RESOURCES_FOLDER + FOLDER_NAME + "Black Hole");
            holeSFX = Resources.Load<AudioClip>(RESOURCES_FOLDER + "Black Hole/sfx_boost_blackhole");

        }
    }
    IEnumerator BarrierFadeAlpha(float targetAlpha, float alphaSpeed = 1f)
    {
        var sprite = barrier.GetComponent<SpriteRenderer>();
        float t = 0;
        while (t < 1)
        {
            t += alphaSpeed * Time.deltaTime;
            float alpha = Mathf.Lerp(sprite.color.a, targetAlpha, t);
            sprite.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }
    }
    void SpawnBlackHole()
    {
        audioSource.PlayOneShot(holeSFX); //play audio sfx
        GridA.Instance.currState = GameState.wait; // pause movement of blocks

        Transform centerBoxPanel = grid.boxesCenterPanel;
        barrier = Instantiate(barrierPrefab, centerBoxPanel.position, transform.rotation, centerBoxPanel);
        StartCoroutine(BarrierFadeAlpha(1));
        blackHole = Instantiate(blackHolePrefab, centerBoxPanel.position, transform.rotation, centerBoxPanel);

        float animTime = blackHole.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        Destroy(blackHole, animTime);
    }

    IEnumerator SuckAllBlocks()
    {
        for (int i = 0; i < blocksToDestroy; i++)
        {
            int randX = Random.Range(0, grid.width);
            int randY = Random.Range(0, grid.hight);
            GameObject box = grid.allBoxes[randX, randY];
            if (box != null)
            {
                box.GetComponent<Box>().enabled = false;
                Animator animator = box.GetComponent<Animator>();
                if (animator)
                    animator.enabled = false;
                StartCoroutine(SuckBlockInHole(box.transform));
                grid.allBoxes[randX, randY] = null;
                grid.bombTiles[randX, randY] = null;
            }
        }
        float animTime = blackHole.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animTime + 0.5f);
        yield return StartCoroutine(BarrierFadeAlpha(0));
        Destroy(barrier);
        StartCoroutine(grid.MoveBoxesDown());
        GridA.Instance.currState = GameState.move; //reallow game movement
        finished = true;
    }

    IEnumerator SuckBlockInHole(Transform box)
    {
        float moveSpeed = Random.Range(minBlockSpeed, minBlockSpeed * 2);
        float rotSpeed = Random.Range(-5f, 5f);
        Vector3 targetPos = grid.boxesCenterPanel.position;
        float t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime * moveSpeed;
            box.position = Vector3.Lerp(box.position, targetPos, t);
            box.localScale = Vector3.Lerp(box.localScale, Vector3.zero, t * 0.1f);
            box.Rotate(new Vector3(0, 0, rotSpeed));
            yield return null;
        }
        Destroy(box.gameObject);
    }

    public override void SetBoostLevel(int lvl)
    {
        base.SetBoostLevel(lvl);

        grid = GridA.Instance;
        int allBlocksAmount = grid.width * grid.hight;
        float percentToDestroy;
        if (lvl <= 3)
        {
            percentToDestroy = 0.15f;
        }
        else if (lvl <= 6)
        {
            percentToDestroy = 0.30f;
        }
        else if (lvl <= 9)
        {
            percentToDestroy = 0.50f;
        }
        else
        {
            percentToDestroy = 0.70f;
        }

        blocksToDestroy = (int)(allBlocksAmount * percentToDestroy);
    }
}