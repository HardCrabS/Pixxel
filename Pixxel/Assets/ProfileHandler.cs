using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileHandler : MonoBehaviour 
{
    [SerializeField] LevelSlider levelSlider;
    [SerializeField] Text profileLevelText;

    [SerializeField] Text rewardNextLevel;
    [SerializeField] Image rewardImage;

    [SerializeField] Text playerName;
    [SerializeField] Image profilePicture;
    [SerializeField] Text titleText;
    [SerializeField] Image bannerImage;

    [SerializeField] Text changedName;

    string currTitle;

    void Start ()
    {
        SetSlider();
        SetRewardNextLevel();
    }
	
    public void SetProfileData()
    {
        User player = GameData.gameData.saveData.playerInfo;

        playerName.text = player.username;
        profilePicture.sprite = Resources.Load<Sprite>(player.spritePath);
        titleText.text = "\"" + player.titleText + "\"";
        currTitle = player.titleText;
        bannerImage.sprite = Resources.Load<Sprite>(player.bannerSpritePath);
    }

    public void ChangeName()
    {
        string newName = changedName.text;
        playerName.text = newName;
        GameData.gameData.saveData.playerInfo.username = newName;
        GameData.gameData.Save();
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
    public string GetCurrentTitle()
    {
        return currTitle;
    }
    public Sprite GetCurrentBanner()
    {
        return bannerImage.sprite;
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

        if(nextLevel > 100)
        {
            rewardNextLevel.text = "You are awesome!";
        }
        else
        {
            var reward = FindObjectOfType<RewardForLevel>().GetReward(nextLevel);
            rewardNextLevel.text = reward.reward.ToString();
            rewardImage.sprite = reward.rewardSprite;
        }
    }
}