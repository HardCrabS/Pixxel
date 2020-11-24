using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magician : MonoBehaviour
{
    LivesManager livesManager;
    // Use this for initialization
    void Start()
    {
        livesManager = FindObjectOfType<LivesManager>();
        if (livesManager != null)
            livesManager.savePlayer += SavePlayerFromBombs;
    }

    void SavePlayerFromBombs()
    {
        ShowCard();
        GridA grid = GridA.Instance;

        for (int i = 0; i < grid.width; i++)
        {
            for (int j = 0; j < grid.hight; j++)
            {
                if (grid.allBoxes[i, j] != null && grid.allBoxes[i, j].GetComponent<BombTile>())
                {
                    Destroy(grid.allBoxes[i, j]);
                    grid.allBoxes[i, j] = null;
                }
            }
        }
        StartCoroutine(grid.MoveBoxesDown());

        livesManager.savePlayer -= SavePlayerFromBombs;
        Time.timeScale = 1;
    }

    void ShowCard()
    {
        GameObject card = Resources.Load<GameObject>("Sprites/Cards/Magician game");
        GameObject go = Instantiate(card, card.transform.position, card.transform.rotation);
        Destroy(go, 5f);
    }
}