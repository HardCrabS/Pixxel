using UnityEngine;
using UnityEngine.UI;

public enum LevelReward
{
    World,
    Trinket,
    Boost,
    BoostSlot,
    Title,
    Banner,
    Avatar,
    CardSet
}

public class RewardForLevel : MonoBehaviour
{
    [SerializeField] GameObject rewardEarned;
    [SerializeField] GameObject fireworksVFX;

    [SerializeField] GameObject rewardPanel;
    [SerializeField] Text rewardText;
    [SerializeField] Image rewardImage;

    [SerializeField] Reward[] rewards;

    public static RewardForLevel Instance;

    bool levelUped = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        int currPlayerLevel = GameData.gameData.saveData.currentLevel;
    }

    public void CheckForReward(int levelAchieved)
    {
        rewardEarned.GetComponent<Text>().text = "New Level!";
        rewards[levelAchieved - 1].ApplyReward(); //-1 cuz array elements start with 0 index

        SetRewardPanel(rewards[levelAchieved - 1].rewards);

        rewardEarned.GetComponent<Animation>().Play();
        LaunchFireworks();
    }

    public RewardTemplate GetReward(int level)
    {
        return rewards[level - 1].rewards[0];
    }

    void LaunchFireworks()
    {
        if (fireworksVFX != null)
        {
            Vector2 bottomLeft = Camera.main.ScreenToWorldPoint(Vector3.zero);
            Vector2 spawnPos = new Vector2(bottomLeft.x + Camera.main.orthographicSize / 2, bottomLeft.y);
            GameObject go = Instantiate(fireworksVFX, spawnPos, fireworksVFX.transform.rotation);
            Destroy(go, 8f);
        }
    }

    void SetRewardPanel(RewardTemplate[] rewards)
    {
        rewardText.text = "Your rewards:\n";
        for (int i = 0; i < rewards.Length; i++)
        {
            rewardText.text += rewards[i].reward + ": " + rewards[i].name + "\n\n";
            var rewSprite = rewards[i].GetRewardSprite();
            if (rewSprite != null)
                rewardImage.sprite = rewSprite;
        }
        levelUped = true;
    }
    public void CheckForLevelUpReward() //called at the end of the game
    {
        if(levelUped)
        {
            rewardPanel.SetActive(true);
        }
    }
    public int GetRankFromRewards(LevelReward levelReward, string id)
    {
        for (int i = 0; i < rewards.Length; i++)
        {
            var rew = rewards[i].rewards;
            for (int j = 0; j < rew.Length; j++)
            {
                if(rew[j].reward == levelReward && rew[j].GetRewardId() == id)
                {
                    return i + 1;
                }
            }
        }
        return -1;
    }
}