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
    [SerializeField] ProfileHandler profileHandler;

    [Header("Trinkets")]
    [SerializeField] Transform trinketsContainer;
    [SerializeField] Transform trinketSelectionGlow;
    [SerializeField] LevelTemplate[] trinkets;

    [Header("Titles")]
    [SerializeField] Transform titlesContainer;
    [SerializeField] Transform titleSelectionGlow;
    [SerializeField] string[] titles;

    [Header("Banners")]
    [SerializeField] Transform bannersContainer;
    [SerializeField] GameObject bannerPanelPrefab;
    [SerializeField] Transform bannerSelectionGlow;
    [SerializeField] Banner[] banners;

    const string SECTION_NAME_DOTS = "<size=120><color=blue>- - - - - - - - - - -</color></size>";
    const string LOCKED = "<color=red>- LOCKED -</color>";
    const string UNLOCKED_IN_SHOP = "Unlocked by purchasing in shop! ";
    const string UNLOCKED_BY_RANK = "Unlocked by reaching Player Rank ";

    const string BANNERS_LOCATION = "Sprites/UI images/Banners/";

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
        //SetTrinkets();
        SetBanners();
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
        unlockNumber.text = "<size=400>" + GetNumberOfUnlocked(worlds)
            + "</size>/" + worlds.Length + "\n<color=blue>UNLOCKED</color>";
        sectionName.text = "<color=lime>WORLDS</color>\n" + SECTION_NAME_DOTS;
        itemDescription.text = "";
        equipButton.gameObject.SetActive(false);
    }
    public void SetTrinketCollectionTexts()
    {
        var trinkets = GameData.gameData.saveData.worldTrinkets;
        int totalTrinkets = 0;
        int totalUnlocked = 0;
        for (int i = 0; i < trinkets.Length; i++)
        {
            totalTrinkets += trinkets[i].trinkets.Length;
            totalUnlocked += GetNumberOfUnlocked(trinkets[i].trinkets);
        }
        unlockNumber.text = "<size=400>" + totalUnlocked
            + "</size>/" + totalTrinkets + "\n<color=blue>UNLOCKED</color>";
        sectionName.text = "<color=blue>TRINKETS</color>\n" + SECTION_NAME_DOTS;
        itemDescription.text = "";
        equipButton.gameObject.SetActive(false);
    }
    public void SetTitleCollectionTexts()
    {
        var titles = GameData.gameData.saveData.titlesUnlocked;
        unlockNumber.text = "<size=400>" + GetNumberOfUnlocked(titles)
            + "</size>/" + titles.Length + "\n<color=blue>UNLOCKED</color>";
        sectionName.text = "<color=orange>TITLES</color>\n" + SECTION_NAME_DOTS;
        itemDescription.text = "";
        equipButton.gameObject.SetActive(false);
    }
    public void SetBannerCollectionTexts()
    {
        var banners = GameData.gameData.saveData.bannersUnlocked;
        unlockNumber.text = "<size=400>" + GetNumberOfUnlocked(banners)
            + "</size>/" + banners.Length + "\n<color=blue>UNLOCKED</color>";
        sectionName.text = "<color=red>BANNERS</color>\n" + SECTION_NAME_DOTS;
        itemDescription.text = "";
        equipButton.gameObject.SetActive(false);
    }
    void SetTitles()
    {
        var titlesUnlocked = GameData.gameData.saveData.titlesUnlocked;

        for (int i = 0; i < titlesUnlocked.Length; i++)
        {
            var titlePanel = titlesContainer.GetChild(i);
            titlePanel.GetComponentInChildren<Text>().text = titles[i];
            Button button = titlePanel.GetComponent<Button>();
            string descr = "<color=orange><size=450>" + titles[i] + "</size></color>";
            int index = i;
            if (!titlesUnlocked[i])
            {
                titlePanel.GetChild(1).gameObject.SetActive(true);  //lock image
                button.onClick.AddListener(delegate () { OnTitleClicked(descr, index, false); });
            }
            else
            {
                button.onClick.AddListener(delegate () { OnTitleClicked(descr, index, true); });
            }
            button.onClick.AddListener(delegate () { SetTitleSelection(titlePanel.position); });
        }
    }
    void SetBanners()
    {
        var bannersUnlocked = GameData.gameData.saveData.bannersUnlocked;

        for (int i = 0; i < banners.Length; i++)
        {
            var bannerPanel = Instantiate(bannerPanelPrefab, bannersContainer).transform;
            bannerPanel.GetComponent<Image>().sprite = banners[i].Sprite;
            Button button = bannerPanel.GetComponent<Button>();
            string descr = "<color=orange><size=450>" + banners[i].Title + "</size></color>";
            int index = i;
            if (!bannersUnlocked[i])
            {
                bannerPanel.GetChild(0).gameObject.SetActive(true);  //lock image
                button.onClick.AddListener(delegate () { OnBannerClicked(descr, index, false); });
            }
            else
            {
                button.onClick.AddListener(delegate () { OnBannerClicked(descr, index, true); });
            }
            button.onClick.AddListener(delegate () { SetBannerSelection(bannerPanel.position); });
        }
    }
    void SetTrinkets()
    {
        var trinketsUnlocked = GameData.gameData.saveData.worldTrinkets;

        int counter = 0;
        foreach(var world in trinketsUnlocked)
        {
            for (int j = 0; j < world.trinkets.Length; j++)
            {
                var trinket = trinketsContainer.GetChild(counter);
                trinket.GetComponent<Image>().sprite = trinkets[counter].trinketSprite;
                Button button = trinket.GetComponent<Button>();
                string descr = "<color=orange><size=450>" + trinkets[counter].trinketName + "</size></color>";

                if (!world.trinkets[j])
                {
                    trinket.GetChild(0).gameObject.SetActive(true);  //lock image
                    button.onClick.AddListener(delegate () { OnTrinketClicked(descr, counter, false); });
                }
                else
                {
                    button.onClick.AddListener(delegate () { OnTrinketClicked(descr, counter, true); });
                }
                button.onClick.AddListener(delegate () { SetTrinketSelection(trinket.position); });

                counter++;
            }
        }
    }
    void OnTrinketClicked(string description, int index, bool isUnlocked)
    {
        if (!isUnlocked)
        {
            string unlockRequirement = GetUnlockRequirement(LevelReward.Title, index);
            description += "\n" + LOCKED + "\n" + unlockRequirement;
        }
        itemDescription.text = description;
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
            Text buttoText = equipButton.GetComponentInChildren<Text>();
            if (titles[index] == profileHandler.GetCurrentTitle())  //set button text if title is equiped or not
            {
                buttoText.text = "Equiped!";
            }
            else
            {
                buttoText.text = "Equip!";
            }
            Button button = equipButton.GetComponent<Button>();
            button.onClick.AddListener(delegate () { SetTitleEquipButton(index); });
        }

        itemDescription.text = description;
    }
    void OnBannerClicked(string description, int index, bool isUnlocked)
    {
        if (!isUnlocked)
        {
            equipButton.gameObject.SetActive(false);    //button to equip a title
            string unlockRequirement = GetUnlockRequirement(LevelReward.Banner, index);
            description += "\n" + LOCKED + "\n" + unlockRequirement;
        }
        else
        {
            description += "\n<size=250>" + banners[index].Description + "</size>";
            equipButton.gameObject.SetActive(true);
            Text buttoText = equipButton.GetComponentInChildren<Text>();
            if (banners[index].Sprite == profileHandler.GetCurrentBanner())  //set button text if title is equiped or not
            {
                buttoText.text = "Equiped!";
            }
            else
            {
                buttoText.text = "Equip!";
            }
            Button button = equipButton.GetComponent<Button>();
            button.onClick.AddListener(delegate () { SetBannerEquipButton(index); });
        }

        itemDescription.text = description;
    }
    void SetTitleEquipButton(int index)
    {
        GameData.gameData.ChangeTitle(titles[index]);   //save title 
        profileHandler.UpdateTitle(titles[index]);   //update title in profile panel
    }
    void SetBannerEquipButton(int index)
    {
        GameData.gameData.ChangeBanner(BANNERS_LOCATION + banners[index].Sprite.name);   //save banner path 
        profileHandler.UpdateBanner(banners[index].Sprite);   //update banner in profile panel
    }
    void SetTitleSelection(Vector3 position)
    {
        titleSelectionGlow.position = position;
    }
    void SetTrinketSelection(Vector3 position)
    {
        trinketSelectionGlow.position = position;
    }
    void SetBannerSelection(Vector3 position)
    {
        bannerSelectionGlow.position = position;
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

    int GetNumberOfUnlocked(bool[] arr)
    {
        int unlockedCount = 0;
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == true)
            {
                unlockedCount++;
            }
        }
        return unlockedCount;
    }
}