using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Effect: Boosts recharge 25% faster.

public class GobletPage : MonoBehaviour
{
    [SerializeField] float rechargePercent = 1.25f; // sets recharge to 25%
    // Use this for initialization
    void Start()
    {
        BonusManager.Instance.MultiplyAllBoostsRecharge(rechargePercent);
    }
}
