using UnityEngine;

public class UISounds : MonoBehaviour
{
    public void PlaySound(AudioClip soundToPlay)
    {
        AudioController.Instance.PlayNewClip(soundToPlay);
    }
}
