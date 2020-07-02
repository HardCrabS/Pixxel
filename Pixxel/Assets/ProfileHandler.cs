using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileHandler : MonoBehaviour 
{
    [SerializeField] LevelSlider levelSlider;
    [SerializeField] Text profileLevelText;

    [SerializeField] RewardForLevel rewarder;
    [SerializeField] Text rewardNextLevel;

    [SerializeField] Text playerName;
    [SerializeField] Image profilePicture;

    [SerializeField] Text changedName;

    // Use this for initialization
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
    }

    public void ChangeName()
    {
        string newName = changedName.text;
        playerName.text = newName;
        GameData.gameData.saveData.playerInfo.username = newName;
        GameData.gameData.Save();
        DatabaseManager.ChangeName(newName);
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
        int levelsUntilReward = rewarder.GetLevelsUntilReward(nextLevel);

        if(levelsUntilReward == 0)
        {
            rewardNextLevel.text = "You are awesome!";
        }
        else if(levelsUntilReward == 1)
        {
            rewardNextLevel.text = "<color=yellow>" + levelsUntilReward + "</color> level until next reward!";
        }
        else
        {
            rewardNextLevel.text = "<color=yellow>" + levelsUntilReward + "</color> levels until next reward!";
        }
    }
}
