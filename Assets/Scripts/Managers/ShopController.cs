using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    public static ShopController Instance;

    [SerializeField] Text sectionName;
    [SerializeField] Text unlockNumber;
    [SerializeField] Text itemDescription;
    [SerializeField] Text costText;
    [SerializeField] TextMeshProUGUI costNoSaleText;
    [SerializeField] Button buyButton;
    [SerializeField] Color equipColor;
    [SerializeField] Color buyColor;
    [SerializeField] Color greenColor;
    [SerializeField] ProfileHandler profileHandler;
    [SerializeField] GameObject buyPart;

    [Header("Welcome Screen")]
    [SerializeField] Color welcomeSectionColor;
    [SerializeField] GameObject welcomeScreen;
    [SerializeField] Button saleItemButton1;
    [SerializeField] Button saleItemButton2;
    [SerializeField] Transform saleSelectionGlow;
    [SerializeField] Sprite titleIcon, bannerIcon;
    [SerializeField] Sprite noSaleSprite;
    [Range(0, 100)] [SerializeField] int saleMin, saleMax;

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

    RewardTemplate currChosenSaleItem;
    RewardTemplate firstPickedSaleItem, secondPickedSaleItem;
    int sale1, sale2;
    Dictionary<LevelReward, RewardTemplate[]> allBuyableItems;

    const string SECTION_NAME_DOTS = "<size=120><color=black>- - - - - - - - - - -</color></size>";
    const string LOCKED = "<color=#ff0048>- LOCKED -</color>";

    const string BANNERS_LOCATION = "Sprites/UI images/Banners/";
    const string TRINKETS_LOCATION = "Sprites/UI images/Trinkets/";

    void Start()
    {
        FillBuyableItemsDict();
        costText.transform.parent.gameObject.SetActive(false);
        SetItemsForSale();
        SetWorlds();
        SetBoosts();
        SetTrinkets();
        SetTitles();
        SetBanners();
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

        if (DateTime.Now.CompareTo(lastClaim.AddHours(12)) < 0)
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
        welcomeScreen.SetActive(true);
        SetUIElementsActiveness(false, false, false);
        unlockNumber.text = "";
        sectionName.text = SequentialText.ColorString("WELCOME\n", welcomeSectionColor) + SECTION_NAME_DOTS;
    }

    #region Welcome
    void SetItemsForSale()
    {
        if (TimeHasPassed())    //new sale every 12 hours
        {
            firstPickedSaleItem = PickRandomItemForSale();
            secondPickedSaleItem = PickRandomItemForSale(firstPickedSaleItem);
            sale1 = GetRandomSale();
            sale2 = GetRandomSale();

            if (firstPickedSaleItem != null)
                GameData.gameData.saveData.saleItemsInfo[0]
                    = (firstPickedSaleItem.reward, firstPickedSaleItem.id, sale1);
            if (secondPickedSaleItem != null)
                GameData.gameData.saveData.saleItemsInfo[1]
                    = (secondPickedSaleItem.reward, secondPickedSaleItem.id, sale2);
            GameData.Save();
        }
        else
        {
            var saleInfo1 = GameData.gameData.saveData.saleItemsInfo[0];
            var saleInfo2 = GameData.gameData.saveData.saleItemsInfo[1];

            firstPickedSaleItem = !string.IsNullOrEmpty(saleInfo1.Item2) ?
                CollectionController.FindItemWithId(allBuyableItems[saleInfo1.Item1], saleInfo1.Item2) : null;
            secondPickedSaleItem = !string.IsNullOrEmpty(saleInfo2.Item2) ?
                CollectionController.FindItemWithId(allBuyableItems[saleInfo2.Item1], saleInfo2.Item2) : null;
            sale1 = saleInfo1.Item3;
            sale2 = saleInfo2.Item3;
        }

        saleItemButton1.onClick.AddListener(delegate ()
        {
            SetSaleSelection(saleItemButton1.transform.position);
            currChosenSaleItem = firstPickedSaleItem;
        });
        saleItemButton2.onClick.AddListener(delegate ()
        {
            SetSaleSelection(saleItemButton2.transform.position);
            currChosenSaleItem = secondPickedSaleItem;
        });

        SetSaleItemUI(ref saleItemButton1, firstPickedSaleItem, sale1);
        SetSaleItemUI(ref saleItemButton2, secondPickedSaleItem, sale2);
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
        /*if (AmountOfBoughtItems(rewardType) == allBuyableItems[rewardType].Length) // all items purchased
        {
            return null;
        }
        else
        {
            RewardTemplate pickedReward;
            do
            {
                RewardTemplate[] rewards = allBuyableItems[rewardType];
                int randIndex = UnityEngine.Random.Range(0, rewards.Length);
                pickedReward = rewards[randIndex];
            }
            while (pickedReward == reward || GameData.IsItemCollected(pickedReward));

            return pickedReward;
        }*/
    }
    public void GetOfferButton()
    {
        if (currChosenSaleItem == null)
            return;
        Transform container = null;
        switch (currChosenSaleItem.reward)
        {
            case LevelReward.World:
                {
                    worldTogle.isOn = true;
                    container = worldsContainer;
                    break;
                }
            case LevelReward.Boost:
                {
                    boostTogle.isOn = true;
                    container = boostsContainer;
                    break;
                }
            case LevelReward.Trinket:
                {
                    trinketTogle.isOn = true;
                    container = trinketsContainer;
                    break;
                }
            case LevelReward.Title:
                {
                    titleTogle.isOn = true;
                    container = titlesContainer;
                    break;
                }
            case LevelReward.Banner:
                {
                    bannerTogle.isOn = true;
                    container = bannersContainer;
                    break;
                }
        }

        foreach (Transform child in container)
        {
            if (child.gameObject.name == currChosenSaleItem.id)
            {
                StartCoroutine(PressButtonDelayed(child.GetComponent<Button>()));
                break;
            }
        }
    }
    IEnumerator PressButtonDelayed(Button button)
    {
        yield return new WaitForSeconds(0.01f);
        button.onClick.Invoke();
    }
    void SetSaleSelection(Vector3 pos)
    {
        saleSelectionGlow.position = pos;
    }
    #endregion
    #region World
    public void SetWorldShopTexts()   //called on section toggles event
    {
        unlockNumber.text = "<size=400>" + AmountOfBoughtItems(LevelReward.World)
            + "</size>/" + worlds.Length + "\n<color=black>BOUGHT</color>";
        Color myColor = new Color(1, 0, 0.282f);
        sectionName.text = SequentialText.ColorString("WORLDS\n", myColor) + SECTION_NAME_DOTS;
        itemDescription.text = "";
        SetUIElementsActiveness(false, false, false);
    }
    public void SetWorlds()
    {
        var worldsUnlocked = GameData.gameData.saveData.worldIds;

        //spawn 2 empty objects at the beggining to make scroll offset
        Instantiate(new GameObject().AddComponent<RectTransform>(), worldsContainer);
        Instantiate(new GameObject().AddComponent<RectTransform>(), worldsContainer);
        for (int i = 0; i < worlds.Length; i++)
        {
            string worldName = worlds[i].id;
            Transform worldPanel = Instantiate(worldTemplate, worldsContainer).transform;
            worldPanel.gameObject.name = worlds[i].id;
            worldPanel.GetComponent<Image>().sprite = worlds[i].GetRewardSprite();
            Button button = worldPanel.GetComponent<Button>();
            Color myColor = new Color(1, 0, 0.282f);
            string descr = SequentialText.ColorString("<size=410>" + worldName + "</size>", myColor);
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
        //spawn 2 empty objects at the end to make scroll offset
        Instantiate(new GameObject().AddComponent<RectTransform>(), worldsContainer);
        Instantiate(new GameObject().AddComponent<RectTransform>(), worldsContainer);
    }
    void OnWorldClicked(string description, int index, bool isUnlocked, Transform worldPanel = null)
    {
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
        unlockNumber.text = "<size=400>" + AmountOfBoughtItems(LevelReward.Boost)
            + "</size>/" + boostInfos.Length + "\n<color=black>BOUGHT</color>";
        sectionName.text = "<color=#ff0048>BOOSTS</color>\n" + SECTION_NAME_DOTS;
        itemDescription.text = "";
        SetUIElementsActiveness(false, false, false);
    }
    void SetBoosts()
    {
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
            string descr = "<color=#ff0048><size=450>" + title + "</size></color>";
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
    }
    void OnBoostClicked(string description, int index, bool isUnlocked, Transform boostPanel = null)
    {
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
        unlockNumber.text = "<size=400>" + AmountOfBoughtItems(LevelReward.Trinket)
            + "</size>/" + trinkets.Length + "\n<color=black>BOUGHT</color>";
        sectionName.text = "<color=#ff0048>TRINKETS</color>\n" + SECTION_NAME_DOTS;
        itemDescription.text = "";
        SetUIElementsActiveness(false, false, false);
    }
    void SetTrinkets()
    {
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
            string descr = SequentialText.ColorString("<size=420>" + trinkName + "</size>", myColor);

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
    }
    void OnTrinketClicked(string description, int index, bool isUnlocked, Transform trinketObj = null)
    {
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
        unlockNumber.text = "<size=400>" + AmountOfBoughtItems(LevelReward.Title)
            + "</size>/" + titles.Length + "\n<color=black>BOUGHT</color>";
        sectionName.text = "<color=#ff0048>TITLES</color>\n" + SECTION_NAME_DOTS;
        itemDescription.text = "";
        SetUIElementsActiveness(false, false, false);
    }
    void SetTitles()
    {
        var titlesUnlocked = GameData.gameData.saveData.titleIds;

        for (int i = 0; i < titles.Length; i++)
        {
            var titlePanel = Instantiate(titlePrefab, titlesContainer).transform;
            titlePanel.gameObject.name = titles[i].id;
            string title = titles[i].id.ToString();
            titlePanel.GetComponentInChildren<Text>().text = title;
            Button button = titlePanel.GetComponent<Button>();
            string descr = "<color=#ff0048><size=450>" + title + "</size></color>";
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
    }
    void OnTitleClicked(string description, int index, bool isUnlocked, Transform titlePanel = null)
    {
        string title = titles[index].id;
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
        unlockNumber.text = "<size=400>" + AmountOfBoughtItems(LevelReward.Banner)
            + "</size>/" + banners.Length + "\n<color=black>BOUGHT</color>";
        Color myColor = new Color(1, 0, 0.282f);
        sectionName.text = SequentialText.ColorString("BANNERS\n", myColor) + SECTION_NAME_DOTS;
        itemDescription.text = "";
        SetUIElementsActiveness(false, false, false);
    }
    void SetBanners()
    {
        var bannersUnlocked = GameData.gameData.saveData.bannerIds;

        for (int i = 0; i < banners.Length; i++)
        {
            var bannerPanel = Instantiate(bannerPanelPrefab, bannersContainer).transform;
            bannerPanel.gameObject.name = banners[i].id;
            bannerPanel.GetComponent<Image>().sprite = banners[i].Sprite;
            Button button = bannerPanel.GetComponent<Button>();
            string bannerName = banners[i].id;
            string descr = "<color=#ff0048><size=450>" + bannerName + "</size></color>";
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
    }
    void OnBannerClicked(string description, int index, bool isUnlocked, Transform bannerPanel = null)
    {
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
        Sprite sprite = rewardTemplate.GetRewardSprite();
        GameData.gameData.ChangeBanner(BANNERS_LOCATION + sprite.name);   //save banner path 
        profileHandler.UpdateBanner(sprite);   //update banner in profile panel
    }
    #endregion
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
        if (rewardTemplates[index] == firstPickedSaleItem || rewardTemplates[index] == secondPickedSaleItem)
        {
            costNoSaleText.gameObject.SetActive(true);
            costNoSaleText.text = "x" + rewardTemplates[index].cost;
            if (rewardTemplates[index] == firstPickedSaleItem)
            {
                int cost1 = (int)(firstPickedSaleItem.cost * (100 - sale1) / 100.0f);
                costText.text = "x" + cost1;
            }
            else
            {
                int cost2 = (int)(secondPickedSaleItem.cost * (100 - sale2) / 100.0f);
                costText.text = "x" + cost2;
            }
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
    bool TryBuyItem(RewardTemplate rewardItem, Transform itemTransform)
    {
        buyButton.onClick.RemoveAllListeners();
        string id = rewardItem.id;
        int cost = rewardItem.cost;
        if (rewardItem == firstPickedSaleItem)
        {
            cost = (int)(rewardItem.cost * (100 - sale1) / 100.0f);
        }
        else if (rewardItem == secondPickedSaleItem)
        {
            cost = (int)(rewardItem.cost * (100 - sale2) / 100.0f);
        }

        if (CoinsDisplay.Instance.GetCoins() < cost) return false;

        switch (rewardItem.reward)
        {
            case LevelReward.World:
                {
                    GameData.gameData.UnlockWorld(id);
                    CollectionController.Instance.SetWorlds();
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