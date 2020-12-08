using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    [SerializeField] int hitPoints;

    public void TakeDamage()
    {
        hitPoints -= 1;
        if (hitPoints <= 0)
        {
            GoalManager.Instance.CompareGoal(gameObject.tag);
            Destroy(gameObject);
        }
    }
}
