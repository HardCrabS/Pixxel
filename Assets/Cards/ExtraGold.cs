using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraGold : MonoBehaviour 
{
    [SerializeField] int extraGoldAmount = 25;
	// Use this for initialization
	void Start () 
    {
        CoinsDisplay.Instance.AddCoinsAmount(extraGoldAmount);
	}
}
