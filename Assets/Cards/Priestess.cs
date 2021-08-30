using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Effect: refills random boost.

public class Priestess : MonoBehaviour
{
    [SerializeField] int refillChance = 20;
    // Use this for initialization
    void Start()
    {
        EndGameManager.Instance.onMatchedBlock += RefillBoost;
    }

    void RefillBoost()
    {
        if(Random.Range(0, 100) < refillChance)
        {
            BonusManager.Instance.ActivateRandomButton();
        }
    }
}