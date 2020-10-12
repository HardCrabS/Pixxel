using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionController : MonoBehaviour 
{
    public static CollectionController Instance;

    [SerializeField] Text sectionName;
    [SerializeField] Text unlockNumber;
    [SerializeField] Text itemDescription;
    [SerializeField] Button equipButton;

    [Header("Titles")]
    [SerializeField] Transform titlesContainer;
    [SerializeField] string[] titles;

    const string SECTION_NAME_DOTS = "<size=120><color=blue>- - - - - - - - - - -</color></size>";
    const string LOCKED = "<color=red>- LOCKED -</color>";
    const string UNLOCKED_IN_SHOP = "Unlocked by purchasing in shop! ";
    const string UNLOCKED_BY_RANK = "Unlocked by reaching Player Rank ";

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

	void Start () 
    {
        SetTitles();
	}

    public void SetWorldDescription(string worldName, string description, int index)
    {
        var worlds = GameData.gameData.saveData.worldUnlocked;
        if (worlds[index] == true)
        {
            itemDescription.text = "<color=purple><size=350>" + worldName + "</size></color>\n" + description;
        }
        else
        {
            string unlockRequirement = GetUnlockRequirement(LevelReward.World, index);
            itemDescription.text = "<color=green>" + worldName + "</color>"
                + "\n" + LOCKED + "\n\n" + unlockRequirement;
        }
    }

    public void SetWorldCollectionTexts()   //called on section toggles event
    {
        var worlds = GameData.gameData.saveData.worldUnlocked;
        SetNumberOfUnlocked(worlds);
        sectionName.text = "<color=blue>WORLDS</color>\n" + SECTION_NAME_DOTS;
        itemDescription.text = "";
        equipButton.gameObject.SetActive(false);
    }

    public void SetTitleCollectionTexts()
    {
        var titles = GameData.gameData.saveData.titlesUnlocked;
        SetNumberOfUnlocked(titles);
        sectionName.text = "<color=orange>TITLES</color>\n" + SECTION_NAME_DOTS;
        itemDescription.text = "";
        equipButton.gameObject.SetActive(false);
    }
    void SetTitles()
    {
        var titlesUnlocked = GameData.gameData.saveData.titlesUnlocked;

        for(int i = 0; i < titlesUnlocked.Length; i++)
        {
            var titlePanel = titlesContainer.GetChild(i);
            titlePanel.GetComponentInChildren<Text>().text = titles[i];
            Button button = titlePanel.GetComponent<Button>();
            string descr = "<color=orange><size=450>" + titles[i] + "</size></color>";
            int index = i;
            if (!titlesUnlocked[i])
            {
                titlePanel.GetChild(1).gameObject.SetActive(true);
                button.onClick.AddListener(delegate () { OnTitleClicked(descr, index, false); });
            }
            else
            {
                button.onClick.AddListener(delegate () { OnTitleClicked(descr, index, true); });
            }
        }
    }

    void OnTitleClicked(string description, int index, bool isUnlocked) 
    {
        if(!isUnlocked)
        {
            equipButton.gameObject.SetActive(false);    //button to equip a title
            string unlockRequirement = GetUnlockRequirement(LevelReward.Title, index);
            description += "\n" + LOCKED + "\n" + unlockRequirement;
        }
        else
        {
            equipButton.gameObject.SetActive(true);
            equipButton.GetComponent<Button>().onClick.
                AddListener(delegate () { GameData.gameData.ChangeTitle(titles[index]); }); //change title in profile on click
        }

        itemDescription.text = description;
    }

    string GetUnlockRequirement(LevelReward levelReward, int index)
    {
        int rankToUnlock = RewardForLevel.Instance.GetRankFromRewards(levelReward, index);

        if (rankToUnlock < 0)   //negative if not found in rewards
        {
            return UNLOCKED_IN_SHOP;
        }
        else
        {
            return "<color=black><size=250>" + UNLOCKED_BY_RANK
                    + rankToUnlock + ".</size></color>";
        }
    }

    void SetNumberOfUnlocked(bool[] arr)
    {
        int unlockedCount = 0;
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == true)
            {
                unlockedCount++;
            }
        }
        unlockNumber.text = "<size=400>" + unlockedCount 
            + "</size>/" + arr.Length + "\n<color=blue>UNLOCKED</color>";
    }
}