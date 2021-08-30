using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRemover : MonoBehaviour
{
    float blockRemoveChance = 20;
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
                GridA.Instance.DestroyBlockAtPosition(randX, randY);
                return;
            }
        }
    }
}
