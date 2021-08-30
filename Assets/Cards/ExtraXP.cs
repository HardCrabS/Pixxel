using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraXP : MonoBehaviour 
{
    float xpMultiplier = 1.10f;
	// Use this for initialization
	void Start () 
    {
        LevelSlider.Instance.SetXPMultiplier(xpMultiplier);	
	}
}
