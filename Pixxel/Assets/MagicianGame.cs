using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicianGame : MonoBehaviour 
{
    [SerializeField] GameObject VFX;
	
    public void SpawnParticles()
    {
        GameObject go = Instantiate(VFX, transform.position, transform.rotation);
        Destroy(go, 5);
    }
}
