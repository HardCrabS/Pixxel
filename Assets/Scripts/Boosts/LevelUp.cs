using UnityEngine;

public class LevelUp : MonoBehaviour 
{
    [SerializeField] BonusManager bonusManager;
    [SerializeField] EquipButton equipButton;
    [SerializeField] GameObject upgradePart;
    [SerializeField] AudioClip boostUpgradeSFX;
    [SerializeField] AudioClip boostUpgradeError;

    public Boost boostInfo;
    public BoostBase bonus;

    AudioSource audioSource;

    public static LevelUp Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void UpgradeBoost()
    {
        if (boostInfo == null)//locked boost has been clicked
        {
            audioSource.PlayOneShot(boostUpgradeError);
            return;
        }
        int level = GameData.gameData.GetBoostLevel(boostInfo.id);
        if (level < 10 && CoinsDisplay.Instance.GetCoins() >= boostInfo.GetUpgradeCost(level))
        {
            CoinsDisplay.Instance.DecreaseCoins(boostInfo.GetUpgradeCost(level));
            GameData.gameData.saveData.boostLevels[boostInfo.id]++;
            GameData.Save();
            level++;
            ClickOnBoost.Instance.ChangeBoostText(boostInfo);
            bonusManager.UpdateBoostSprites(boostInfo, level);
            equipButton.UpdateEquipedBoosts(boostInfo);

            var part = Instantiate(upgradePart, bonus.transform);
            part.transform.localScale *= 2;
            Destroy(part, 1);

            audioSource.PlayOneShot(boostUpgradeSFX);
        }
        else
            audioSource.PlayOneShot(boostUpgradeError);
    }
}
