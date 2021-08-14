using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GoldenRock : MonoBehaviour
{
    public GameObject paticleCoin;
    public AudioClip tappedClip;

    public UnityEvent OnGoldenRockClicked = new UnityEvent();

    public void SetValues(GameObject goldenRockPrefab)
    {
        var goldenRock = goldenRockPrefab.GetComponent<GoldenRock>();
        paticleCoin = goldenRock.paticleCoin;
        tappedClip = goldenRock.tappedClip;
        GetComponent<SpriteRenderer>().sprite = goldenRockPrefab.GetComponent<SpriteRenderer>().sprite;
    }
    public void DetonateGoldenRock()
    {
        GameObject audioGO = new GameObject();
        audioGO.AddComponent<AudioSource>().PlayOneShot(tappedClip);
        Destroy(audioGO, tappedClip.length);

        GameObject go = Instantiate(paticleCoin, transform.position, transform.rotation);
        Destroy(go, 0.5f);

        GetComponent<SpriteRenderer>().enabled = false;

        int coins = Random.Range(2, 5);
        CoinsDisplay.Instance.AddCoinsAmount(coins);
    }
    void OnMouseDown()
    {
        OnGoldenRockClicked.Invoke();
    }
}