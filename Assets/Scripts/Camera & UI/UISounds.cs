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
    public void PlaySound(AudioClip soundToPlay)
    {
        AudioController.Instance.PlayNewClip(soundToPlay);
    }
}
