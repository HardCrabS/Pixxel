using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombTile : MonoBehaviour
{
    [SerializeField] int counter = 3;
    [SerializeField] GameObject explosionVFX;
    Text timerText;
    EndGameManager endGameManager;
    void Start()
    {
        timerText = GetComponentInChildren<Text>();
        timerText.text = counter.ToString();
        endGameManager = FindObjectOfType<EndGameManager>();
        endGameManager.onMatchedBlock += DecreaseBombCounter;
    }

    IEnumerator ExplodeBomb()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        GetComponent<SpriteRenderer>().enabled = false;
        if (FindObjectOfType<LivesManager>().DecreaseHeart())
        {
            GameObject go = Instantiate(explosionVFX, transform.position, transform.rotation);
            Destroy(go, 0.5f);
            yield return new WaitForSeconds(0.3f);
        }
        Destroy(gameObject);
    }

    public void DecreaseBombCounter()
    {
        counter--;
        if (counter <= 0)
        {
            StartCoroutine(ExplodeBomb());
        }
        if (timerText != null)
            timerText.text = counter.ToString();
    }

    public void DeleteBombByMatch()
    {
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        endGameManager.onMatchedBlock -= DecreaseBombCounter;
    }
}