using UnityEngine;
using UnityEngine.UI;

public enum CollectionSection
{
    World,
    Boost,
    Trinket,
    Title,
    Banner,
    None
}
public class CollectionController : MonoBehaviour
{
    public static CollectionController Instance;

    [SerializeField] Text sectionName;
    [SerializeField] Text unlockNumber;
    [SerializeField] Text itemDescription;
    [SerializeField] Image descriptionImage;
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
    [SerializeField] Transform trinketButtonContainer;
    [SerializeField] Transform trinketSelectionGlow;
    [SerializeField] GameObject trinketTemplate;
    [SerializeField] Sprite rankTrinketsSprite;
    [SerializeField] Sprite shopTrinketsSprite;
    public LevelTemplate[] trinkets;
    public LevelTemplate[] trinketsRank;
    public LevelTemplate[] trinketsShop;

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


    CollectionSection currCollectionSection = CollectionSection.None;
    public readonly string BANNERS_LOCATION = "Sprites/UI images/Banners/";
    public readonly string TRINKETS_LOCATION = "Sprites/UI images/Trinkets/";


    const string LOCK_TAG = "Lock";
    const string SECTION_NAME_DOTS = "<size=120><color=black>- - - - - - - - - - -</color></size>";
    const string LOCKED = "<color=#ff0048>- LOCKED -</color>";
    const string UNLOCKED_IN_SHOP = "Unlocked by purchasing in Shop ";
    const string UNLOCKED_BY_RANK = "Unlocked on Player Rank ";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        SetWorldCollectionTexts();
        //SetTitles();
        //SetBanners();
        //SetTrinkets();
        //SetBoosts();
    }
    void ClearContainer(Transform container, Transform exception = null)
    {
        foreach (Transform child in container)
        {
            if (child != exception)
                Destroy(child.gameObject);
        }
    }
    #region World
    public void SetWorldCollectionTexts()   //called on section toggles event
    {
        if (currCollectionSection == CollectionSection.World) return;
        descriptionImage.gameObject.SetActive(false);
        SetWorlds();
        currCollectionSection = CollectionSection.World;
        var worldsUnlocked = GameData.gameData.saveData.worldIds;
        unlockNumber.text = "<size=400>" + worldsUnlocked.Count
            + "</size>/" + worlds.Length + "\n<color=black>OWNED</color>";
        sectionName.text = "<color=#ff0048>WORLDS</color>\n" + SECTION_NAME_DOTS;
        itemDescription.text = "";
        equipButton.gameObject.SetActive(false);
    }
    void SetWorlds()
    {
        if (worldsContainer.childCount > 3) return;
        //ClearContainer(worldsContainer);
        var worldsUnlocked = GameData.gameData.saveData.worldIds;

        //spawn 2 empty objects at the beggining to make scroll offset (2 for 2 rows)
        for (int i = 0; i < 2; i++)
            Instantiate(new GameObject().AddComponent<RectTransform>(), worldsContainer);
        for (int i = 0; i < worlds.Length; i++)
        {
            string worldName = worlds[i].id;
            Transform worldPanel = Instantiate(worldTemplate, worldsContainer).transform;
            worldPanel.gameObject.name = worldName;
            Image image = worldPanel.GetComponent<Image>();
            image.sprite = worlds[i].GetRewardSprite();
            Button button = worldPanel.GetComponent<Button>();
            Color myColor = new Color(1, 0, 0.282f);
            string descr = SequentialText.ColorString("<size=310>" + worldName + "</size>", myColor);
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
        //spawn 2 empty objects at the end to make scroll offset
        for (int i = 0; i < 2; i++)
            Instantiate(new GameObject().AddComponent<RectTransform>(), worldsContainer);
        //move container right to show first worlds
        worldsContainer.GetComponent<RectTransform>().anchoredPosition += Vector2.right * 3000;
    }
    void OnWorldClicked(string description, int index, bool isUnlocked)
    {
            descriptionImage.gameObject.SetActive(true);
        if (!isUnlocked)
        {
            string unlockRequirement = GetUnlockRequirement(LevelReward.World, worlds[index]);
            description += "\n" + LOCKED + "\n" + unlockRequirement;
        }
        else
        {
            description += "\n" + worlds[index].description;
        }
        itemDescription.text = description;
    }
    public void ResetWorlds()
    {
        if (worldsContainer.childCount < 3) return;//it was not set once
        ClearContainer(worldsContainer, worldSelectionGlow);
        SetWorlds();
    }
    #endregion
    #region Boost
    public void SetBoostCollectionTexts()   //called on section toggles event
    {
        if (currCollectionSection == CollectionSection.Boost) return;
        descriptionImage.gameObject.SetActive(false);
        SetBoosts();
        currCollectionSection = CollectionSection.Boost;
        var boostsUnlocked = GameData.gameData.saveData.boostIds;
        unlockNumber.text = "<size=400>" + boostsUnlocked.Count
            + "</size>/" + boostInfos.Length + "\n<color=black>OWNED</color>";
        sectionName.text = "<color=#ff0048>BOOSTS</color>\n" + SECTION_NAME_DOTS;
        itemDescription.text = "";
        equipButton.gameObject.SetActive(false);
    }
    public void SetBoosts()
    {
        if (boostsContainer.childCount > 3) return;
        //ClearContainer(boostsContainer);
        var boostsUnlocked = GameData.gameData.saveData.boostIds;

        //spawn 2 empty objects at the beggining to make scroll offset (2 for 2 rows)
        for (int i = 0; i < 2; i++)
            Instantiate(new GameObject().AddComponent<RectTransform>(), boostsContainer);

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
            string descr = "<color=#ff0048><size=320>" + title + "</size></color>";
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

        //spawn 2 empty objects at the beggining to make scroll offset (2 for 2 rows)
        for (int i = 0; i < 2; i++)
            Instantiate(new GameObject().AddComponent<RectTransform>(), boostsContainer);

        boostsContainer.GetComponent<RectTransform>().anchoredPosition += Vector2.right * 1500;
    }
    void OnBoostClicked(string description, int index, bool isUnlocked)
    {
            descriptionImage.gameObject.SetActive(true);
        if (!isUnlocked)
        {
            string unlockRequirement = GetUnlockRequirement(LevelReward.Boost, boostInfos[index]);
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
        ToggleContainers(showButtons: true);
        SetTrinkets();
        descriptionImage.gameObject.SetActive(false);
        currCollectionSection = CollectionSection.Trinket;
        var trinketsUnlocked = GameData.gameData.saveData.trinketIds;
        unlockNumber.text = "<size=400>" + trinketsUnlocked.Count
            + "</size>/" + trinkets.Length + "\n<color=black>OWNED</color>";
        sectionName.text = "<color=#ff0048>TRINKETS</color>\n" + SECTION_NAME_DOTS 
            + SequentialText.SizeString("\n//" + "SELECT ORIGIN", 150);
        itemDescription.text = "";
        equipButton.gameObject.SetActive(false);
    }
    Button SpawnButton(string name, Sprite buttonSprite)
    {
        Transform worldPanel = Instantiate(worldTemplate, trinketButtonContainer).transform;//button as world button shape
        worldPanel.gameObject.name = name;
        Image image = worldPanel.GetComponent<Image>();
        image.sprite = buttonSprite;
        Button button = worldPanel.GetComponent<Button>();
        return button;
    }
    public void SetTrinkets()
    {
        if (trinketButtonContainer.childCount > 3) return;

        //spawn 2 empty objects at the beggining to make scroll offset (2 for 2 rows)
        for (int i = 0; i < 2; i++)
            Instantiate(new GameObject().AddComponent<RectTransform>(), trinketButtonContainer);

        //spawn world buttons
        SpawnWorldButtons();

        //spawn rank button
        Button rankButton = SpawnButton("rank", rankTrinketsSprite);
        rankButton.onClick.AddListener(() =>
        {
            ToggleContainers(showButtons: false);
            SpawnSetOfTrinkets(trinketsRank, "Rank");
        });

        //spawn shop button
        Button shopButton = SpawnButton("shop", shopTrinketsSprite);
        shopButton.onClick.AddListener(() =>
        {
            ToggleContainers(showButtons: false);
            SpawnSetOfTrinkets(trinketsShop, "Shop");
        });

        //spawn 2 empty objects at the beggining to make scroll offset (2 for 2 rows)
        for (int i = 0; i < 2; i++)
            Instantiate(new GameObject().AddComponent<RectTransform>(), trinketButtonContainer);

        trinketButtonContainer.GetComponent<RectTransform>().anchoredPosition += Vector2.right * 2500;
    }

    void SpawnWorldButtons()
    {
        for (int i = 0; i < worlds.Length; i++)
        {
            Button button = SpawnButton(worlds[i].id, worlds[i].GetRewardSprite());
            int counter = 0;
            LevelTemplate[] trinketsToSpawn = new LevelTemplate[10];
            for (int j = i * 10; j < i * 10 + 10; j++)//loop 10 times starting from world index i*10(10 trinkets for each world)
            {
                trinketsToSpawn[counter] = trinkets[j];
                counter++;
            }
            string worldName = worlds[i].id;
            button.onClick.AddListener(() =>
            {
                ToggleContainers(showButtons: false);
                SpawnSetOfTrinkets(trinketsToSpawn, worldName);
            });
        }
    }

    void SpawnSetOfTrinkets(LevelTemplate[] trinkets, string setName)
    {
        if (trinketsContainer.childCount > 3)
            ClearContainer(trinketsContainer, trinketSelectionGlow);//clear container from last trinkets
        var trinketsUnlocked = GameData.gameData.saveData.trinketIds;
        int numberOfUnlocked = 0;

        //spawn 2 empty objects at the beggining to make scroll offset (2 for 2 rows)
        for (int i = 0; i < 2; i++)
            Instantiate(new GameObject().AddComponent<RectTransform>(), trinketsContainer);

        for (int j = 0; j < trinkets.Length; j++)
        {
            var trinket = Instantiate(trinketTemplate, trinketsContainer).transform;
            trinket.gameObject.name = trinkets[j].id;
            Image image = trinket.GetComponent<Image>();
            image.sprite = trinkets[j].trinketSprite;
            Button button = trinket.GetComponent<Button>();
            string trinkName = trinkets[j].id;
            string descr = "<color=#ff0048><size=320>" + trinkName + "</size></color>";

            LevelTemplate trinketToClick = trinkets[j];
            if (!trinketsUnlocked.Contains(trinkets[j].id))
            {
                trinket.GetChild(0).gameObject.SetActive(true);  //lock image
                image.material = blackAndWhiteMat;
                button.onClick.AddListener(delegate () { OnTrinketClicked(descr, trinketToClick, false); });
            }
            else
            {
                button.onClick.AddListener(delegate () { OnTrinketClicked(descr, trinketToClick, true); });
                numberOfUnlocked++;
            }
            button.onClick.AddListener(delegate () { SetSelectionGlowPos(trinketSelectionGlow, trinket.position); });
        }
        //spawn 2 empty objects at the beggining to make scroll offset (2 for 2 rows)
        for (int i = 0; i < 2; i++)
            Instantiate(new GameObject().AddComponent<RectTransform>(), trinketsContainer);

        trinketsContainer.GetComponent<RectTransform>().anchoredPosition += Vector2.right * trinkets.Length * 30;

        unlockNumber.text = "<size=400>" + numberOfUnlocked + "</size>/" + trinkets.Length;
        sectionName.text = "<color=#ff0048>TRINKETS</color>\n" + SECTION_NAME_DOTS
            + SequentialText.SizeString("\n//" + setName, 170);
        itemDescription.text = "";
    }
    void ToggleContainers(bool showButtons = true)
    {
        trinketButtonContainer.gameObject.SetActive(showButtons);
        trinketsContainer.gameObject.SetActive(!showButtons);

        var scrollRect = trinketButtonContainer.GetComponentInParent<ScrollRect>();
        if (scrollRect)
            scrollRect.content = showButtons ?
                (RectTransform)trinketButtonContainer : (RectTransform)trinketsContainer;
    }
    void OnTrinketClicked(string description, LevelTemplate trinket, bool isUnlocked)
    {
            descriptionImage.gameObject.SetActive(true);
        if (!isUnlocked)
        {
            equipButton.gameObject.SetActive(false);    //button to equip a avatar
            string unlockRequirement = GetUnlockRequirement(LevelReward.Trinket, trinket);
            description += "\n" + LOCKED + "\n" + unlockRequirement;
        }
        else
        {
            description += "\n" + trinket.description;
            equipButton.gameObject.SetActive(true);
            Text buttoText = equipButton.GetComponentInChildren<Text>();
            if (trinket.trinketSprite == profileHandler.GetCurrAvatar())  //set button text if avatar is equiped or not
            {
                buttoText.text = "EQUIPPED";
            }
            else
            {
                buttoText.text = "EQUIP";
            }
            Button button = equipButton.GetComponent<Button>();
            button.onClick.AddListener(delegate () { SetAvatarEquipButton(trinket.trinketSprite); });
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
        if (currCollectionSection == CollectionSection.Title) return;
        descriptionImage.gameObject.SetActive(false);
        SetTitles();
        currCollectionSection = CollectionSection.Title;
        var titlesUnlocked = GameData.gameData.saveData.titleIds;
        unlockNumber.text = "<size=400>" + titlesUnlocked.Count
            + "</size>/" + titles.Length + "\n<color=black>OWNED</color>";
        sectionName.text = "<color=#ff0048>TITLES</color>\n" + SECTION_NAME_DOTS;
        itemDescription.text = "";
        equipButton.gameObject.SetActive(false);
    }
    public void SetTitles()
    {
        if (titlesContainer.childCount > 3) return;
        //ClearContainer(titlesContainer);
        var titlesUnlocked = GameData.gameData.saveData.titleIds;

        for (int i = 0; i < titles.Length; i++)
        {
            var titlePanel = Instantiate(titlePrefab, titlesContainer).transform;
            string title = titles[i].id;
            titlePanel.gameObject.name = title;
            titlePanel.GetComponentInChildren<Text>().text = title;
            Button button = titlePanel.GetComponent<Button>();
            string descr = "<color=#ff0048><size=320>" + title + "</size></color>";
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
        //spawn empty object at the end to make scroll offset
        Instantiate(new GameObject().AddComponent<RectTransform>(), titlesContainer);
        //move container down to show first titles
        titlesContainer.GetComponent<RectTransform>().anchoredPosition += Vector2.down * 8000;
    }
    void OnTitleClicked(string description, int index, bool isUnlocked)
    {
            descriptionImage.gameObject.SetActive(true);
        string title = titles[index].id;
        if (!isUnlocked)
        {
            equipButton.gameObject.SetActive(false);    //button to equip a title
            string unlockRequirement = GetUnlockRequirement(LevelReward.Title, titles[index]);
            description += "\n" + LOCKED + "\n" + unlockRequirement;
        }
        else
        {
            equipButton.gameObject.SetActive(true);
            Text buttoText = equipButton.GetComponentInChildren<Text>();
            if (title == profileHandler.GetCurrentTitle())  //set button text if title is equiped or not
            {
                buttoText.text = "EQUIPPED";
            }
            else
            {
                equipButton.onClick.AddListener(delegate () { SetTitleEquipButton(title); });
                buttoText.text = "EQUIP";
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
        if (currCollectionSection == CollectionSection.Banner) return;
        descriptionImage.gameObject.SetActive(false);
        SetBanners();
        currCollectionSection = CollectionSection.Banner;
        var bannersUnlocked = GameData.gameData.saveData.bannerIds;
        unlockNumber.text = "<size=400>" + bannersUnlocked.Count
            + "</size>/" + banners.Length + "\n<color=black>OWNED</color>";
        sectionName.text = "<color=#ff0048>BANNERS</color>\n" + SECTION_NAME_DOTS;
        itemDescription.text = "";
        equipButton.gameObject.SetActive(false);
    }
    public void SetBanners()
    {
        if (bannersContainer.childCount > 3) return;
        //ClearContainer(bannersContainer);
        var bannersUnlocked = GameData.gameData.saveData.bannerIds;

        for (int i = 0; i < banners.Length; i++)
        {
            var bannerPanel = Instantiate(bannerPanelPrefab, bannersContainer).transform;
            bannerPanel.gameObject.name = banners[i].id;
            Image image = bannerPanel.GetComponent<Image>();
            image.sprite = banners[i].Sprite;
            if(banners[i].Material != null)
            {
                image.material = banners[i].Material;
                if (!string.IsNullOrEmpty(banners[i].animatorValues.propertyName))//has property to animate
                {
                    var matAnimator = bannerPanel.gameObject.AddComponent<MatPropertyAnim>();
                    matAnimator.SetValues(banners[i].animatorValues);
                    matAnimator.Animate();
                }
            }

            Button button = bannerPanel.GetComponent<Button>();
            string bannerName = banners[i].id;
            string descr = "<color=#ff0048><size=320>" + bannerName + "</size></color>";
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
        //spawn empty object at the end to make scroll offset
        Instantiate(new GameObject().AddComponent<RectTransform>(), bannersContainer);
        //move container down to show first banners
        bannersContainer.GetComponent<RectTransform>().anchoredPosition += Vector2.down * 8000;
    }
    void OnBannerClicked(string description, int index, bool isUnlocked)
    {
            descriptionImage.gameObject.SetActive(true);
        if (!isUnlocked)
        {
            equipButton.gameObject.SetActive(false);    //button to equip a banner
            string unlockRequirement = GetUnlockRequirement(LevelReward.Banner, banners[index]);
            description += "\n" + LOCKED + "\n" + unlockRequirement;
        }
        else
        {
            description += "\n<size=250>" + banners[index].description + "</size>";
            equipButton.gameObject.SetActive(true);
            Text buttoText = equipButton.GetComponentInChildren<Text>();
            if (banners[index].Sprite == profileHandler.GetCurrentBanner())  //set button text if banner is equiped or not
            {
                buttoText.text = "EQUIPPED";
            }
            else
            {
                buttoText.text = "EQUIP";
            }
            Button button = equipButton.GetComponent<Button>();
            button.onClick.AddListener(delegate () { SetBannerEquipButton(index); });
        }

        itemDescription.text = description;
    }
    void SetBannerEquipButton(int index)
    {
        string spritePath = BANNERS_LOCATION + banners[index].Sprite.name;
        string materialPath = "";
        if (banners[index].Material != null)
            materialPath = BANNERS_LOCATION + banners[index].Material.name;
        GameData.gameData.ChangeBanner(spritePath + "|" + materialPath, banners[index].animatorValues);//save banner path 
        profileHandler.UpdateBanner(banners[index].Sprite, banners[index].Material, banners[index].animatorValues);//update banner in profile panel
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
                    itemDescription.text = "<color=#ff0048><size=450>" + reward.id + "</size></color>"
                    + "\n<size=200>" + reward.description + "</size>";
                });
                break;
            }
        }
    }
    public static GameObject GetChildWithTag(Transform parent, string tag)
    {
        foreach (Transform child in parent)
        {
            if (child.tag.CompareTo(tag) == 0)
                return child.gameObject;
        }
        return null;
    }
    public static string GetUnlockRequirement(LevelReward levelReward, RewardTemplate rewardTemplate)
    {
        if(!string.IsNullOrEmpty(rewardTemplate.unlockRequirement))//specific requirement
        {
            return rewardTemplate.unlockRequirement;
        }

        //find at what rank reward is unlocked
        int rankToUnlock = RewardForLevel.Instance.GetRankFromRewards(levelReward, rewardTemplate.id);

        if (rankToUnlock < 0)   //negative if not found in rewards
        {
            return UNLOCKED_IN_SHOP;
        }
        else
        {
            //return "<color=black><size=250>" + UNLOCKED_BY_RANK + rankToUnlock + ".</size></color>";
            return UNLOCKED_BY_RANK + rankToUnlock;
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