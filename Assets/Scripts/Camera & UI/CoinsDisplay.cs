using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinsDisplay : MonoBehaviour
{
    [SerializeField] int coinsToAdd = 1;
    [SerializeField] AudioClip coinSFX;
    Text coinsText;
    private int coins;
    private int currChance = 20;
    private int coinsAtStartOfLevel;
    private float coinSoundAtATime = 0.1f;
    private float lastTimeCoinSoundPlayed = 0;

    AudioSource audioSource;

    public static CoinsDisplay Instance;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        coins = SaveSystem.LoadCoinsAmount();
        coinsAtStartOfLevel = coins;
        coinsText = GetComponent<Text>();
        coinsText.text = coins.ToString();
        audioSource = GetComponent<AudioSource>();
    }

    void UpdateText()
    {
        if (coinsText == null) return;
        coinsText.text = coins.ToString();
        SaveSystem.SaveCoinsAmount(coins);
    }

    public void RandomizeCoin()
    {
        int randNumber = Random.Range(0, 100);
        if (randNumber < currChance)
        {
            if (Time.time > lastTimeCoinSoundPlayed + coinSoundAtATime)
            {
                if (audioSource)
                    audioSource.PlayOneShot(coinSFX);
                lastTimeCoinSoundPlayed = Time.time;
            }
            AddCoins();
            UpdateText();
        }
    }

    public void IncreaseCoinDropChance(int amount)
    {
        currChance += amount;
    }

    public void AddCoinsAmount(int value)
    {
        coins += value;
        UpdateText();
    }

    private void AddCoins()
    {
        coins += coinsToAdd;
    }
    public void DecreaseCoins(int amount)
    {
        coins -= amount;
        UpdateText();
    }

    public int GetCoins()
    {
        return coins;
    }
    public int EarnedCoinsSinceStart()
    {
        return coins - coinsAtStartOfLevel;
    }
}