using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    [SerializeField] int hitPoints;
    private GoalManager goalManager;

    void Start()
    {
        goalManager = FindObjectOfType<GoalManager>();
    }

    public void TakeDamage()
    {
        hitPoints -= 1;
        if(hitPoints <= 0)
        {
            if(goalManager != null)
            {
                goalManager.CompareGoal(gameObject.tag);
                goalManager.UpdateGoals();
            }
            Destroy(gameObject);
        }
    }
}
