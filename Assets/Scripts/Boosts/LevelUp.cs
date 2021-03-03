using UnityEngine;

public class LevelUp : MonoBehaviour 
{
    [SerializeField] BonusManager bonusManager;
    [SerializeField] EquipButton equipButton;
    [SerializeField] AudioClip boostUpgradeSFX;
    [SerializeField] AudioClip boostUpgradeError;

    public Boost boostInfo;
    public BoostBase bonus;

    AudioSource audioSource;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void UpgradeBoost()
    {
        int level = GameData.gameData.GetBoostLevel(boostInfo.id);
        if (level < 10 && CoinsDisplay.Instance.GetCoins() >= boostInfo.GetUpgradeCost(level))
        {
            CoinsDisplay.Instance.DecreaseCoins(boostInfo.GetUpgradeCost(level));
            GameData.gameData.saveData.boostLevels[boostInfo.id]++;
            GameData.Save();
            level++;
            ClickOnBoost.Instance.ChangeBoostText(boostInfo);
            bonusManager.UpdateBoostSprites(boostInfo.id, level);
            equipButton.UpdateEquipedBoosts(boostInfo);

            audioSource.PlayOneShot(boostUpgradeSFX);
        }
        else
            audioSource.PlayOneShot(boostUpgradeError);
    }
}
