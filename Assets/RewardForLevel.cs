using System;
using System.Collections;
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
    CardSet
}

public class RewardForLevel : MonoBehaviour
{
    [SerializeField] GameObject rewardEarned;
    [SerializeField] GameObject fireworksVFX;
    [SerializeField] GameObject itemAddedPart;

    [SerializeField] Reward[] rewards;

    [Header("End Game Reward Panel")]
    [SerializeField] Slider xpSlider;
    [SerializeField] Text XPText;
    [SerializeField] Text coinsText;
    [SerializeField] Transform scrollContainer;

    [Header("Reward Sprites")]
    [SerializeField] Text rewardDescrText;
    [SerializeField] Transform rewardsContainer;
    [SerializeField] Sprite title, banner;

    [Header("Sounds")]
    [SerializeField] AudioClip xpBarSFX;
    [SerializeField] AudioClip coinsAppearSFX;

    AudioSource audioSource;

    public static RewardForLevel Instance;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void CheckForReward(int levelAchieved)
    {
        if(levelAchieved > rewards.Length) { return; }
        rewardEarned.GetComponent<Text>().text = "New Level!";
        rewards[levelAchieved - 1].ApplyReward(); //-1 because array elements start with 0 index

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
        for (int i = 0; i < rewards.Length; i++)
        {
            GameObject go;
            if (rewards[i].reward == LevelReward.Title)
            {
                go = SpawnRewardImage(title, rewards[i]);
            }
            else if(rewards[i].reward == LevelReward.Banner)
            {
                go = SpawnRewardImage(banner, rewards[i]);
            }
            else
            {
                go = SpawnRewardImage(rewards[i].GetRewardSprite(), rewards[i]);          
            }
            //scrollContainer.AddObject(go);
        }
        if (scrollContainer.childCount <= 3)
            scrollContainer.GetComponent<HorizontalLayoutGroup>().spacing = 120;
        else
            scrollContainer.GetComponent<HorizontalLayoutGroup>().spacing = 50;
        //scrollContainer.MoveToFirstObject();
        rewardDescrText.text = "<color=yellow>New " + rewards[0].reward
                + "</color>\n" + rewards[0].id + "\n\n";
    }
    public void SpawnParticles()   //show particles on rewards screen
    {
        foreach(Transform child in scrollContainer)
        {
            var part = Instantiate(itemAddedPart, child);
            part.transform.localScale *= 2;
            Destroy(part, 1);
        }
    }
    GameObject SpawnRewardImage(Sprite rewSprite, RewardTemplate reward)
    {
        var rew = new GameObject();
        rew.name = rewSprite.name;
        rew.transform.SetParent(rewardsContainer);

        var image = rew.AddComponent<Image>();
        image.sprite = rewSprite;
        rew.transform.localScale = Vector3.one;
        float textWidth = image.sprite.texture.width;
        float textHeight = image.sprite.texture.height;
        if (reward.reward == LevelReward.World)
        {
            textHeight -= 200;
            rew.AddComponent<Outline>().effectDistance = new Vector2(3, 3);
            rew.AddComponent<Shadow>().effectDistance = new Vector2(10, -8);
        }
        else if(reward.reward == LevelReward.CardSet)
        {
            textWidth /= 5;     //5 cards in spritesheet
        }

        if(textHeight > 150 || textWidth > 150)
        {
            float b = textHeight > textWidth ? 100 / textHeight : 100 / textWidth;
            image.rectTransform.sizeDelta = new Vector2(textWidth, textHeight) * b;
        }

        SetButton(reward, rew);

        return rew;
    }

    void SetButton(RewardTemplate reward, GameObject spawnedRew)
    {
        var button = spawnedRew.AddComponent<Button>();
        button.onClick.AddListener(delegate () { SetRewardDescriptionText(reward); });
        //button.interactable = false;
        var colors = button.colors;
        colors.disabledColor = Color.white;
        button.colors = colors;
    }

    void SetRewardDescriptionText(RewardTemplate reward)
    {
        rewardDescrText.text = "<color=yellow>New " + reward.reward + "</color>\n" + reward.id;
    }

    public void SetRewardScreenUI() //called at the end of the game and paused "retire" button 
    {
        StartCoroutine(ShowCoinsAfterXPBar());
    }
    public int GetRankFromRewards(LevelReward levelReward, string id)
    {
        for (int i = 0; i < rewards.Length; i++)
        {
            var rew = rewards[i].rewards;
            for (int j = 0; j < rew.Length; j++)
            {
                if (rew[j].reward == levelReward && rew[j].id == id)
                {
                    return i + 1;
                }
            }
        }
        return -1;
    }

    IEnumerator ShowCoinsAfterXPBar()
    {
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FillXPBar());
        SetEarnedCoinsText();
    }

    IEnumerator FillXPBar()
    {
        Tuple<int, float> initLevelInfo = LevelSlider.Instance.GetInitLevelInfo();
        int initLevel = initLevelInfo.Item1;
        float initLevelXP = initLevelInfo.Item2;

        int levelDif = LevelSlider.Instance.GetGameLevel() - initLevel;
        float finalXP = LevelSlider.Instance.GetLevelProgress();
        float totalXPEarned = 0;
        xpSlider.maxValue = GameData.gameData.saveData.maxXPforLevelUp;
        xpSlider.value = initLevelXP;

        float xpDelta = levelDif > 0 ? xpSlider.maxValue - initLevelXP + finalXP : finalXP - initLevelXP;
        if (xpDelta == 0) yield break;
        float deltaTime = xpBarSFX.length / xpDelta;
        audioSource.PlayOneShot(xpBarSFX);
        for (int i = 0; i < levelDif; i++)
        {
            for (float xp = initLevelXP; xp < xpSlider.maxValue; xp++) //fill to max for comleted levels
            {
                xpSlider.value = xp;
                XPText.text = "+" + totalXPEarned + "xp";
                totalXPEarned++;
                yield return new WaitForSeconds(deltaTime);
            }
            xpSlider.value = 0;
            initLevelXP = 0;
        }
        for (float xp = initLevelXP; xp < finalXP; xp++) //fill to current xp value
        {
            xpSlider.value = xp;
            XPText.text = "+" + totalXPEarned + "xp";
            totalXPEarned++;
            yield return new WaitForSeconds(deltaTime);
        }
    }

    void SetEarnedCoinsText()
    {
        int earnedCoins = CoinsDisplay.Instance.EarnedCoinsSinceStart();
        coinsText.text = "+" + earnedCoins + "G";
        coinsText.transform.parent.gameObject.SetActive(true); //activate block of coins info
        audioSource.PlayOneShot(coinsAppearSFX);
    }
}