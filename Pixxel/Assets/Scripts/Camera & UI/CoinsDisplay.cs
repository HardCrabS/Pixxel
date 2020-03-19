using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinsDisplay : MonoBehaviour {
    [SerializeField] int coinsToAdd = 1;
    Text coinsText;
    private int coins;
    private int currChance = 20;
	void Start () {
        coins = SaveSystem.LoadCoinsAmount();
        coinsText = GetComponent<Text>();
        coinsText.text = coins.ToString();
    }

    void UpdateText()
    {
        coinsText.text = coins.ToString();
        SaveSystem.SaveCoinsAmount(this);
    }

    public void RandomizeCoin()
    {
        int randNumber = Random.Range(0, 100);
        if(randNumber < currChance)
        {
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
}
