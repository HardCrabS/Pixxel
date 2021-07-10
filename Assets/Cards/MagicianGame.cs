using UnityEngine;

public class MagicianGame : MonoBehaviour 
{
    [SerializeField] GameObject VFX;
	
    public void SpawnParticles()
    {
        GameObject go = Instantiate(VFX, transform.position, transform.rotation);
        Destroy(go, 5);
    }

    public void ShakeCamera()
    {
        var camShake = Camera.main.GetComponent<CameraShake>();
        camShake.ShakeCam(0.1f, 0.7f);
    }
}