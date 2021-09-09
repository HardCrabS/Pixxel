using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    public static ShopController Instance;

    [SerializeField] UI_Screen shopScreen;
    [SerializeField] GameObject buyCoinsPopup;
    [SerializeField] Text sectionName;
    [SerializeField] Text unlockNumber;
    [SerializeField] Text itemDescription;
    [SerializeField] Text costText;
    [SerializeField] Image descriptionImage;
    [SerializeField] TextMeshProUGUI costNoSaleText;
    [SerializeField] Button buyButton;
    [SerializeField] Color equipColor;
    [SerializeField] Color buyColor;
    [SerializeField] Color greenColor;
    [SerializeField] ProfileHandler profileHandler;
    [SerializeField] GameObject buyPart;

    [Header("Welcome Screen")]
    [SerializeField] Text timeUntilSaleText;
    [SerializeField] Color welcomeSectionColor;
    [SerializeField] GameObject welcomeScreen;
    //[SerializeField] Button saleItemButton1;
    [SerializeField] Button saleItemButton2;
    //[SerializeField] Transform saleSelectionGlow;
    [SerializeField] Sprite titleIcon, bannerIcon;
    [SerializeField] Sprite noSaleSprite;
    [Range(0, 100)] [SerializeField] int saleMin, saleMax;

    [Header("IAP Coins")]
    [SerializeField] GameObject coinsScreen;

    [Header("Worlds")]
    [SerializeField] Transform worldsContainer;
    [SerializeField] Transform worldSelectionGlow;
    [SerializeField] GameObject worldTemplate;
    [SerializeField] WorldInformation[] worlds;

    [Header("Boosts")]
    [SerializeField] Transform boostsContainer;
    [SerializeField] Transform boostSelectionGlow;
    [SerializeField] GameObject boostTemplate;
    [SerializeField] Sprite[] levelFrames;
    [SerializeField] Boost[] boostInfos;

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

    [Header("Shop Toggles")]
    [SerializeField] Toggle worldTogle;
    [SerializeField] Toggle boostTogle;
    [SerializeField] Toggle trinketTogle;
    [SerializeField] Toggle titleTogle;
    [SerializeField] Toggle bannerTogle;

    [Header("Audio")]
    [SerializeField] AudioClip buySuccess;
    [SerializeField] AudioClip buyFailed;

    AudioSource audioSource;

    CollectionSection currCollectionSection = CollectionSection.None;

    RewardTemplate pickedSaleItem;
    int saleValue;
    Dictionary<LevelReward, RewardTemplate[]> allBuyableItems;

    const string SECTION_NAME_DOTS = "<size=120><color=black>- - - - - - - - - - -</color></size>";
    const string LOCKED = "<color=#ff0048>- LOCKED -</color>";

    const string BANNERS_LOCATION = "Sprites/UI images/Banners/";
    const string TRINKETS_LOCATION = "Sprites/UI images/Trinkets/";

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        FillBuyableItemsDict();
        costText.transform.parent.gameObject.SetActive(false);
        SetItemsForSale();
        //SetWorlds();
        //SetBoosts();
        //SetTrinkets();
        //SetTitles();
        //SetBanners();
        OpenShop();
        SetBuyCoinsButton();
        audioSource = GetComponent<AudioSource>();
    }
    bool TimeHasPassed()
    {
        string lastClaimStr = GameData.gameData.saveData.lastTimeSaleClaimed;
        DateTime lastClaim;
        if (string.IsNullOrEmpty(lastClaimStr))
        {
            lastClaim = new DateTime(2017, 2, 20);
        }
        else
        {
            lastClaim = Convert.ToDateTime(lastClaimStr);
        }

        if (DateTime.Now.CompareTo(lastClaim.AddHours(6)) < 0)
        {
            return false;
        }
        GameData.gameData.saveData.lastTimeSaleClaimed = DateTime.Now.ToString();
        GameData.Save();
        return true;
    }
    void FillBuyableItemsDict()
    {
        allBuyableItems = new Dictionary<LevelReward, RewardTemplate[]>();
        allBuyableItems.Add(LevelReward.World, worlds);
        allBuyableItems.Add(LevelReward.Boost, boostInfos);
        allBuyableItems.Add(LevelReward.Trinket, trinkets);
        allBuyableItems.Add(LevelReward.Title, titles);
        allBuyableItems.Add(LevelReward.Banner, banners);
    }
    public void OpenShop()
    {
        worldTogle.isOn = false;
        boostTogle.isOn = false;
        trinketTogle.isOn = false;
        titleTogle.isOn = false;
        bannerTogle.isOn = false;
        descriptionImage.gameObject.SetActive(false);
        coinsScreen.SetActive(false);
        welcomeScreen.SetActive(true);
        SetUIElementsActiveness(false, false, false);
        unlockNumber.text = "";
        itemDescription.text = "";
        sectionName.text = SequentialText.ColorString("WELCOME\n", welcomeSectionColor) + SECTION_NAME_DOTS;
    }

    public void CoinsIAP()
    {
        coinsScreen.SetActive(true);
        welcomeScreen.SetActive(false);
        sectionName.text = SequentialText.ColorString("COIN SHOP!\n", welcomeSectionColor) + SECTION_NAME_DOTS;
    }

    public void OpenShopItem(RewardTemplate item)
    {
        UI_System.Instance.SwitchScreens(shopScreen);

        RectTransform container = null;
        switch (item.reward)
        {
            case LevelReward.World:
                {
                    worldTogle.isOn = true;
                    container = (RectTransform)worldsContainer;
                    break;
                }
            case LevelReward.Boost:
                {
                    boostTogle.isOn = true;
                    container = (RectTransform)boostsContainer;
                    break;
                }
            case LevelReward.Trinket:
                {
                    trinketTogle.isOn = true;
                    container = (RectTransform)trinketsContainer;
                    break;
                }
            case LevelReward.Title:
                {
                    titleTogle.isOn = true;
                    container = (RectTransform)titlesContainer;
                    break;
                }
            case LevelReward.Banner:
                {
                    bannerTogle.isOn = true;
                    container = (RectTransform)bannersContainer;
                    break;
                }
        }

        foreach (RectTransform child in container)
        {
            if (child.gameObject.name == item.id)
            {
                StartCoroutine(PressButtonDelayed(child.GetComponent<Button>()));
                SnapTo(child, container, container.GetComponentInParent<ScrollRect>());
                break;
            }
        }
    }

    #region Welcome
    void SetItemsForSale()
    {
        descriptionImage.gameObject.SetActive(true);
        if (TimeHasPassed())    //new sale every 6 hours
        {
            //firstPickedSaleItem = PickRandomItemForSale();
            pickedSaleItem = PickRandomItemForSale();
            //sale1 = GetRandomSale();
            saleValue = GetRandomSale();

            /*if (firstPickedSaleItem != null)
                GameData.gameData.saveData.saleItemsInfo[0]
                    = (firstPickedSaleItem.reward, firstPickedSaleItem.id, sale1);*/
            if (pickedSaleItem != null)
                GameData.gameData.saveData.saleItemsInfo[1]
                    = (pickedSaleItem.reward, pickedSaleItem.id, saleValue);
            GameData.Save();
        }
        else
        {
            var saleInfo1 = GameData.gameData.saveData.saleItemsInfo[0];
            var saleInfo2 = GameData.gameData.saveData.saleItemsInfo[1];

            /*firstPickedSaleItem = !string.IsNullOrEmpty(saleInfo1.Item2) ?
                CollectionController.FindItemWithId(allBuyableItems[saleInfo1.Item1], saleInfo1.Item2) : null;*/
            pickedSaleItem = !string.IsNullOrEmpty(saleInfo2.Item2) ?
                CollectionController.FindItemWithId(allBuyableItems[saleInfo2.Item1], saleInfo2.Item2) : null;
            //sale1 = saleInfo1.Item3;
            saleValue = saleInfo2.Item3;
        }

        /*saleItemButton1.onClick.AddListener(delegate ()
        {
            SetSelectionGlowPos(saleSelectionGlow, saleItemButton1.transform.position);
            currChosenSaleItem = firstPickedSaleItem;
        });*/
        saleItemButton2.onClick.AddListener(delegate ()
        {
            GetOfferButton();
        });

        //SetSaleItemUI(ref saleItemButton1, firstPickedSaleItem, sale1);
        SetSaleItemUI(ref saleItemButton2, pickedSaleItem, saleValue);

        UpdateTimeUntilSaleText();
    }
    int GetRandomSale()
    {
        //picking random int from (min/5) and (max/5) => rand(0, 20) * 5 == rand(0, 100). Allows rand nums with fives (15, 65)
        int s1 = (int)UnityEngine.Random.Range(saleMin * 0.2f, (saleMax + 1) * 0.2f) * 5; // x * 0.2 == x / 5
        int s2 = (int)UnityEngine.Random.Range(saleMin * 0.2f, (saleMax + 1) * 0.2f) * 5;

        return s1 < s2 ? s1 : s2; //getting lower num of two random
    }
    void SetSaleItemUI(ref Button saleItemObj, RewardTemplate pickedSaleItem, int sale)
    {
        Image image = saleItemObj.GetComponent<Image>();

        if (pickedSaleItem == null)
        {
            image.sprite = noSaleSprite;
            saleItemObj.GetComponentInChildren<Text>().text = "OUT OF\nSTOCK!";
        }
        else
        {
            if (pickedSaleItem.reward == LevelReward.Title)
            {
                image.sprite = titleIcon;
            }
            else if (pickedSaleItem.reward == LevelReward.Banner)
            {
                image.sprite = bannerIcon;
            }
            else
            {
                image.sprite = pickedSaleItem.GetRewardSprite();
                if (pickedSaleItem.reward == LevelReward.World)
                {
                    image.gameObject.AddComponent<Outline>().effectDistance = new Vector2(3, 3);
                }
            }
            saleItemObj.GetComponentInChildren<Text>().text = sale + "%\nOFF!";
        }
    }
    RewardTemplate PickRandomItemForSale(RewardTemplate reward = null)
    {
        LevelReward[] rewardTypes =
        {
            LevelReward.World,
            LevelReward.Boost,
            LevelReward.Trinket,
            LevelReward.Title,
            LevelReward.Banner
        };
        int randRewardTypeIndex = UnityEngine.Random.Range(0, rewardTypes.Length);
        LevelReward rewardType = rewardTypes[randRewardTypeIndex];

        RewardTemplate[] rewards = allBuyableItems[rewardType];
        int randIndex = UnityEngine.Random.Range(0, rewards.Length);
        RewardTemplate pickedReward = rewards[randIndex];
        if (pickedReward == reward || GameData.IsItemCollected(pickedReward))
        {
            return null;
        }
        else
            return pickedReward;
    }
    public void GetOfferButton()
    {
        RectTransform container = null;
        switch (pickedSaleItem.reward)
        {
            case LevelReward.World:
                {
                    worldTogle.isOn = true;
                    container = (RectTransform)worldsContainer;
                    break;
                }
            case LevelReward.Boost:
                {
                    boostTogle.isOn = true;
                    container = (RectTransform)boostsContainer;
                    break;
                }
            case LevelReward.Trinket:
                {
                    trinketTogle.isOn = true;
                    container = (RectTransform)trinketsContainer;
                    break;
                }
            case LevelReward.Title:
                {
                    titleTogle.isOn = true;
                    container = (RectTransform)titlesContainer;
                    break;
                }
            case LevelReward.Banner:
                {
                    bannerTogle.isOn = true;
                    container = (RectTransform)bannersContainer;
                    break;
                }
        }

        foreach (RectTransform child in container)
        {
            if (child.gameObject.name == pickedSaleItem.id)
            {
                StartCoroutine(PressButtonDelayed(child.GetComponent<Button>()));
                SnapTo(child, container, container.GetComponentInParent<ScrollRect>());
                break;
            }
        }
    }
    void UpdateTimeUntilSaleText()
    {
        string lastClaimStr = GameData.gameData.saveData.lastTimeSaleClaimed;
        DateTime lastClaim;
        lastClaim = Convert.ToDateTime(lastClaimStr);

        TimeSpan nextClaim = lastClaim.AddHours(6).Subtract(DateTime.Now);
        Color timeColor = new Color(1, 0, 0, 015f);
        string time = SequentialText.ColorString(nextClaim.Hours + ":" + nextClaim.Minutes, timeColor);
        timeUntilSaleText.text = "New deals in " + time + " my friends!";
    }
    IEnumerator PressButtonDelayed(Button button)
    {
        yield return new WaitForSeconds(0.01f);
        button.onClick.Invoke();
    }
    #endregion
    #region World
    public void SetWorldShopTexts()   //called on section toggles event
    {
        if (currCollectionSection == CollectionSection.World) return;
        descriptionImage.gameObject.SetActive(false);
        SetWorlds();
        currCollectionSection = CollectionSection.World;
        unlockNumber.text = "<size=400>" + AmountOfBoughtItems(LevelReward.World)
            + "</size>/" + worlds.Length + "\n<color=black>BOUGHT</color>";
        Color myColor = new Color(1, 0, 0.282f);
        sectionName.text = SequentialText.ColorString("WORLDS\n", myColor) + SECTION_NAME_DOTS;
        itemDescription.text = "";
        SetUIElementsActiveness(false, false, false);
    }
    public void SetWorlds()
    {
        if (worldsContainer.childCount > 3) return;
        var worldsUnlocked = GameData.gameData.saveData.worldIds;

        //spawn 2 empty objects at the beggining to make scroll offset (2 for 2 rows)
        for (int i = 0; i < 2; i++)
            Instantiate(new GameObject().AddComponent<RectTransform>(), worldsContainer);

        for (int i = 0; i < worlds.Length; i++)
        {
            string worldName = worlds[i].id;
            Transform worldPanel = Instantiate(worldTemplate, worldsContainer).transform;
            worldPanel.gameObject.name = worlds[i].id;
            worldPanel.GetComponent<Image>().sprite = worlds[i].GetRewardSprite();
            Button button = worldPanel.GetComponent<Button>();
            Color myColor = new Color(1, 0, 0.282f);
            string descr = SequentialText.ColorString("<size=300>" + worldName + "</size>", myColor);
            int index = i;

            if (!worldsUnlocked.Contains(worlds[i].id))
            {
                button.onClick.AddListener(delegate () { OnWorldClicked(descr, index, false, worldPanel); });
            }
            else
            {
                ActivateSoldOutText(worldPanel, 0);
                button.onClick.AddListener(delegate () { OnWorldClicked(descr, index, true); });
            }
            button.onClick.AddListener(delegate () { SetSelectionGlowPos(worldSelectionGlow, worldPanel.position); });
        }
        //spawn 2 empty objects at the beggining to make scroll offset (2 for 2 rows)
        for (int i = 0; i < 2; i++)
            Instantiate(new GameObject().AddComponent<RectTransform>(), worldsContainer);

        worldsContainer.GetComponent<RectTransform>().anchoredPosition += Vector2.right * 1500;
    }
    void OnWorldClicked(string description, int index, bool isUnlocked, Transform worldPanel = null)
    {
        descriptionImage.gameObject.SetActive(true);
        buyButton.gameObject.SetActive(true); //buy and equip button
        string initDescr = description;
        if (!isUnlocked)
        {
            SetBuyInfo();
            buyButton.onClick.AddListener(delegate ()
            {
                if (!TryBuyItem(worlds[index], worldPanel)) return;
                ActivateSoldOutText(worldPanel, 0);

                itemDescription.text += SequentialText.ColorString("\nSUCCESS! THE WORLD IS UNLOCKED!", buyColor);
                costText.transform.parent.gameObject.SetActive(false);
                buyButton.gameObject.SetActive(false);

                ResetClickEvent(() => OnWorldClicked(initDescr, index, true), worldPanel, worldSelectionGlow);
            }
            );
            SetCostText(worlds, index);
        }
        else
        {
            SetUIElementsActiveness(false, false, false);
        }
        description += SequentialText.ColorString("\n" + worlds[index].description, greenColor);
        itemDescription.text = description;
    }
    #endregion
    #region Boost
    public void SetBoostShopTexts()   //called on section toggles event
    {
        if (currCollectionSection == CollectionSection.Boost) return;
        descriptionImage.gameObject.SetActive(false);
        SetBoosts();
        currCollectionSection = CollectionSection.Boost;
        unlockNumber.text = "<size=400>" + AmountOfBoughtItems(LevelReward.Boost)
            + "</size>/" + boostInfos.Length + "\n<color=black>BOUGHT</color>";
        sectionName.text = "<color=#ff0048>BOOSTS</color>\n" + SECTION_NAME_DOTS;
        itemDescription.text = "";
        SetUIElementsActiveness(false, false, false);
    }
    void SetBoosts()
    {
        if (boostsContainer.childCount > 3) return;
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
            string descr = "<color=#ff0048><size=300>" + title + "</size></color>";
            int index = i;
            if (!boostsUnlocked.Contains(boost.id))
            {
                button.onClick.AddListener(delegate () { OnBoostClicked(descr, index, false, boostPanel); });
            }
            else
            {
                ActivateSoldOutText(boostPanel, 1);

                Image boostImage = boostPanel.GetChild(0).GetComponent<Image>();
                var colors = boostImage.color;
                colors.a *= 0.5f;
                boostImage.color = colors;

                button.onClick.AddListener(delegate () { OnBoostClicked(descr, index, true); });
            }
            button.onClick.AddListener(delegate () { SetSelectionGlowPos(boostSelectionGlow, boostPanel.position); });
        }

        //spawn 2 empty objects at the beggining to make scroll offset (2 for 2 rows)
        for (int i = 0; i < 2; i++)
            Instantiate(new GameObject().AddComponent<RectTransform>(), boostsContainer);

        boostsContainer.GetComponent<RectTransform>().anchoredPosition += Vector2.right * 1500;
    }
    void OnBoostClicked(string description, int index, bool isUnlocked, Transform boostPanel = null)
    {
        descriptionImage.gameObject.SetActive(true);
        buyButton.gameObject.SetActive(true); //buy and equip button
        string initDescr = description;
        if (!isUnlocked)
        {
            SetBuyInfo();
            buyButton.onClick.AddListener(delegate ()
            {
                if (!TryBuyItem(boostInfos[index], boostPanel)) return;
                ActivateSoldOutText(boostPanel, 1);

                itemDescription.text += SequentialText.ColorString("\nSUCCESS! EQUIP BOOST IN A WORLD SELECT MENU!", buyColor);
                costText.transform.parent.gameObject.SetActive(false);
                buyButton.gameObject.SetActive(false);

                ResetClickEvent(() => OnBoostClicked(initDescr, index, true), boostPanel, boostSelectionGlow);
            }
            );
            SetCostText(boostInfos, index);
        }
        else
        {
            SetUIElementsActiveness(false, false, false);
        }
        description += SequentialText.ColorString("\n" + boostInfos[index].description, greenColor);
        itemDescription.text = description;
    }
    #endregion
    #region Trinket
    public void SetTrinketShopTexts()
    {
        if (currCollectionSection == CollectionSection.Trinket) return;
        descriptionImage.gameObject.SetActive(false);
        SetTrinkets();
        currCollectionSection = CollectionSection.Trinket;
        unlockNumber.text = "<size=400>" + AmountOfBoughtItems(LevelReward.Trinket)
            + "</size>/" + trinkets.Length + "\n<color=black>BOUGHT</color>";
        sectionName.text = "<color=#ff0048>TRINKETS</color>\n" + SECTION_NAME_DOTS;
        itemDescription.text = "";
        SetUIElementsActiveness(false, false, false);
    }
    void SetTrinkets()
    {
        if (trinketsContainer.childCount > 3) return;
        var trinketsUnlocked = GameData.gameData.saveData.trinketIds;

        //spawn 3 empty objects at the beggining to make scroll offset (3 for 3 rows)
        for (int i = 0; i < 3; i++)
            Instantiate(new GameObject().AddComponent<RectTransform>(), trinketsContainer);
        for (int j = 0; j < trinkets.Length; j++)
        {
            var trinket = Instantiate(trinketTemplate, trinketsContainer).transform;
            trinket.gameObject.name = trinkets[j].id;
            trinket.GetComponent<Image>().sprite = trinkets[j].trinketSprite;
            Button button = trinket.GetComponent<Button>();
            string trinkName = trinkets[j].id;
            Color myColor = new Color(1, 0, 0.282f);
            string descr = SequentialText.ColorString("<size=300>" + trinkName + "</size>", myColor);

            int index = j;
            if (!trinketsUnlocked.Contains(trinkets[j].id))
            {
                trinket.GetChild(0).gameObject.SetActive(true);  //lock image
                button.onClick.AddListener(delegate () { OnTrinketClicked(descr, index, false, trinket); });
            }
            else
            {
                button.onClick.AddListener(delegate () { OnTrinketClicked(descr, index, true); });
            }
            button.onClick.AddListener(delegate () { SetSelectionGlowPos(trinketSelectionGlow, trinket.position); });
        }
        //spawn 3 empty objects at the end to make scroll offset (3 for 3 rows)
        for (int i = 0; i < 3; i++)
            Instantiate(new GameObject().AddComponent<RectTransform>(), trinketsContainer);

        trinketsContainer.GetComponent<RectTransform>().anchoredPosition += Vector2.right * 1500;
    }
    void OnTrinketClicked(string description, int index, bool isUnlocked, Transform trinketObj = null)
    {
        descriptionImage.gameObject.SetActive(true);
        buyButton.gameObject.SetActive(true);
        string initDescr = description;
        if (!isUnlocked)
        {
            SetBuyInfo();
            buyButton.onClick.AddListener(delegate ()
            {
                if (!TryBuyItem(trinkets[index], trinketObj)) return;
                HandleEquipButton(false, () => SetAvatarEquipButton(trinkets[index]));
                GameObject lockGameObject = trinketObj.GetChild(0).gameObject; //lock image
                Destroy(lockGameObject);

                itemDescription.text = initDescr + SequentialText.ColorString("\n" + trinkets[index].description, greenColor); ;
                costText.transform.parent.gameObject.SetActive(false);

                ResetClickEvent(() => OnTrinketClicked(initDescr, index, true), trinketObj, trinketSelectionGlow);
            }
            );
            SetCostText(trinkets, index);
        }
        else
        {
            SetUIElementsActiveness(false, false, true);
            SetEquipButtonColor();
            bool isEquiped = trinkets[index].trinketSprite == profileHandler.GetCurrAvatar();
            HandleEquipButton(isEquiped, () => SetAvatarEquipButton(trinkets[index]));
        }
        description += SequentialText.ColorString("\n" + trinkets[index].description, greenColor);
        itemDescription.text = description;
    }
    void SetAvatarEquipButton(RewardTemplate rewardTemplate)
    {
        Sprite avatar = rewardTemplate.GetRewardSprite();
        GameData.gameData.ChangeAvatar(TRINKETS_LOCATION + avatar.name);   //save avatar 
        profileHandler.UpdateAvatar(avatar);   //update avatar in profile panel
    }
    #endregion
    #region Title
    public void SetTitleShopTexts()
    {
        if (currCollectionSection == CollectionSection.Title) return;
        descriptionImage.gameObject.SetActive(false);
        SetTitles();
        currCollectionSection = CollectionSection.Title;
        unlockNumber.text = "<size=400>" + AmountOfBoughtItems(LevelReward.Title)
            + "</size>/" + titles.Length + "\n<color=black>BOUGHT</color>";
        sectionName.text = "<color=#ff0048>TITLES</color>\n" + SECTION_NAME_DOTS;
        itemDescription.text = "";
        SetUIElementsActiveness(false, false, false);
    }
    void SetTitles()
    {
        if (titlesContainer.childCount > 3) return;
        var titlesUnlocked = GameData.gameData.saveData.titleIds;

        for (int i = 0; i < titles.Length; i++)
        {
            var titlePanel = Instantiate(titlePrefab, titlesContainer).transform;
            titlePanel.gameObject.name = titles[i].id;
            string title = titles[i].id.ToString();
            titlePanel.GetComponentInChildren<Text>().text = title;
            Button button = titlePanel.GetComponent<Button>();
            string descr = "<color=#ff0048><size=300>" + title + "</size></color>";
            int index = i;
            if (!titlesUnlocked.Contains(title))
            {
                GameObject lockGameObject = titlePanel.GetChild(1).gameObject; //lock image
                lockGameObject.SetActive(true);
                button.onClick.AddListener(delegate () { OnTitleClicked(descr, index, false, titlePanel); });
            }
            else
            {
                button.onClick.AddListener(delegate () { OnTitleClicked(descr, index, true); });
            }
            button.onClick.AddListener(delegate () { SetSelectionGlowPos(titleSelectionGlow, titlePanel.position); });
        }

        //spawn 1 empty objects at the beggining to make scroll offset
        Instantiate(new GameObject().AddComponent<RectTransform>(), titlesContainer);

        titlesContainer.GetComponent<RectTransform>().anchoredPosition += Vector2.down * 1500;
    }
    void OnTitleClicked(string description, int index, bool isUnlocked, Transform titlePanel = null)
    {
        string title = titles[index].id;
        descriptionImage.gameObject.SetActive(true);
        buyButton.gameObject.SetActive(true); //buy and equip button
        string initDescr = description;
        if (!isUnlocked)
        {
            SetBuyInfo();
            buyButton.onClick.AddListener(delegate ()
            {
                if (!TryBuyItem(titles[index], titlePanel)) return;
                HandleEquipButton(false, () => SetTitleEquipButton(titles[index]));
                GameObject lockGameObject = titlePanel.GetChild(1).gameObject; //lock image
                Destroy(lockGameObject);

                itemDescription.text = initDescr;
                costText.transform.parent.gameObject.SetActive(false);

                ResetClickEvent(() => OnTitleClicked(initDescr, index, true), titlePanel, titleSelectionGlow);
            }
            );
            SetCostText(titles, index);
            description += "\n" + LOCKED + "\n";
        }
        else
        {
            SetUIElementsActiveness(false, false, true);
            SetEquipButtonColor();
            bool isEquiped = title == profileHandler.GetCurrentTitle();
            HandleEquipButton(isEquiped, () => SetTitleEquipButton(titles[index]));
        }

        itemDescription.text = description;
    }
    void SetTitleEquipButton(RewardTemplate rewardTemplate)
    {
        string title = rewardTemplate.id;
        GameData.gameData.ChangeTitle(title);   //save title
        profileHandler.UpdateTitle(title);   //update title in profile panel
    }
    #endregion
    #region Banner
    public void SetBannerShopTexts()
    {
        if (currCollectionSection == CollectionSection.Banner) return;
        descriptionImage.gameObject.SetActive(false);
        SetBanners();
        currCollectionSection = CollectionSection.Banner;
        unlockNumber.text = "<size=400>" + AmountOfBoughtItems(LevelReward.Banner)
            + "</size>/" + banners.Length + "\n<color=black>BOUGHT</color>";
        Color myColor = new Color(1, 0, 0.282f);
        sectionName.text = SequentialText.ColorString("BANNERS\n", myColor) + SECTION_NAME_DOTS;
        itemDescription.text = "";
        SetUIElementsActiveness(false, false, false);
    }
    void SetBanners()
    {
        if (bannersContainer.childCount > 3) return;
        var bannersUnlocked = GameData.gameData.saveData.bannerIds;

        for (int i = 0; i < banners.Length; i++)
        {
            var bannerPanel = Instantiate(bannerPanelPrefab, bannersContainer).transform;
            bannerPanel.gameObject.name = banners[i].id;
            var image = bannerPanel.GetComponent<Image>();
            image.sprite = banners[i].Sprite;
            if (banners[i].Material != null)
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
            string descr = "<color=#ff0048><size=300>" + bannerName + "</size></color>";
            int index = i;
            if (!bannersUnlocked.Contains(bannerName))
            {
                bannerPanel.GetChild(0).gameObject.SetActive(true);  //lock image
                button.onClick.AddListener(delegate () { OnBannerClicked(descr, index, false, bannerPanel); });
            }
            else
            {
                button.onClick.AddListener(delegate () { OnBannerClicked(descr, index, true); });
            }
            button.onClick.AddListener(delegate () { SetSelectionGlowPos(bannerSelectionGlow, bannerPanel.position); });
        }

        //spawn 1 empty objects at the beggining to make scroll offset
        Instantiate(new GameObject().AddComponent<RectTransform>(), bannersContainer);

        bannersContainer.GetComponent<RectTransform>().anchoredPosition += Vector2.down * 1500;
    }
    void OnBannerClicked(string description, int index, bool isUnlocked, Transform bannerPanel = null)
    {
        descriptionImage.gameObject.SetActive(true);
        buyButton.gameObject.SetActive(true);
        string initDescr = description;
        if (!isUnlocked)
        {
            SetBuyInfo();
            buyButton.onClick.AddListener(delegate ()
            {
                if (!TryBuyItem(banners[index], bannerPanel)) return;
                HandleEquipButton(false, () => SetBannerEquipButton(banners[index]));
                GameObject lockGameObject = bannerPanel.GetChild(0).gameObject; //lock image
                Destroy(lockGameObject);

                itemDescription.text = initDescr;
                costText.transform.parent.gameObject.SetActive(false);

                ResetClickEvent(() => OnBannerClicked(initDescr, index, true), bannerPanel, bannerSelectionGlow);
            }
            );
            SetCostText(banners, index);
            description += "\n" + LOCKED + "\n";
        }
        else
        {
            SetUIElementsActiveness(false, false, true);
            SetEquipButtonColor();
            description += "\n<size=250>" + banners[index].description + "</size>";
            bool isEquiped = banners[index].Sprite == profileHandler.GetCurrentBanner();
            HandleEquipButton(isEquiped, () => SetBannerEquipButton(banners[index]));
        }

        itemDescription.text = description;
    }
    void SetBannerEquipButton(RewardTemplate rewardTemplate)
    {
        Banner banner = (Banner)rewardTemplate;
        string spritePath = BANNERS_LOCATION + banner.Sprite.name;
        string materialPath = "";
        if (banner.Material != null)
            materialPath = BANNERS_LOCATION + banner.Material.name;
        GameData.gameData.ChangeBanner(spritePath + "|" + materialPath, banner.animatorValues);//save banner path 
        profileHandler.UpdateBanner(banner.Sprite, banner.Material, banner.animatorValues);//update banner in profile panel
    }
    #endregion

    public static void SnapTo(RectTransform target, RectTransform contentPanel, ScrollRect scrollRect)
    {
        Canvas.ForceUpdateCanvases();

        if (scrollRect.horizontal)
        {
            contentPanel.anchoredPosition = new Vector2(
                ((Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position)
                - (Vector2)scrollRect.transform.InverseTransformPoint(target.position)).x,
                contentPanel.anchoredPosition.y);
        }
        else
        {
            contentPanel.anchoredPosition = new Vector2(
                contentPanel.anchoredPosition.x,
                ((Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position)
                - (Vector2)scrollRect.transform.InverseTransformPoint(target.position)).y);
        }
    }
    private void ResetClickEvent(Action clickFunction, Transform panelClone, Transform selectionGlow)
    {
        Button button = panelClone.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(delegate () { SetSelectionGlowPos(selectionGlow, panelClone.position); });
        button.onClick.AddListener(delegate () { clickFunction(); });
    }
    void SetSelectionGlowPos(Transform selection, Vector3 pos)
    {
        selection.position = pos;
    }
    void SetCostText(RewardTemplate[] rewardTemplates, int index)
    {
        costText.transform.parent.gameObject.SetActive(true);
        if (rewardTemplates[index] == pickedSaleItem)
        {
            costNoSaleText.gameObject.SetActive(true);
            costNoSaleText.text = "x" + rewardTemplates[index].cost;
            int cost2 = (int)(pickedSaleItem.cost * (100 - saleValue) / 100.0f);
            costText.text = "x" + cost2;
        }
        else
        {
            costNoSaleText.gameObject.SetActive(false);
            costText.text = "x" + rewardTemplates[index].cost;
        }
    }
    void SetUIElementsActiveness(bool _costText, bool _saleCostText, bool _buyButton)
    {
        costText.transform.parent.gameObject.SetActive(_costText);
        costNoSaleText.gameObject.SetActive(_saleCostText);
        buyButton.gameObject.SetActive(_buyButton);
    }
    void HandleEquipButton(bool isEquiped, Action equipButtonMethod)
    {
        buyButton.onClick.RemoveAllListeners();
        Text buttoText = buyButton.GetComponentInChildren<Text>();
        if (isEquiped)
        {
            buttoText.text = "ACTIVE!";
        }
        else
        {
            buttoText.text = "EQUIP!";
            buyButton.onClick.AddListener(delegate () { equipButtonMethod(); });
            buyButton.onClick.AddListener(delegate ()
            {
                buyButton.GetComponentInChildren<Text>().text = "ACTIVE!";
            });
        }
    }
    void SetBuyCoinsButton()
    {
        buyCoinsPopup.GetComponentsInChildren<Button>()[1].onClick.AddListener(() =>
        {
            buyCoinsPopup.GetComponent<Animator>().SetTrigger("Hide");

            worldTogle.isOn = false;
            boostTogle.isOn = false;
            trinketTogle.isOn = false;
            titleTogle.isOn = false;
            bannerTogle.isOn = false;
            descriptionImage.gameObject.SetActive(false);
            SetUIElementsActiveness(false, false, false);
            unlockNumber.text = "";
            itemDescription.text = "";

            CoinsIAP();
        });
    }
    bool TryBuyItem(RewardTemplate rewardItem, Transform itemTransform)
    {
        buyButton.onClick.RemoveAllListeners();
        string id = rewardItem.id;
        int cost = rewardItem.cost;
        if (rewardItem == pickedSaleItem)
        {
            cost = (int)(rewardItem.cost * (100 - saleValue) / 100.0f);
        }

        if (CoinsDisplay.Instance.GetCoins() < cost)
        {
            buyCoinsPopup.GetComponent<Animator>().SetTrigger("Show");
            audioSource.PlayOneShot(buyFailed);
            return false;
        }

        switch (rewardItem.reward)
        {
            case LevelReward.World:
                {
                    GameData.gameData.UnlockWorld(id);
                    CollectionController.Instance.ResetWorlds();
                    unlockNumber.text = "<size=400>" + AmountOfBoughtItems(LevelReward.World)
            + "</size>/" + worlds.Length + "\n<color=black>BOUGHT</color>";
                    break;
                }
            case LevelReward.Boost:
                {
                    GameData.gameData.UnlockBoost(id);
                    unlockNumber.text = "<size=400>" + AmountOfBoughtItems(LevelReward.Boost)
            + "</size>/" + boostInfos.Length + "\n<color=black>BOUGHT</color>";
                    break;
                }
            case LevelReward.Trinket:
                {
                    GameData.gameData.UnlockTrinket(id);
                    unlockNumber.text = "<size=400>" + AmountOfBoughtItems(LevelReward.Trinket)
            + "</size>/" + trinkets.Length + "\n<color=black>BOUGHT</color>";
                    buyButton.onClick.AddListener(delegate () { SetAvatarEquipButton(rewardItem); });
                    break;
                }
            case LevelReward.Title:
                {
                    GameData.gameData.UnlockTitle(id);
                    unlockNumber.text = "<size=400>" + AmountOfBoughtItems(LevelReward.Title)
            + "</size>/" + titles.Length + "\n<color=black>BOUGHT</color>";
                    buyButton.onClick.AddListener(delegate () { SetTitleEquipButton(rewardItem); });
                    break;
                }
            case LevelReward.Banner:
                {
                    GameData.gameData.UnlockBanner(id);
                    unlockNumber.text = "<size=400>" + AmountOfBoughtItems(LevelReward.Banner)
            + "</size>/" + banners.Length + "\n<color=black>BOUGHT</color>";
                    buyButton.onClick.AddListener(delegate () { SetBannerEquipButton(rewardItem); });
                    break;
                }
        }
        audioSource.PlayOneShot(buySuccess);
        CoinsDisplay.Instance.DecreaseCoins(cost);
        CollectionController.Instance.UpdateCollectionElement(rewardItem);
        costText.transform.parent.gameObject.SetActive(false);

        SetEquipButtonColor();
        var part = Instantiate(buyPart, itemTransform.position, transform.rotation, itemTransform);
        part.transform.localScale *= 0.5f;
        Destroy(part, 1);
        return true;
    }
    void ActivateSoldOutText(Transform panel, int soldOutChildIndex)
    {
        Transform soldOutText = panel.GetChild(soldOutChildIndex);  //sold out text
        soldOutText.gameObject.SetActive(true);
        float randRotation = UnityEngine.Random.Range(-5f, 5f);
        soldOutText.Rotate(new Vector3(0, 0, randRotation));
    }
    void SetBuyInfo()
    {
        buyButton.onClick.RemoveAllListeners();
        buyButton.GetComponent<Image>().color = buyColor;
        buyButton.GetComponentInChildren<Text>().text = "BUY!";
    }
    void SetEquipButtonColor()
    {
        buyButton.GetComponent<Image>().color = equipColor;
        buyButton.GetComponentInChildren<Text>().text = "EQUIP!";
    }

    int AmountOfBoughtItems(LevelReward levelReward)
    {
        List<string> savedIds = GameData.gameData.allItemsId[levelReward];
        RewardTemplate[] buyableRewards = allBuyableItems[levelReward];

        int amountOfBoughtItems = 0;
        for (int i = 0; i < buyableRewards.Length; i++)
        {
            if (savedIds.Contains(buyableRewards[i].id))
            {
                amountOfBoughtItems++;
            }
        }

        return amountOfBoughtItems;
    }
}