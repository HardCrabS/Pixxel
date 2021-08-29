using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinsDisplay : MonoBehaviour
{
    [SerializeField] int coinsPerBlock = 1;
    [Range(0, 100f)] [SerializeField] float startCoinDropChance = 20;
    [SerializeField] float chanceIncremPerRank = 0.2f;

    [SerializeField] AudioClip coinSFX;

    [Header("References")]
    [SerializeField] Text coinsText;

    int coins;
    float currChance;
    int coinsAtStartOfLevel;
    float coinSoundAtATime = 0.1f;
    float lastTimeCoinSoundPlayed = 0;

    AudioSource audioSource;

    public static CoinsDisplay Instance;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        //coins = SaveSystem.LoadCoinsAmount();
        coins = GameData.gameData.saveData.coins;
        coinsAtStartOfLevel = coins;
        currChance = startCoinDropChance;
        //coinsText = GetComponent<Text>();
        coinsText.text = coins.ToString();
        audioSource = GetComponent<AudioSource>();

        if (AudioController.Instance && audioSource)
        {
            audioSource.volume = AudioController.Instance.SFXVolume * 0.5f;
            AudioController.Instance.onSFXVolumeChange += ChangeSFXVolume;
        }
        UpdateText();
    }

    void UpdateText()
    {
        if (coinsText == null) return;
        coinsText.text = coins.ToString();

        GameData.gameData.saveData.coins = coins;
        //SaveSystem.SaveCoinsAmount(coins);
    }

    public void RandomizeCoin()
    {
        float randChance = Random.Range(0, 100f);
        if (randChance < currChance)
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

    public void IncreaseCoinDropChance(int amount = 0)
    {
        currChance += amount == 0 ? chanceIncremPerRank : amount;
    }
    public void AddCoinsAmount(int value)
    {
        coins += value;
        UpdateText();
    }
    private void AddCoins()
    {
        coins += coinsPerBlock;
    }
    public void DecreaseCoins(int amount)
    {
        coins -= amount;
        UpdateText();
    }
    public int GetCoins() => coins;
    public int EarnedCoinsSinceStart() => coins - coinsAtStartOfLevel;
    void ChangeSFXVolume(float volume)
    {
        audioSource.volume = volume * 0.5f;
    }
}