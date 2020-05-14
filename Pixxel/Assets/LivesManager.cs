using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivesManager : MonoBehaviour 
{
    [SerializeField] Image[] hearts;
    [SerializeField] Color lostHeartColor;
    int totalLives;

	void Start () 
    {
        totalLives = hearts.Length;
	}
	
	public void DecreaseHeart()
    {
        totalLives--;
        hearts[totalLives].color = lostHeartColor;
        if(totalLives <= 0)
        {
            FindObjectOfType<EndGameManager>().GameOver();
        }
    }
}
