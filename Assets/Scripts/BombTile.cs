using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BombTile : MonoBehaviour
{
    [SerializeField] int counter = 3;
    [SerializeField] float bombPulsateMultipliyer = 1.3f;
    [SerializeField] Color middleExplodeColor;
    [SerializeField] Color finalCountColor;
    [SerializeField] GameObject finalCountSprite;

    [SerializeField] GameObject explosionVFX;
    [SerializeField] AudioClip bombExploadeSFX;
    Text timerText;

    void Start()
    {
        timerText = GetComponentInChildren<Text>();
        timerText.text = counter.ToString();

        if (EndGameManager.Instance)
            EndGameManager.Instance.onMatchedBlock += DecreaseBombCounter;
    }

    IEnumerator ExplodeBomb()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        GetComponent<SpriteRenderer>().enabled = false;

        StartCoroutine(LivesManager.Instance.DecreaseHeart());
        GameObject go = Instantiate(explosionVFX, transform.position, transform.rotation);
        if (AudioController.Instance)
            AudioController.Instance.PlayNewClip(bombExploadeSFX);
        Destroy(go, 0.5f);
        yield return new WaitForSeconds(0.3f);

        Box box = GetComponent<Box>();
        int row = box.row, column = box.column;

        Destroy(gameObject);
        GridA.Instance.allBoxes[row, column] = null;
        GridA.Instance.bombTiles[row, column] = null;
    }
    public void DecreaseBombCounter()
    {
        if (LivesManager.Instance.BombCounterState == BombCounterState.ticking)
            StartCoroutine(DecreaseCounterDelayed());
    }
    IEnumerator DecreaseCounterDelayed()
    {
        counter--;
        yield return new WaitForSeconds(0.1f);
        if (counter <= 0)
        {
            StartCoroutine(ExplodeBomb());
        }
        ChangeTimerColor();
        if (timerText != null)
            timerText.text = counter.ToString();
        GetComponent<Animator>().speed *= bombPulsateMultipliyer;
    }
    void ChangeTimerColor()
    {
        if (counter == 2)
            timerText.color = middleExplodeColor;
        else if (counter == 1)
        {
            timerText.color = finalCountColor;
            finalCountSprite.SetActive(true);
        }
    }
    private void OnDestroy()
    {
        if (EndGameManager.Instance)
            EndGameManager.Instance.onMatchedBlock -= DecreaseBombCounter;
    }
}