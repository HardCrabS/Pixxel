using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivesManager : MonoBehaviour
{
    [SerializeField] AudioClip heartLost;
    [SerializeField] AudioClip heartGained;
    [SerializeField] AudioClip lowHealth;

    [SerializeField] Image[] hearts;
    [SerializeField] Color lostHeartColor;

    public delegate void ZeroHearts();
    public event ZeroHearts savePlayer;

    int totalLives;
    AudioSource audioSource;

    public static LivesManager Instance;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        totalLives = hearts.Length;
        audioSource = GetComponent<AudioSource>();
    }

#if UNITY_EDITOR
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L)) //lose by button press(for testing) 
        {
            for (int i = 0; i < 3; i++)
            {
                hearts[i].color = lostHeartColor;
            }
            EndGameManager.Instance.GameOver();
            Destroy(this);
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            StartCoroutine(DecreaseHeart());
        }
    }
#endif
    public IEnumerator DecreaseHeart()
    {
        if (totalLives == 1)
        {
            if (savePlayer != null)
            {
                yield return null;
                savePlayer?.Invoke();
                MakeAllHeartsActive();
                yield break;
            }
            else
            {
                EndGameManager.Instance.GameOver();
                Destroy(this);
            }
        }
        ShakeCamera(0.1f, 0.3f, 30);
        audioSource.PlayOneShot(heartLost);
        Glitcher.Instance.GlitchOut();
        totalLives--;
        if (totalLives < 3 && totalLives >= 0)
            hearts[totalLives].color = lostHeartColor;
        if(totalLives == 1)
        {
            StartCoroutine(PlayLowHealthSFX());
        }
    }
    void ShakeCamera(float duration, float strength, int vibrato)
    {
        var camShake = Camera.main.GetComponent<CameraShake>();
        camShake.ShakeCam(duration, strength, vibrato);
    }
    void MakeAllHeartsActive()
    {
        audioSource.PlayOneShot(heartGained);
        totalLives = 3;
        for (int i = 0; i < 3; i++)
        {
            hearts[i].color = Color.white;
        }
    }
    IEnumerator PlayLowHealthSFX()
    {
        yield return new WaitForSeconds(0.2f);
        audioSource.PlayOneShot(lowHealth);
        yield return new WaitForSeconds(lowHealth.length);
        audioSource.PlayOneShot(lowHealth);
    }
}