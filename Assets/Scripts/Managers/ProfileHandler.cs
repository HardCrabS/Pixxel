using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProfileHandler : MonoBehaviour
{
    [SerializeField] LevelSlider levelSlider;
    [SerializeField] Text profileLevelText;

    [SerializeField] TextMeshProUGUI rewardNextLevel;
    [SerializeField] Image rewardImage;

    [SerializeField] Text playerName;
    [SerializeField] Image avatarImage;
    [SerializeField] Text titleText;
    [SerializeField] Image bannerImage;

    [SerializeField] Text changedName;

    [Header("Change Profile Info")]
    [SerializeField] UI_Screen collectionScreen;
    [SerializeField] Toggle trinketsToggle;
    [SerializeField] Toggle bannersToggle;

    string currTitle;

    public static ProfileHandler Instance;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        SetSlider();
        SetRewardNextLevel();
        SetProfileData();
    }

    public void SetProfileData()
    {
        User player = GameData.gameData.saveData.playerInfo;

        playerName.text = player.username;
        avatarImage.sprite = Resources.Load<Sprite>(player.spritePath);
        titleText.text = "\"" + player.titleText + "\"";
        currTitle = player.titleText;
        bannerImage.sprite = Resources.Load<Sprite>(player.bannerPath);
    }

    public void ChangeName()
    {
        string newName = changedName.text;
        playerName.text = newName;
        GameData.gameData.saveData.playerInfo.username = newName;
        GameData.Save();
        DatabaseManager.ChangeName(newName);
    }

    public void UpdateTitle(string title)
    {
        titleText.text = "\"" + title + "\"";
        currTitle = title;
    }
    public void UpdateBanner(Sprite banner)
    {
        bannerImage.sprite = banner;
    }
    public void ResetBanner()
    {
        string bannerPath = CollectionController.Instance.BANNERS_LOCATION + "DefaultBanner";
        GameData.gameData.ChangeBanner(bannerPath);
        bannerImage.sprite = Resources.Load<Sprite>(bannerPath);
    }
    public void UpdateAvatar(Sprite avatar)
    {
        avatarImage.sprite = avatar;
    }
    public void ResetAvatar()
    {
        string avatarPath = CollectionController.Instance.TRINKETS_LOCATION + "DefaultAvatar";
        GameData.gameData.ChangeAvatar(avatarPath);
        avatarImage.sprite = Resources.Load<Sprite>(avatarPath);
    }
    public string GetCurrentTitle()
    {
        return currTitle;
    }
    public Sprite GetCurrentBanner()
    {
        return bannerImage.sprite;
    }
    public Sprite GetCurrAvatar()
    {
        return avatarImage.sprite;
    }
    void SetSlider()
    {
        if (profileLevelText != null)
        {
            int xpForLevelUp = levelSlider.GetXPforLevelUp();
            int nextLevel = levelSlider.GetGameLevel() + 1;

            profileLevelText.text = "<color=yellow>" + xpForLevelUp + "</color> XP > <size=300>Rank</size> "
                + "<color=yellow>" + nextLevel + "</color>";
        }
    }

    void SetRewardNextLevel()
    {
        int nextLevel = levelSlider.GetGameLevel() + 1;

        if (nextLevel > 100)
        {
            rewardNextLevel.text = "You are awesome!";
        }
        else
        {
            var reward = FindObjectOfType<RewardForLevel>().GetReward(nextLevel);
            Sprite rewardSprite = null;
            if (reward != null)
            {
                rewardNextLevel.text = reward.reward.ToString() + ": " + reward.id;
                rewardSprite = reward.GetRewardSprite();
            }
            else
            {
                rewardNextLevel.text = "No rewards :(";
            }
            if (rewardSprite != null)
            {
                rewardImage.sprite = rewardSprite;
            }
            else
            {
                rewardImage.gameObject.SetActive(false);
            }
        }
    }

    public void ActivateTrinketsCollection() //for changing avatar image
    {
        UI_System.Instance.SwitchScreens(collectionScreen);
        trinketsToggle.isOn = true;
    }
    public void ActivateBannersCollection() //for changing banner image
    {
        UI_System.Instance.SwitchScreens(collectionScreen);
        bannersToggle.isOn = true;
    }
}