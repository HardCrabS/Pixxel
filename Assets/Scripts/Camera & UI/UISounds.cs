using UnityEngine;

public class UISounds : MonoBehaviour
{
    AudioSource audioSource;
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void PlayClip(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    public void PlayClipBetweenScenes(AudioClip clip)
    {
        if(AudioController.Instance)
        {
            AudioController.Instance.PlayClipOneShot(clip);
        }
    }
}