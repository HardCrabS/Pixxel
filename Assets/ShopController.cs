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

    [Header("Welcome Screen")]
    [SerializeField] Color welcomeSectionColor;
    [SerializeField] GameObject welcomeScreen;
    [SerializeField] Button saleItem1;
    [SerializeField] Button saleItem2;
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
    RewardTemplate firstSaleItem, secondSaleItem;
    int sale1, sale2;
    Dictionary<LevelReward, RewardTemplate[]> allBuyableItems;

    const string SECTION_NAME_DOTS = "<size=120><color=blue>- - - - - - - - - - -</color></size>";
    const string LOCKED = "<color=red>- LOCKED -</color>";
    const string UNLOCKED_IN_SHOP = "Unlocked by purchasing in shop! ";
    const string UNLOCKED_BY_RANK = "Unlocked by reaching Player Rank ";

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
        firstSaleItem = PickRandomItemForSale();
        secondSaleItem = PickRandomItemForSale(firstSaleItem);

        saleItem1.onClick.AddListener(delegate ()
        {
            SetSaleSelection(saleItem1.transform.position);
            currChosenSaleItem = firstSaleItem;
        });
        saleItem2.onClick.AddListener(delegate ()
        {
            SetSaleSelection(saleItem2.transform.position);
            currChosenSaleItem = secondSaleItem;
        });

        Image image1 = saleItem1.GetComponent<Image>();
        Image image2 = saleItem2.GetComponent<Image>();
        if (firstSaleItem == null)
        {
            image1.sprite = noSaleSprite;
            saleItem1.GetComponentInChildren<Text>().text = "OUT OF\nSTOCK!";
        }
        else
        {
            if (firstSaleItem.reward == LevelReward.Title)
            {
                image1.sprite = titleIcon;
            }
            else if (firstSaleItem.reward == LevelReward.Banner)
            {
                image1.sprite = bannerIcon;
            }
            else
            {
                image1.sprite = firstSaleItem.GetRewardSprite();
                if (firstSaleItem.reward == LevelReward.World)
                {
                    image1.gameObject.AddComponent<Outline>().effectDistance = new Vector2(3, 3);
                }
            }
            sale1 = GetRandomSale();
            saleItem1.GetComponentInChildren<Text>().text = sale1 + "%\nOFF!";
        }
        if (secondSaleItem == null)
        {
            image2.sprite = noSaleSprite;
            saleItem2.GetComponentInChildren<Text>().text = "OUT OF\nSTOCK!";
        }
        else
        {
            if (secondSaleItem.reward == LevelReward.Title)
            {
                image2.sprite = titleIcon;
            }
            else if (secondSaleItem.reward == LevelReward.Banner)
            {
                image2.sprite = bannerIcon;
            }
            else
            {
                image2.sprite = secondSaleItem.GetRewardSprite();
                if (secondSaleItem.reward == LevelReward.World)
                {
                    image2.gameObject.AddComponent<Outline>().effectDistance = new Vector2(3, 3);
                }
            }
            sale2 = GetRandomSale();
            saleItem2.GetComponentInChildren<Text>().text = sale2 + "%\nOFF!";
        }
    }
    int GetRandomSale()
    {
        //picking random int from (min/5) and (max/5) => rand(0, 20) * 5 == rand(0, 100). Allows rand nums with fives (15, 65)
        int s1 = (int)UnityEngine.Random.Range(saleMin * 0.2f, (saleMax + 1) * 0.2f) * 5; // x * 0.2 == x / 5
        int s2 = (int)UnityEngine.Random.Range(saleMin * 0.2f, (saleMax + 1) * 0.2f) * 5;

        return s1 < s2 ? s1 : s2; //getting lower num of two random
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
            if (child.gameObject.name == currChosenSaleItem.GetRewardId())
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
            + "</size>/" + worlds.Length + "\n<color=blue>BOUGHT</color>";
        sectionName.text = "<color=lime>WORLDS</color>\n" + SECTION_NAME_DOTS;
        itemDescription.text = "";
        SetUIElementsActiveness(false, false, false);
    }
    public void SetWorlds()
    {
        var worldsUnlocked = GameData.gameData.saveData.worldIds;

        for (int i = 0; i < worlds.Length; i++)
        {
            string worldName = worlds[i].WorldName;
            Transform worldPanel = Instantiate(worldTemplate, worldsContainer).transform;
            worldPanel.gameObject.name = worlds[i].GetRewardId();
            worldPanel.GetComponent<Image>().sprite = worlds[i].GetRewardSprite();
            Button button = worldPanel.GetComponent<Button>();
            string descr = SequentialText.ColorString("<size=410>" + worldName + "</size>", buyColor);
            int index = i;

            if (!worldsUnlocked.Contains(worlds[i].GetRewardId()))
            {
                button.onClick.AddListener(delegate () { OnWorldClicked(descr, index, false, worldPanel); });
            }
            else
            {
                ActivateSoldOutText(worldPanel, 0);
                button.onClick.AddListener(delegate () { OnWorldClicked(descr, index, true); });
            }
            button.onClick.AddListener(delegate () { SetWorldSelection(worldPanel.position); });
        }
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
                if (CoinsDisplay.Instance.GetCoins() >= worlds[index].cost)
                {
                    BuyItem(LevelReward.World, worlds[index]);
                    ActivateSoldOutText(worldPanel, 0);

                    itemDescription.text += SequentialText.ColorString("\nSUCCESS! THE WORLD IS UNLOCKED!", buyColor);
                    costText.transform.parent.gameObject.SetActive(false);
                    buyButton.gameObject.SetActive(false);

                    Button button = worldPanel.GetComponent<Button>();
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(delegate () { SetWorldSelection(worldPanel.position); });
                    button.onClick.AddListener(delegate () { OnWorldClicked(initDescr, index, true); });
                }
            }
            );
            SetCostText(worlds, index);
        }
        else
        {
            SetUIElementsActiveness(false, false, false);
        }
        description += SequentialText.ColorString("\n" + worlds[index].Description, greenColor);
        itemDescription.text = description;
    }
    public void SetWorldSelection(Vector3 pos)
    {
        worldSelectionGlow.position = pos;
    }
    #endregion
    #region Boost
    public void SetBoostShopTexts()   //called on section toggles event
    {
        unlockNumber.text = "<size=400>" + AmountOfBoughtItems(LevelReward.Boost)
            + "</size>/" + boostInfos.Length + "\n<color=blue>BOUGHT</color>";
        sectionName.text = "<color=red>BOOSTS</color>\n" + SECTION_NAME_DOTS;
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
            boostPanel.gameObject.name = boost.GetRewardId();
            Image[] images = boostPanel.GetComponentsInChildren<Image>();
            int levelFrameIndex = BonusManager.ChooseBoostSpriteIndex(GameData.gameData.GetBoostLevel(boost.GetRewardId())); //get frame based on boost level

            images[0].sprite = levelFrames[levelFrameIndex]; // set boost frame sprite
            images[1].sprite = boost.UpgradeSprites[levelFrameIndex]; // set boost image based on level

            string title = boost.Title;
            Button button = boostPanel.GetComponent<Button>();
            string descr = "<color=red><size=450>" + title + "</size></color>";
            int index = i;
            if (!boostsUnlocked.Contains(boost.GetRewardId()))
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
            button.onClick.AddListener(delegate () { SetBoostSelection(boostPanel.position); });
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
                if (CoinsDisplay.Instance.GetCoins() >= boostInfos[index].cost)
                {
                    BuyItem(LevelReward.Boost, boostInfos[index]);
                    //GameObject lockGameObject = boostPanel.GetChild(1).gameObject; //lock image
                    //Destroy(lockGameObject);
                    ActivateSoldOutText(boostPanel, 1);
                    itemDescription.text += SequentialText.ColorString("\nSUCCESS! EQUIP BOOST IN A WORLD SELECT MENU!", buyColor);
                    costText.transform.parent.gameObject.SetActive(false);
                    buyButton.gameObject.SetActive(false);

                    Button button = boostPanel.GetComponent<Button>();
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(delegate () { SetBoostSelection(boostPanel.position); });
                    button.onClick.AddListener(delegate () { OnBoostClicked(initDescr, index, true); });
                }
            }
            );
            SetCostText(boostInfos, index);
        }
        else
        {
            SetUIElementsActiveness(false, false, false);
        }
        description += SequentialText.ColorString("\n" + boostInfos[index].Description, greenColor);
        itemDescription.text = description;
    }
    void SetBoostSelection(Vector3 position)
    {
        boostSelectionGlow.position = position;
    }
    #endregion
    #region Trinket
    public void SetTrinketShopTexts()
    {
        unlockNumber.text = "<size=400>" + AmountOfBoughtItems(LevelReward.Trinket)
            + "</size>/" + trinkets.Length + "\n<color=blue>BOUGHT</color>";
        sectionName.text = "<color=blue>TRINKETS</color>\n" + SECTION_NAME_DOTS;
        itemDescription.text = "";
        SetUIElementsActiveness(false, false, false);
    }
    void SetTrinkets()
    {
        var trinketsUnlocked = GameData.gameData.saveData.trinketIds;

        for (int j = 0; j < trinkets.Length; j++)
        {
            var trinket = Instantiate(trinketTemplate, trinketsContainer).transform;
            trinket.gameObject.name = trinkets[j].GetRewardId();
            trinket.GetComponent<Image>().sprite = trinkets[j].trinketSprite;
            Button button = trinket.GetComponent<Button>();
            string trinkName = RewardTemplate.SplitCamelCase(trinkets[j].GetRewardId());
            string descr = SequentialText.ColorString("<size=420>" + trinkName + "</size>", buyColor);

            int index = j;
            if (!trinketsUnlocked.Contains(trinkets[j].GetRewardId()))
            {
                trinket.GetChild(0).gameObject.SetActive(true);  //lock image
                button.onClick.AddListener(delegate () { OnTrinketClicked(descr, index, false, trinket); });
            }
            else
            {
                button.onClick.AddListener(delegate () { OnTrinketClicked(descr, index, true); });
            }
            button.onClick.AddListener(delegate () { SetTrinketSelection(trinket.position); });
        }
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
                if (CoinsDisplay.Instance.GetCoins() >= trinkets[index].cost)
                {
                    BuyItem(LevelReward.Trinket, trinkets[index]);
                    HandleEquipButton(false, () => SetAvatarEquipButton(trinkets[index]));
                    GameObject lockGameObject = trinketObj.GetChild(0).gameObject; //lock image
                    Destroy(lockGameObject);

                    itemDescription.text = initDescr + SequentialText.ColorString("\n" + trinkets[index].description, greenColor); ;
                    costText.transform.parent.gameObject.SetActive(false);

                    Button button = trinketObj.GetComponent<Button>();
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(delegate () { SetTrinketSelection(trinketObj.position); });
                    button.onClick.AddListener(delegate () { OnTrinketClicked(initDescr, index, true); });
                }
            }
            );
            SetCostText(trinkets, index);
        }
        else
        {
            SetUIElementsActiveness(false, false, true);
            SetEquipButtonColor();
            /*Text buttoText = buyButton.GetComponentInChildren<Text>();
            if (trinkets[index].trinketSprite == profileHandler.GetCurrAvatar())  //set button text if avatar is equiped or not
            {
                buttoText.text = "ACTIVE!";
                buyButton.onClick.RemoveAllListeners();
            }
            else
            {
                buttoText.text = "EQUIP!";
                buyButton.onClick.AddListener(delegate () { SetAvatarEquipButton(trinkets[index].trinketSprite); });
            }*/
            bool isEquiped = trinkets[index].trinketSprite == profileHandler.GetCurrAvatar();
            HandleEquipButton(isEquiped, () => SetAvatarEquipButton(trinkets[index]));
        }
        description += SequentialText.ColorString("\n" + trinkets[index].description, greenColor);
        itemDescription.text = description;
    }
    void SetTrinketSelection(Vector3 position)
    {
        trinketSelectionGlow.position = position;
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
            + "</size>/" + titles.Length + "\n<color=blue>BOUGHT</color>";
        sectionName.text = "<color=orange>TITLES</color>\n" + SECTION_NAME_DOTS;
        itemDescription.text = "";
        SetUIElementsActiveness(false, false, false);
    }
    void SetTitles()
    {
        var titlesUnlocked = GameData.gameData.saveData.titleIds;

        for (int i = 0; i < titles.Length; i++)
        {
            var titlePanel = Instantiate(titlePrefab, titlesContainer).transform;
            titlePanel.gameObject.name = titles[i].GetRewardId();
            string title = titles[i].title.ToString();
            titlePanel.GetComponentInChildren<Text>().text = title;
            Button button = titlePanel.GetComponent<Button>();
            string descr = "<color=orange><size=450>" + title + "</size></color>";
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
            button.onClick.AddListener(delegate () { SetTitleSelection(titlePanel.position); });
        }
    }
    void OnTitleClicked(string description, int index, bool isUnlocked, Transform titlePanel = null)
    {
        string title = RewardTemplate.SplitCamelCase(titles[index].title.ToString());
        buyButton.gameObject.SetActive(true); //buy and equip button
        string initDescr = description;
        if (!isUnlocked)
        {
            SetBuyInfo();
            buyButton.onClick.AddListener(delegate ()
            {
                if (CoinsDisplay.Instance.GetCoins() >= titles[index].cost)
                {
                    BuyItem(LevelReward.Title, titles[index]);
                    HandleEquipButton(false, () => SetTitleEquipButton(titles[index]));
                    GameObject lockGameObject = titlePanel.GetChild(1).gameObject; //lock image
                    Destroy(lockGameObject);

                    itemDescription.text = initDescr;
                    costText.transform.parent.gameObject.SetActive(false);

                    Button button = titlePanel.GetComponent<Button>();
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(delegate () { SetTitleSelection(titlePanel.position); });
                    button.onClick.AddListener(delegate () { OnTitleClicked(initDescr, index, true); });
                }
            }
            );
            SetCostText(titles, index);
            description += "\n" + LOCKED + "\n";
        }
        else
        {
            SetUIElementsActiveness(false, false, true);
            SetEquipButtonColor();
            /*Text buttoText = buyButton.GetComponentInChildren<Text>();
            //set button text if title is equiped or not
            if (title == profileHandler.GetCurrentTitle())
            {
                buttoText.text = "ACTIVE!";
                buyButton.onClick.RemoveAllListeners();
            }
            else
            {
                buttoText.text = "EQUIP!";
                buyButton.onClick.AddListener(delegate () { SetTitleEquipButton(title); });
            }*/
            bool isEquiped = title == profileHandler.GetCurrentTitle();
            HandleEquipButton(isEquiped, () => SetTitleEquipButton(titles[index]));
        }

        itemDescription.text = description;
    }
    void SetTitleSelection(Vector3 position)
    {
        titleSelectionGlow.position = position;
    }
    void SetTitleEquipButton(RewardTemplate rewardTemplate)
    {
        string title = rewardTemplate.GetRewardId();
        GameData.gameData.ChangeTitle(title);   //save title
        profileHandler.UpdateTitle(title);   //update title in profile panel
    }
    #endregion
    #region Banner
    public void SetBannerShopTexts()
    {
        unlockNumber.text = "<size=400>" + AmountOfBoughtItems(LevelReward.Banner)
            + "</size>/" + banners.Length + "\n<color=blue>BOUGHT</color>";
        sectionName.text = "<color=red>BANNERS</color>\n" + SECTION_NAME_DOTS;
        itemDescription.text = "";
        SetUIElementsActiveness(false, false, false);
    }
    void SetBanners()
    {
        var bannersUnlocked = GameData.gameData.saveData.bannerIds;

        for (int i = 0; i < banners.Length; i++)
        {
            var bannerPanel = Instantiate(bannerPanelPrefab, bannersContainer).transform;
            bannerPanel.gameObject.name = banners[i].GetRewardId();
            bannerPanel.GetComponent<Image>().sprite = banners[i].Sprite;
            Button button = bannerPanel.GetComponent<Button>();
            string bannerName = RewardTemplate.SplitCamelCase(banners[i].BannerName);
            string descr = "<color=orange><size=450>" + bannerName + "</size></color>";
            int index = i;
            if (!bannersUnlocked.Contains(banners[i].BannerName))
            {
                bannerPanel.GetChild(0).gameObject.SetActive(true);  //lock image
                button.onClick.AddListener(delegate () { OnBannerClicked(descr, index, false, bannerPanel); });
            }
            else
            {
                button.onClick.AddListener(delegate () { OnBannerClicked(descr, index, true); });
            }
            button.onClick.AddListener(delegate () { SetBannerSelection(bannerPanel.position); });
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
                if (CoinsDisplay.Instance.GetCoins() >= banners[index].cost)
                {
                    BuyItem(LevelReward.Banner, banners[index]);
                    HandleEquipButton(false, () => SetBannerEquipButton(banners[index]));
                    GameObject lockGameObject = bannerPanel.GetChild(0).gameObject; //lock image
                    Destroy(lockGameObject);

                    itemDescription.text = initDescr;
                    costText.transform.parent.gameObject.SetActive(false);

                    Button button = bannerPanel.GetComponent<Button>();
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(delegate () { SetBannerSelection(bannerPanel.position); });
                    button.onClick.AddListener(delegate () { OnBannerClicked(initDescr, index, true); });
                }
            }
            );
            SetCostText(banners, index);
            description += "\n" + LOCKED + "\n";
        }
        else
        {
            SetUIElementsActiveness(false, false, true);
            SetEquipButtonColor();
            description += "\n<size=250>" + banners[index].Description + "</size>";
            /*Text buttoText = buyButton.GetComponentInChildren<Text>();

            if(banners[index].Sprite == profileHandler.GetCurrentBanner())
            {
                buttoText.text = "ACTIVE!";
                buyButton.onClick.RemoveAllListeners();
            }
            else
            {
                buttoText.text = "EQUIP!";
                buyButton.onClick.AddListener(delegate () { SetBannerEquipButton(banners[index].Sprite); });
            }*/
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
    void SetBannerSelection(Vector3 position)
    {
        bannerSelectionGlow.position = position;
    }
    #endregion
    void SetCostText(RewardTemplate[] rewardTemplates, int index)
    {
        costText.transform.parent.gameObject.SetActive(true);
        if (rewardTemplates[index] == firstSaleItem || rewardTemplates[index] == secondSaleItem)
        {
            costNoSaleText.gameObject.SetActive(true);
            costNoSaleText.text = "x" + rewardTemplates[index].cost;
            if (rewardTemplates[index] == firstSaleItem)
            {
                int cost1 = (int)(firstSaleItem.cost * (100 - sale1) / 100.0f);
                costText.text = "x" + cost1;
            }
            else
            {
                int cost2 = (int)(secondSaleItem.cost * (100 - sale2) / 100.0f);
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
    void BuyItem(LevelReward levelReward, RewardTemplate rewardItem)
    {
        buyButton.onClick.RemoveAllListeners();
        string id = rewardItem.GetRewardId();
        int cost = rewardItem.cost;
        if (rewardItem == firstSaleItem)
        {
            cost = (int)(rewardItem.cost * (100 - sale1) / 100.0f);
        }
        else if (rewardItem == secondSaleItem)
        {
            cost = (int)(rewardItem.cost * (100 - sale2) / 100.0f);
        }
        switch (levelReward)
        {
            case LevelReward.World:
                {
                    GameData.gameData.UnlockWorld(id);
                    CollectionController.Instance.SetWorlds();
                    unlockNumber.text = "<size=400>" + AmountOfBoughtItems(LevelReward.World)
            + "</size>/" + worlds.Length + "\n<color=blue>BOUGHT</color>";
                    break;
                }
            case LevelReward.Boost:
                {
                    CollectionController.Instance.SetBoosts();
                    GameData.gameData.UnlockBoost(id);
                    unlockNumber.text = "<size=400>" + AmountOfBoughtItems(LevelReward.Boost)
            + "</size>/" + boostInfos.Length + "\n<color=blue>BOUGHT</color>";
                    break;
                }
            case LevelReward.Trinket:
                {
                    CollectionController.Instance.SetTrinkets();
                    GameData.gameData.UnlockTrinket(id);
                    unlockNumber.text = "<size=400>" + AmountOfBoughtItems(LevelReward.Trinket)
            + "</size>/" + trinkets.Length + "\n<color=blue>BOUGHT</color>";
                    buyButton.onClick.AddListener(delegate () { SetAvatarEquipButton(rewardItem); });
                    break;
                }
            case LevelReward.Title:
                {
                    CollectionController.Instance.SetTitles();
                    GameData.gameData.UnlockTitle(id);
                    unlockNumber.text = "<size=400>" + AmountOfBoughtItems(LevelReward.Title)
            + "</size>/" + titles.Length + "\n<color=blue>BOUGHT</color>";
                    buyButton.onClick.AddListener(delegate () { SetTitleEquipButton(rewardItem); });
                    break;
                }
            case LevelReward.Banner:
                {
                    CollectionController.Instance.SetBanners();
                    GameData.gameData.UnlockBanner(id);
                    unlockNumber.text = "<size=400>" + AmountOfBoughtItems(LevelReward.Banner)
            + "</size>/" + banners.Length + "\n<color=blue>BOUGHT</color>";
                    buyButton.onClick.AddListener(delegate () { SetBannerEquipButton(rewardItem); });
                    break;
                }
        }
        CoinsDisplay.Instance.DecreaseCoins(cost);
        costText.transform.parent.gameObject.SetActive(false);

        SetEquipButtonColor();
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
            if (savedIds.Contains(buyableRewards[i].GetRewardId()))
            {
                amountOfBoughtItems++;
            }
        }

        return amountOfBoughtItems;
    }
}