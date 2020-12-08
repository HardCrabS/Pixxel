using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombRemover : MonoBehaviour
{
    float bombRemoveChance = 5;
    void Start()
    {
        EndGameManager.Instance.onMatchedBlock += RemoveBomb;
    }

    void RemoveBomb()
    {
        int chance = Random.Range(0, 100);
        if (chance <= bombRemoveChance)
        {
            var bombs = GridA.Instance.bombTiles;
            for (int y = 0; y < bombs.GetLength(0); y++)
            {
                for (int x = 0; x < bombs.GetLength(1); x++)
                {
                    if (bombs[x, y] != null)
                    {
                        Vector2 pos = bombs[x, y].transform.position;
                        bombs[x, y].DeleteBombByMatch();
                        bombs[x, y] = null;
                        GridA.Instance.allBoxes[x, y] = null;
                        GridA.Instance.SpawnBlockParticles(pos);
                        return;
                    }
                }
            }
        }
    }
}
