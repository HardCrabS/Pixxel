using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenRock : MonoBehaviour
{
    int column;
    int row;
    public GameObject paticleCoin;

    public GoldenRock(int x, int y)
    {
        column = x;
        row = y;
    }

    public void SetValues(int x, int y)
    {
        column = x;
        row = y;
    }
    void OnMouseDown()
    {
        GameObject go = Instantiate(paticleCoin, transform.position, transform.rotation);
        Destroy(go, 0.5f);
        Destroy(gameObject, 0.5f);
        GetComponent<SpriteRenderer>().enabled = false;

        int coins = Random.Range(2, 5);
        CoinsDisplay.Instance.AddCoinsAmount(coins);

        GridA.Instance.SetBlankSpace(column, row, false);
        StartCoroutine(GridA.Instance.MoveBoxesDown());
    }
}
