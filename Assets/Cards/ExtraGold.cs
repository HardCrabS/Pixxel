using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraGold : MonoBehaviour 
{
    [SerializeField] int extraGoldPercent = 5;
	// Use this for initialization
	void Start () 
    {
        CoinsDisplay.Instance.IncreaseCoinDropChance(extraGoldPercent);
	}
}
