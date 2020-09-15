using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivesManager : MonoBehaviour
{
    [SerializeField] Image[] hearts;
    [SerializeField] Color lostHeartColor;

    public delegate void ZeroHearts();
    public event ZeroHearts savePlayer;

    int totalLives;

    public static LivesManager Instance;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        totalLives = hearts.Length;
    }

    public IEnumerator DecreaseHeart()
    {
        if ((totalLives - 1) <= 0)
        {
            if (savePlayer != null)
            {
                yield return null;
                if (savePlayer != null)
                    savePlayer();
                totalLives = 3;
                MakeAllHeartsActive();
                yield break;
            }
            else
            {
                FindObjectOfType<EndGameManager>().GameOver();
            }
        }
        totalLives--;
        if (totalLives < 3 && totalLives >= 0)
            hearts[totalLives].color = lostHeartColor;
    }

    void MakeAllHeartsActive()
    {
        for (int i = 0; i < 3; i++)
        {
            hearts[i].color = Color.white;
        }
    }
}
