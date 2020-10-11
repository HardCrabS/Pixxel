using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BombTile : MonoBehaviour
{
    [SerializeField] int counter = 3;
    [SerializeField] GameObject explosionVFX;
    Text timerText;

    void Start()
    {
        timerText = GetComponentInChildren<Text>();
        timerText.text = counter.ToString();

        EndGameManager.Instance.onMatchedBlock += DecreaseBombCounter;
        var box = GetComponent<Box>();
    }

    IEnumerator ExplodeBomb()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        GetComponent<SpriteRenderer>().enabled = false;

        StartCoroutine(LivesManager.Instance.DecreaseHeart());
        GameObject go = Instantiate(explosionVFX, transform.position, transform.rotation);
        Destroy(go, 0.5f);
        ShakeCamera();
        yield return new WaitForSeconds(0.3f);

        Box box = GetComponent<Box>();
        int row = box.row, column = box.column;
        Destroy(gameObject);
        GridA.Instance.allBoxes[row, column] = null;
        GridA.Instance.bombTiles[row, column] = null;
    }
    void ShakeCamera()
    {
        var camShake = Camera.main.GetComponent<CameraShake>();
        StartCoroutine(camShake.Shake(0.1f, 0.05f));
    }
    public void DecreaseBombCounter()
    {
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
        if (timerText != null)
            timerText.text = counter.ToString();
    }
    public void DeleteBombByMatch()
    {
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        EndGameManager.Instance.onMatchedBlock -= DecreaseBombCounter;
    }
}