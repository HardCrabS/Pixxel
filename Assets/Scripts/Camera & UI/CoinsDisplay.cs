using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CoinsDisplay : MonoBehaviour
{
    [SerializeField] int coinsPerBlock = 1;
    [Range(0, 100f)] [SerializeField] float startCoinDropChance = 20;
    [SerializeField] float chanceIncremPerRank = 0.2f;

    [SerializeField] AudioClip coinSFX;
    [SerializeField] GameObject coinPrefab;

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
        coins = GameData.gameData.saveData.coins;
        coinsAtStartOfLevel = coins;
        currChance = startCoinDropChance;
        coinsText.text = coins.ToString();
        audioSource = GetComponent<AudioSource>();

        if (AudioController.Instance && audioSource)
        {
            audioSource.volume = AudioController.Instance.SFXVolume * 0.5f;
            AudioController.Instance.onSFXVolumeChange += ChangeSFXVolume;
        }
        UpdateText();
    }

    void UpdateText(bool punch = false)
    {
        if (coinsText == null) return;

        if (punch)
        {
            coinsText.transform.DOPunchScale(coinsText.transform.localScale / 3, 0.5f, 0, 0f);
        }
        coinsText.text = coins.ToString();

        GameData.gameData.saveData.coins = coins;
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
    public void AddCoinsWithCoinAnim(int amount, RectTransform parent)
    {
        StartCoroutine(MoveCoin(amount, parent));
    }
    IEnumerator MoveCoin(int amount, RectTransform parent)
    {
        var coin = Instantiate(coinPrefab, parent.position, Quaternion.identity, parent);
        Vector2 coinsTextPos = coinsText.rectTransform.position;

        coin.transform.DOMove(coinsTextPos, 1);
        yield return coin.transform.DOScale(0, 1).WaitForCompletion();

        Destroy(coin);
        AddCoinsAmount(amount, punch: true);
    }
    public void IncreaseCoinDropChance(int amount = 0)
    {
        currChance += amount == 0 ? chanceIncremPerRank : amount;
    }
    public void AddCoinsAmount(int value, bool punch = false)
    {
        coins += value;
        UpdateText(punch);
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