using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenRock : MonoBehaviour
{
    int row;
    int column;
    public GameObject paticleCoin;
    public AudioClip tappedClip;

    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SetValues(int x, int y)
    {
        row = x;
        column = y;
    }
    void OnMouseDown()
    {
        audioSource.PlayOneShot(tappedClip);
        GameObject go = Instantiate(paticleCoin, transform.position, transform.rotation);
        Destroy(go, 0.5f);
        Destroy(gameObject, 0.5f);
        GetComponent<SpriteRenderer>().enabled = false;

        int coins = Random.Range(2, 5);
        CoinsDisplay.Instance.AddCoinsAmount(coins);

        GridA.Instance.SetBlankSpace(row, column, false);
        StartCoroutine(GridA.Instance.MoveBoxesDown());
    }
}