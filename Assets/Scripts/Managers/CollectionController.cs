﻿using UnityEngine;
using UnityEngine.UI;

public class CollectionController : MonoBehaviour
{
    public static CollectionController Instance;

    [SerializeField] Text sectionName;
    [SerializeField] Text unlockNumber;
    [SerializeField] Text itemDescription;
    [SerializeField] Button equipButton;
    [SerializeField] Material blackAndWhiteMat;
    [SerializeField] ProfileHandler profileHandler;

    [Header("Worlds")]
    [SerializeField] Transform worldsContainer;
    [SerializeField] Transform worldSelectionGlow;
    [SerializeField] GameObject worldTemplate;
    public WorldInformation[] worlds;   //quest uses for randomisation

    [Header("Boosts")]
    [SerializeField] Transform boostsContainer;
    [SerializeField] Transform boostSelectionGlow;
    [SerializeField] GameObject boostTemplate;
    [SerializeField] Sprite[] levelFrames;
    public Boost[] boostInfos;  // public for unlocking all boosts (debugging)

    [Header("Trinkets")]
    [SerializeField] Transform trinketsContainer;
    [SerializeField] Transform trinketSelectionGlow;
    [SerializeField] GameObject trinketTemplate;
    [SerializeField] LevelTemplate[] trinkets;

    [Header("Titles")]
    [SerializeField] Transform titlesContainer;
    [SerializeField] Transform titleSelectionGlow;
    [SerializeField] GameObject titlePrefab;
    [SerializeField] Title[] titles;

    [Header("Banners")]
    [SerializeField] Transform bannersContainer;
    [SerializeField] GameObject bannerPanelPrefab;
    [SerializeField] Transform bannerSelectionGlow;
    [SerializeField] Banner[] banners;

    const string LOCK_TAG = "Lock";
    const string SECTION_NAME_DOTS = "<size=120><color=blue>- - - - - - - - - - -</color></size>";
    const string LOCKED = "<color=red>- LOCKED -</color>";
    const string UNLOCKED_IN_SHOP = "Unlocked by purchasing in shop! ";
    const string UNLOCKED_BY_RANK = "Unlocked by reaching Player Rank ";

    const string BANNERS_LOCATION = "Sprites/UI images/Banners/";
    const string TRINKETS_LOCATION = "Sprites/UI images/Trinkets/";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        SetWorlds();
        SetTitles();
        SetBanners();
        SetTrinkets();
        SetBoosts();
    }
    void ClearContainer(Transform container)
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }
    #region World
    public void SetWorldCollectionTexts()   //called on section toggles event
    {
        var worldsUnlocked = GameData.gameData.saveData.worldIds;
        unlockNumber.text = "<size=400>" + worldsUnlocked.Count
            + "</size>/" + worlds.Length + "\n<color=blue>UNLOCKED</color>";
        sectionName.text = "<color=lime>WORLDS</color>\n" + SECTION_NAME_DOTS;
        itemDescription.text = "";
        equipButton.gameObject.SetActive(false);
    }
    public void SetWorlds()
    {
        //ClearContainer(worldsContainer);
        var worldsUnlocked = GameData.gameData.saveData.worldIds;

        for (int i = 0; i < worlds.Length; i++)
        {
            string worldName = worlds[i].id;
            Transform worldPanel = Instantiate(worldTemplate, worldsContainer).transform;
            worldPanel.gameObject.name = worldName;
            Image image = worldPanel.GetComponent<Image>();
            image.sprite = worlds[i].GetRewardSprite();
            Button button = worldPanel.GetComponent<Button>();
            string descr = SequentialText.ColorString("<size=410>" + worldName + "</size>", Color.green);
            int index = i;

            if (!worldsUnlocked.Contains(worldName))
            {
                worldPanel.GetChild(1).gameObject.SetActive(true);  //lock image
                image.material = blackAndWhiteMat;
                button.onClick.AddListener(delegate () { OnWorldClicked(descr, index, false); });
            }
            else
            {
                button.onClick.AddListener(delegate () { OnWorldClicked(descr, index, true); });
            }
            button.onClick.AddListener(delegate () { SetSelectionGlowPos(worldSelectionGlow, worldPanel.position); });
        }
    }
    void OnWorldClicked(string description, int index, bool isUnlocked)
    {
        if (!isUnlocked)
        {
            string unlockRequirement = GetUnlockRequirement(LevelReward.World, worlds[index].id, UNLOCKED_IN_SHOP, UNLOCKED_BY_RANK);
            description += "\n" + LOCKED + "\n" + unlockRequirement;
        }
        else
        {
            description += "\n" + worlds[index].description;
        }
        itemDescription.text = description;
    }
    #endregion
    #region Boost
    public void SetBoostCollectionTexts()   //called on section toggles event
    {
        var boostsUnlocked = GameData.gameData.saveData.boostIds;
        unlockNumber.text = "<size=400>" + boostsUnlocked.Count
            + "</size>/" + boostInfos.Length + "\n<color=blue>UNLOCKED</color>";
        sectionName.text = "<color=red>BOOSTS</color>\n" + SECTION_NAME_DOTS;
        itemDescription.text = "";
        equipButton.gameObject.SetActive(false);
    }
    public void SetBoosts()
    {
        //ClearContainer(boostsContainer);
        var boostsUnlocked = GameData.gameData.saveData.boostIds;

        for (int i = 0; i < boostInfos.Length; i++)
        {
            Boost boost = boostInfos[i];
            var boostPanel = Instantiate(boostTemplate, boostsContainer).transform;
            boostPanel.gameObject.name = boost.id;
            Image[] images = boostPanel.GetComponentsInChildren<Image>();
            int levelFrameIndex = BonusManager.ChooseBoostSpriteIndex(GameData.gameData.GetBoostLevel(boost.id)); //get frame based on boost level

            images[0].sprite = levelFrames[levelFrameIndex]; // set boost frame sprite
            images[1].sprite = boost.UpgradeSprites[levelFrameIndex]; // set boost image based on level

            string title = boost.id;
            Button button = boostPanel.GetComponent<Button>();
            string descr = "<color=red><size=450>" + title + "</size></color>";
            int index = i;
            if (!boostsUnlocked.Contains(boost.id))
            {
                boostPanel.GetChild(2).gameObject.SetActive(true);  //lock image
                images[1].material = blackAndWhiteMat;
                button.onClick.AddListener(delegate () { OnBoostClicked(descr, index, false); });
            }
            else
            {
                button.onClick.AddListener(delegate () { OnBoostClicked(descr, index, true); });
            }
            button.onClick.AddListener(delegate () { SetSelectionGlowPos(boostSelectionGlow, boostPanel.position); });
        }
    }
    void OnBoostClicked(string description, int index, bool isUnlocked)
    {
        if (!isUnlocked)
        {
            string unlockRequirement = GetUnlockRequirement(LevelReward.Boost, boostInfos[index].id, UNLOCKED_IN_SHOP, UNLOCKED_BY_RANK);
            description += "\n" + LOCKED + "\n" + unlockRequirement;
        }
        else
        {
            description += "\n" + boostInfos[index].description;
        }
        itemDescription.text = description;
    }
    #endregion
    #region Trinket
    public void SetTrinketCollectionTexts()
    {
        var trinketsUnlocked = GameData.gameData.saveData.trinketIds;
        unlockNumber.text = "<size=400>" + trinketsUnlocked.Count
            + "</size>/" + trinkets.Length + "\n<color=blue>UNLOCKED</color>";
        sectionName.text = "<color=blue>TRINKETS</color>\n" + SECTION_NAME_DOTS;
        itemDescription.text = "";
        equipButton.gameObject.SetActive(false);
    }
    public void SetTrinkets()
    {
        //ClearContainer(trinketsContainer);
        var trinketsUnlocked = GameData.gameData.saveData.trinketIds;

        for (int j = 0; j < trinkets.Length; j++)
        {
            var trinket = Instantiate(trinketTemplate, trinketsContainer).transform;
            trinket.gameObject.name = trinkets[j].id;
            Image image = trinket.GetComponent<Image>();
            image.sprite = trinkets[j].trinketSprite;
            Button button = trinket.GetComponent<Button>();
            string trinkName = trinkets[j].id;
            string descr = "<color=orange><size=420>" + trinkName + "</size></color>";

            int index = j;
            if (!trinketsUnlocked.Contains(trinkets[j].id))
            {
                trinket.GetChild(0).gameObject.SetActive(true);  //lock image
                image.material = blackAndWhiteMat;
                button.onClick.AddListener(delegate () { OnTrinketClicked(descr, index, false); });
            }
            else
            {
                button.onClick.AddListener(delegate () { OnTrinketClicked(descr, index, true); });
            }
            button.onClick.AddListener(delegate () { SetSelectionGlowPos(trinketSelectionGlow, trinket.position); });
        }
    }
    void OnTrinketClicked(string description, int index, bool isUnlocked)
    {
        if (!isUnlocked)
        {
            equipButton.gameObject.SetActive(false);    //button to equip a avatar
            string unlockRequirement = GetUnlockRequirement(LevelReward.Trinket, trinkets[index].id, UNLOCKED_IN_SHOP, UNLOCKED_BY_RANK);
            description += "\n" + LOCKED + "\n" + unlockRequirement;
        }
        else
        {
            description += "\n" + trinkets[index].description;
            equipButton.gameObject.SetActive(true);
            Text buttoText = equipButton.GetComponentInChildren<Text>();
            if (trinkets[index].trinketSprite == profileHandler.GetCurrAvatar())  //set button text if avatar is equiped or not
            {
                buttoText.text = "Equiped!";
            }
            else
            {
                buttoText.text = "Equip!";
            }
            Button button = equipButton.GetComponent<Button>();
            button.onClick.AddListener(delegate () { SetAvatarEquipButton(trinkets[index].trinketSprite); });
        }
        itemDescription.text = description;
    }
    void SetAvatarEquipButton(Sprite avatar)
    {
        GameData.gameData.ChangeAvatar(TRINKETS_LOCATION + avatar.name);   //save avatar 
        profileHandler.UpdateAvatar(avatar);   //update avatar in profile panel
    }
    #endregion
    #region Title
    public void SetTitleCollectionTexts()
    {
        var titlesUnlocked = GameData.gameData.saveData.titleIds;
        unlockNumber.text = "<size=400>" + titlesUnlocked.Count
            + "</size>/" + titles.Length + "\n<color=blue>UNLOCKED</color>";
        sectionName.text = "<color=orange>TITLES</color>\n" + SECTION_NAME_DOTS;
        itemDescription.text = "";
        equipButton.gameObject.SetActive(false);
    }
    public void SetTitles()
    {
        //ClearContainer(titlesContainer);
        var titlesUnlocked = GameData.gameData.saveData.titleIds;

        for (int i = 0; i < titles.Length; i++)
        {
            var titlePanel = Instantiate(titlePrefab, titlesContainer).transform;
            string title = titles[i].id;
            titlePanel.gameObject.name = title;
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
            button.onClick.AddListener(delegate () { SetSelectionGlowPos(titleSelectionGlow, titlePanel.position); });
        }
    }
    void OnTitleClicked(string description, int index, bool isUnlocked)
    {
        string title = titles[index].id;
        if (!isUnlocked)
        {
            equipButton.gameObject.SetActive(false);    //button to equip a title
            string unlockRequirement = GetUnlockRequirement(LevelReward.Title, titles[index].id.ToString(), UNLOCKED_IN_SHOP, UNLOCKED_BY_RANK);
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
                equipButton.onClick.AddListener(delegate () { SetTitleEquipButton(title); });
                buttoText.text = "Equip!";
            }
        }

        itemDescription.text = description;
    }
    void SetTitleEquipButton(string title)
    {
        GameData.gameData.ChangeTitle(title);   //save title 
        profileHandler.UpdateTitle(title);   //update title in profile panel
    }
    #endregion
    #region Banner
    public void SetBannerCollectionTexts()
    {
        var bannersUnlocked = GameData.gameData.saveData.bannerIds;
        unlockNumber.text = "<size=400>" + bannersUnlocked.Count
            + "</size>/" + banners.Length + "\n<color=blue>UNLOCKED</color>";
        sectionName.text = "<color=red>BANNERS</color>\n" + SECTION_NAME_DOTS;
        itemDescription.text = "";
        equipButton.gameObject.SetActive(false);
    }
    public void SetBanners()
    {
        //ClearContainer(bannersContainer);
        var bannersUnlocked = GameData.gameData.saveData.bannerIds;

        for (int i = 0; i < banners.Length; i++)
        {
            var bannerPanel = Instantiate(bannerPanelPrefab, bannersContainer).transform;
            bannerPanel.gameObject.name = banners[i].id;
            Image image = bannerPanel.GetComponent<Image>();
            image.sprite = banners[i].Sprite;
            Button button = bannerPanel.GetComponent<Button>();
            string bannerName = banners[i].id;
            string descr = "<color=orange><size=450>" + bannerName + "</size></color>";
            int index = i;
            if (!bannersUnlocked.Contains(bannerName))
            {
                bannerPanel.GetChild(0).gameObject.SetActive(true);  //lock image
                image.material = blackAndWhiteMat;
                button.onClick.AddListener(delegate () { OnBannerClicked(descr, index, false); });
            }
            else
            {
                button.onClick.AddListener(delegate () { OnBannerClicked(descr, index, true); });
            }
            button.onClick.AddListener(delegate () { SetSelectionGlowPos(bannerSelectionGlow, bannerPanel.position); });
        }
    }
    void OnBannerClicked(string description, int index, bool isUnlocked)
    {
        if (!isUnlocked)
        {
            equipButton.gameObject.SetActive(false);    //button to equip a banner
            string unlockRequirement = GetUnlockRequirement(LevelReward.Banner, banners[index].id, UNLOCKED_IN_SHOP, UNLOCKED_BY_RANK);
            description += "\n" + LOCKED + "\n" + unlockRequirement;
        }
        else
        {
            description += "\n<size=250>" + banners[index].description + "</size>";
            equipButton.gameObject.SetActive(true);
            Text buttoText = equipButton.GetComponentInChildren<Text>();
            if (banners[index].Sprite == profileHandler.GetCurrentBanner())  //set button text if banner is equiped or not
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
    void SetBannerEquipButton(int index)
    {
        GameData.gameData.ChangeBanner(BANNERS_LOCATION + banners[index].Sprite.name);   //save banner path 
        profileHandler.UpdateBanner(banners[index].Sprite);   //update banner in profile panel
    }
    #endregion
    void SetSelectionGlowPos(Transform selection, Vector3 pos)
    {
        selection.position = pos;
    }
    public void UpdateCollectionElement(RewardTemplate reward)
    {
        Transform container = null;
        switch (reward.reward)
        {
            case LevelReward.World:
                {
                    container = worldsContainer;
                    break;
                }
            case LevelReward.Boost:
                {
                    container = boostsContainer;
                    break;
                }
            case LevelReward.Trinket:
                {
                    container = trinketsContainer;
                    break;
                }
            case LevelReward.Title:
                {
                    container = titlesContainer;
                    break;
                }
            case LevelReward.Banner:
                {
                    container = bannersContainer;
                    break;
                }
        }

        foreach (Transform child in container)
        {
            if (child.gameObject.name == reward.id)
            {
                GameObject lockObj = GetChildWithTag(child, LOCK_TAG);
                if (lockObj != null)
                    lockObj.SetActive(false);
                child.GetComponent<Image>().material = null;
                child.GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    itemDescription.text = "<color=orange><size=450>" + reward.id + "</size></color>"
                    + "\n<size=250>" + reward.description + "</size>";
                });
                break;
            }
        }
    }
    GameObject GetChildWithTag(Transform parent, string tag)
    {
        foreach (Transform child in parent)
        {
            if (child.tag.CompareTo(tag) == 0)
                return child.gameObject;
        }
        return null;
    }
    public static string GetUnlockRequirement(LevelReward levelReward, string id, string UNL_IN_SHOP_TEXT, string UNL_BY_RANK_TEXT)
    {
        int rankToUnlock = RewardForLevel.Instance.GetRankFromRewards(levelReward, id);

        if (rankToUnlock < 0)   //negative if not found in rewards
        {
            return UNL_IN_SHOP_TEXT;
        }
        else
        {
            //return "<color=black><size=250>" + UNLOCKED_BY_RANK + rankToUnlock + ".</size></color>";
            return UNL_BY_RANK_TEXT + rankToUnlock;
        }
    }
    public static RewardTemplate FindItemWithId(RewardTemplate[] items, string id)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].id.CompareTo(id) == 0)
            {
                return items[i];
            }
        }
        return null;
    }
}