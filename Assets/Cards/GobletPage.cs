using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Effect: Boosts recharge 15% faster.

public class GobletPage : MonoBehaviour
{
    [SerializeField] int extraGoldPercent = 5;
    // Use this for initialization
    void Start()
    {
        CoinsDisplay.Instance.IncreaseCoinDropChance(extraGoldPercent);
    }
}
