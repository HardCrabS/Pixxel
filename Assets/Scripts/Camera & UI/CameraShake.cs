using System.Collections;
using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour 
{
    public void ShakeCam(float duration, float magnitude, int vibrato = 30)
    {
        transform.DOShakePosition(duration, magnitude, vibrato);
    }
	/*public IEnumerator Shake(float duration, float magnitude)
    {
        Vector2 originalPos = transform.localPosition;

        float elapsed = 0.0f;

        while(elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector2(x, y);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
    }*/
}