using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenRock : MonoBehaviour
{
    int column;
    int row;
    public GameObject paticleCoin;
    GridA _grid;
    CoinsDisplay _coinsDisplay;

    public GoldenRock(int x, int y)
    {
        column = x;
        row = y;
    }

    public void SetValues(int x, int y, GridA grid, CoinsDisplay coinsDisplay)
    {
        column = x;
        row = y;
        _grid = grid;
        _coinsDisplay = coinsDisplay;
    }
    void OnMouseDown()
    {
        GameObject go = Instantiate(paticleCoin, transform.position, transform.rotation);
        Destroy(go, 0.5f);
        Destroy(gameObject, 0.5f);
        GetComponent<SpriteRenderer>().enabled = false;

        int coins = Random.Range(2, 5);
        _coinsDisplay.AddCoinsAmount(coins);

        _grid.SetBlankSpace(column, row, false);
        StartCoroutine(_grid.MoveBoxesDown());
    }
}
