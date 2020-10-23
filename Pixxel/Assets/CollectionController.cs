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

    [Header("Worlds")]
    [SerializeField] WorldInformation[] worlds;

    [Header("Trinkets")]
    [SerializeField] Transform trinketsContainer;
    [SerializeField] Transform trinketSelectionGlow;
    [SerializeField] GameObject trinketTemplate;
    [SerializeField] LevelTemplate[] trinkets;

    [Header("Titles")]
    [SerializeField] Transform titlesContainer;
    [SerializeField] Transform titleSelectionGlow;
    [SerializeField] Title[] titles;

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
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        SetTitles();
        SetBanners();
        SetTrinkets();
    }

    public void SetWorldDescription(string worldName, string description)//called in WorldSprite class
    {
        var worlds = GameData.gameData.saveData.worldIds;
        if (worlds.Contains(worldName))
        {
            itemDescription.text = "<color=purple><size=350>" + worldName + "</size></color>\n" + description;
        }
        else
        {
            string unlockRequirement = GetUnlockRequirement(LevelReward.World, worldName);
            itemDescription.text = "<color=green>" + worldName + "</color>"
                + "\n" + LOCKED + "\n\n" + unlockRequirement;
        }
    }

    public void SetWorldCollectionTexts()   //called on section toggles event
    {
        var worldsUnlocked = GameData.gameData.saveData.worldIds;
        unlockNumber.text = "<size=400>" + worldsUnlocked.Count
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
            //totalUnlocked += GetNumberOfUnlocked(trinkets[i].trinkets);
        }
        unlockNumber.text = "<size=400>" + totalUnlocked
            + "</size>/" + totalTrinkets + "\n<color=blue>UNLOCKED</color>";
        sectionName.text = "<color=blue>TRINKETS</color>\n" + SECTION_NAME_DOTS;
        itemDescription.text = "";
        equipButton.gameObject.SetActive(false);
    }
    public void SetTitleCollectionTexts()
    {
        var titlesUnlocked = GameData.gameData.saveData.titleIds;
        unlockNumber.text = "<size=400>" + titlesUnlocked.Count
            + "</size>/" + titles.Length + "\n<color=blue>UNLOCKED</color>";
        sectionName.text = "<color=orange>TITLES</color>\n" + SECTION_NAME_DOTS;
        itemDescription.text = "";
        equipButton.gameObject.SetActive(false);
    }
    public void SetBannerCollectionTexts()
    {
        var bannersUnlocked = GameData.gameData.saveData.bannerIds;
        unlockNumber.text = "<size=400>" + bannersUnlocked.Count
            + "</size>/" + banners.Length + "\n<color=blue>UNLOCKED</color>";
        sectionName.text = "<color=red>BANNERS</color>\n" + SECTION_NAME_DOTS;
        itemDescription.text = "";
        equipButton.gameObject.SetActive(false);
    }
    void SetTitles()
    {
        var titlesUnlocked = GameData.gameData.saveData.titleIds;

        for (int i = 0; i < titles.Length; i++)
        {
            var titlePanel = titlesContainer.GetChild(i);
            string title = titles[i].title.ToString();
            titlePanel.GetComponentInChildren<Text>().text = title;
            Button button = titlePanel.GetComponent<Button>();
            string descr = "<color=orange><size=450>" + title + "</size></color>";
            int index = i;
            if (!titlesUnlocked.Contains(title))
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
        var bannersUnlocked = GameData.gameData.saveData.bannerIds;

        for (int i = 0; i < banners.Length; i++)
        {
            var bannerPanel = Instantiate(bannerPanelPrefab, bannersContainer).transform;
            bannerPanel.GetComponent<Image>().sprite = banners[i].Sprite;
            Button button = bannerPanel.GetComponent<Button>();
            string bannerName = RewardTemplate.SplitCamelCase(banners[i].BannerName);
            string descr = "<color=orange><size=450>" + bannerName + "</size></color>";
            int index = i;
            if (!bannersUnlocked.Contains(banners[i].BannerName))
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
        var trinketsUnlocked = GameData.gameData.saveData.trinketIds;

        for (int j = 0; j < trinkets.Length; j++)
        {
            var trinket = Instantiate(trinketTemplate, trinketsContainer).transform;
            trinket.GetComponent<Image>().sprite = trinkets[j].trinketSprite;
            Button button = trinket.GetComponent<Button>();
            string trinkName = RewardTemplate.SplitCamelCase(trinkets[j].GetRewardId());
            string descr = "<color=orange><size=450>" + trinkName + "</size></color>";

            int index = j;
            if (!trinketsUnlocked.Contains(trinkets[j].GetRewardId()))
            {
                trinket.GetChild(0).gameObject.SetActive(true);  //lock image
                button.onClick.AddListener(delegate () { OnTrinketClicked(descr, index, false); });
            }
            else
            {
                button.onClick.AddListener(delegate () { OnTrinketClicked(descr, index, true); });
            }
            button.onClick.AddListener(delegate () { SetTrinketSelection(trinket.position); });
        }
    }
    void OnTrinketClicked(string description, int index, bool isUnlocked)
    {
        if (!isUnlocked)
        {
            string unlockRequirement = GetUnlockRequirement(LevelReward.Trinket, trinkets[index].GetRewardId());
            description += "\n" + LOCKED + "\n" + unlockRequirement;
        }
        else
        {
            description += "\n" + trinkets[index].description;
        }
        itemDescription.text = description;
    }
    void OnTitleClicked(string description, int index, bool isUnlocked)
    {
        string title = RewardTemplate.SplitCamelCase(titles[index].title.ToString());
        if (!isUnlocked)
        {
            equipButton.gameObject.SetActive(false);    //button to equip a title
            string unlockRequirement = GetUnlockRequirement(LevelReward.Title, titles[index].title.ToString());
            description += "\n" + LOCKED + "\n" + unlockRequirement;
        }
        else
        {
            equipButton.gameObject.SetActive(true);
            Text buttoText = equipButton.GetComponentInChildren<Text>();
            if (title == profileHandler.GetCurrentTitle())  //set button text if title is equiped or not
            {
                buttoText.text = "Equiped!";
            }
            else
            {
                buttoText.text = "Equip!";
            }
            Button button = equipButton.GetComponent<Button>();
            button.onClick.AddListener(delegate () { SetTitleEquipButton(title); });
        }

        itemDescription.text = description;
    }
    void OnBannerClicked(string description, int index, bool isUnlocked)
    {
        if (!isUnlocked)
        {
            equipButton.gameObject.SetActive(false);    //button to equip a title
            string unlockRequirement = GetUnlockRequirement(LevelReward.Banner, banners[index].BannerName);
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
    void SetTitleEquipButton(string title)
    {
        GameData.gameData.ChangeTitle(title);   //save title 
        profileHandler.UpdateTitle(title);   //update title in profile panel
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
    string GetUnlockRequirement(LevelReward levelReward, string id)
    {
        int rankToUnlock = RewardForLevel.Instance.GetRankFromRewards(levelReward, id);

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
}