using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

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
    [SerializeField] GameObject fireworksVFX;
    [SerializeField] AudioClip fireworksSFX;
    [SerializeField] GameObject itemAddedPart;
    [SerializeField] GameObject rankUpFX;

    [SerializeField] Reward[] rewards;

    [Header("End Game Reward Panel")]
    [SerializeField] Slider xpSlider;
    [SerializeField] Text XPText;
    [SerializeField] Text coinsText;
    [SerializeField] Transform scrollContainer;

    [Header("Reward Sprites")]
    [SerializeField] GameObject chestPrefab;
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
        if (levelAchieved > rewards.Length) { return; }
        rewards[levelAchieved - 1].ApplyReward(); //-1 because array elements start with 0 index

        SetRewardPanel(rewards[levelAchieved - 1].rewards);

        LaunchFireworks();
    }

    public RewardTemplate GetReward(int level)
    {
        return level - 1 < rewards.Length ? rewards[level - 1].rewards[0] : null;
    }

    void LaunchFireworks()
    {
        if (fireworksVFX != null)
        {
            audioSource.PlayOneShot(fireworksSFX);
            Vector2 bottomLeft = Camera.main.ScreenToWorldPoint(Vector3.zero);
            Vector2 spawnPos = new Vector2(bottomLeft.x + Camera.main.orthographicSize / 2, bottomLeft.y);
            GameObject go = Instantiate(fireworksVFX, spawnPos, fireworksVFX.transform.rotation);
            Destroy(go, 8f);
        }
    }

    void SpawnRewardChest(RectTransform reward, RewardTemplate rewardInfo)
    {
        reward.localScale = Vector3.zero;
        var chest = Instantiate(chestPrefab, reward.position, Quaternion.identity, scrollContainer);
        chest.transform.SetAsFirstSibling();
        chest.transform.localScale = Vector3.one * 0.6f;

        chest.GetComponent<Button>().onClick.AddListener(()
            =>
        { StartCoroutine(ScaleRewardFromChest(chest.transform, reward, rewardInfo)); });
    }
    IEnumerator ScaleRewardFromChest(Transform chest, Transform reward, RewardTemplate rewardInfo)
    {
        SetRewardDescriptionText(rewardInfo);
        yield return new WaitForSeconds(0.5f);
        reward.DOScale(1, 0.5f);
        yield return new WaitForSeconds(1f);
        var image = chest.GetComponent<Image>();
        image.DOFade(0, 1);
        image.raycastTarget = false;
    }
    IEnumerator SpawnChests(List<RectTransform> spawnedRewards, RewardTemplate[] rewards)
    {
        //wait until layout group is active so it can position elements
        yield return new WaitUntil(() => scrollContainer.gameObject.activeInHierarchy);
        //wait for end of frame so all elements are positioned
        yield return new WaitForEndOfFrame();

        int i = 0;
        foreach (RectTransform item in spawnedRewards)
        {
            SpawnRewardChest(item, rewards[i]);
            i++;
        }
    }
    void SetRewardPanel(RewardTemplate[] rewards)
    {
        List<RectTransform> spawnedRewards = new List<RectTransform>();

        for (int i = 0; i < rewards.Length; i++)
        {
            GameObject go;
            if (rewards[i].reward == LevelReward.Title)
            {
                go = SpawnRewardImage(title, rewards[i]);
            }
            else if (rewards[i].reward == LevelReward.Banner)
            {
                go = SpawnRewardImage(banner, rewards[i]);
            }
            else
            {
                go = SpawnRewardImage(rewards[i].GetRewardSprite(), rewards[i]);
            }
            spawnedRewards.Add((RectTransform)go.transform);
        }

        if (scrollContainer.childCount <= 4)
            scrollContainer.GetComponent<HorizontalLayoutGroup>().spacing = 120;
        else
            scrollContainer.GetComponent<HorizontalLayoutGroup>().spacing = 50;

        StartCoroutine(SpawnChests(spawnedRewards, rewards));

        rewardDescrText.text = "<color=yellow>New " + rewards[0].reward
                + "</color>\n" + rewards[0].id + "\n\n";
    }
    public void SpawnParticles()   //show particles on rewards screen
    {
        foreach (Transform child in scrollContainer)
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
        else if (reward.reward == LevelReward.CardSet)
        {
            textWidth /= 5;     //5 cards in spritesheet
        }

        if (textHeight > 150 || textWidth > 150)
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
        //empty objects to create offset
        Instantiate(new GameObject("left offset").AddComponent<RectTransform>(), scrollContainer).transform.SetAsFirstSibling();
        Instantiate(new GameObject("right offset").AddComponent<RectTransform>(), scrollContainer).transform.SetAsLastSibling();

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

        xpSlider.maxValue = GameData.gameData.saveData.maxXPforLevelUp;
        xpSlider.value = initLevelXP;

        float xpDelta = levelDif > 0 ? xpSlider.maxValue - initLevelXP + finalXP : finalXP - initLevelXP;
        if (xpDelta == 0) yield break;  //no xp earned

        float startXPToFillFrom;
        float earnedXpUntillCurrLevel = 0;
        float durationToFill = 1f;
        for (int i = 0; i < levelDif; i++)//for every rank that player comleted
        {
            startXPToFillFrom = earnedXpUntillCurrLevel;
            earnedXpUntillCurrLevel += xpSlider.maxValue - initLevelXP;//update earned xp for rank

            StartCoroutine(AnimateXpText(startXPToFillFrom, earnedXpUntillCurrLevel, durationToFill)); //text to total earned xp on curr rank
            yield return xpSlider.DOValue(xpSlider.maxValue, durationToFill).WaitForCompletion();  //fill bar on max value

            //ShowRankUp(); 

            rankUpFX.SetActive(true); //SET IT ACTIVE

            xpSlider.value = 0;
            initLevelXP = 0;
        }
        startXPToFillFrom = earnedXpUntillCurrLevel;
        earnedXpUntillCurrLevel += finalXP;

        StartCoroutine(AnimateXpText(startXPToFillFrom, earnedXpUntillCurrLevel, durationToFill));
        xpSlider.DOValue(finalXP, durationToFill);
    }

    IEnumerator AnimateXpText(float start, float end, float time)
    {
        float deltaTime = time / (end - start);
        for (int xp = (int)start; xp < end; xp++)
        {
            XPText.text = "+" + xp + "xp";
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