using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRemover : MonoBehaviour
{
    float blockRemoveChance = 5;
    void Start()
    {
        EndGameManager.Instance.onMatchedBlock += RemoveBlock;
    }

    void RemoveBlock()
    {
        int chance = Random.Range(0, 100);
        if (chance <= blockRemoveChance)
        {
            var blocks = GridA.Instance.allBoxes;
            int randX = Random.Range(0, blocks.GetLength(0));
            int randY = Random.Range(0, blocks.GetLength(1));
            if (blocks[randX, randY] != null)
            {
                Vector2 pos = blocks[randX, randY].transform.position;
                Destroy(blocks[randX, randY]);
                blocks[randX, randY] = null;
                GridA.Instance.SpawnBlockParticles(pos);
                return;
            }
        }
    }
}
