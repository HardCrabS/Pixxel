using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableSpawner : MonoBehaviour
{
    public GameObject destrPrefab;
    public Transform spawnPoint;
    
    public void SpawnObject()
    {
        Instantiate(destrPrefab, spawnPoint.position, transform.rotation);
    }
}
