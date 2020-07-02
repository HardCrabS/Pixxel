using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CardSpawner : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
    {
        string cardTypeStr = GameData.gameData.saveData.cardType;

        if (!string.IsNullOrEmpty(cardTypeStr))
        {
            Type cardType = Type.GetType(cardTypeStr);
            gameObject.AddComponent(cardType);
        }
	}
}
