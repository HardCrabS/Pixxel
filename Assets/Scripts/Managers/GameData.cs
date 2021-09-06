using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int currentLevel;
    public float levelXP;
    public float maxXPforLevelUp;

    public List<string> equipedBoosts;

    public int coins;

    //lists of unlocked rewards
    public List<string> worldIds;
    public List<string> boostIds;
    public List<string> trinketIds;
    public List<string> titleIds;
    public List<string> bannerIds;
    public List<string> cardIds;
    public Dictionary<string, int> worldBestScores = new Dictionary<string, int>()
    {
        { "Twilight City", 0 }   //unlocked by default
    };
    public Dictionary<string, int> boostLevels = new Dictionary<string, int>()
    {
        { "Flash Field", 1 }   //unlocked by default
    };
    public Dictionary<string, int> trinketsProgress = new Dictionary<string, int>();

    public (LevelReward, string, int)[] saleItemsInfo = new (LevelReward, string, int)[2]; //rewardType, Id, sale
    public string lastTimeSaleClaimed;

    public string cardType;
    public string nextPossibleCardClaime;

    public string nextPossibleFacebookShare;
    public string nextPossibleTwitterShare;

    public bool[] slotsForBoostsUnlocked;
    public string nextPossibleQuestClaime;
    public QuestProgress[] dailyQuests;

    public bool adsRemoved;
    public User playerInfo;
}

public class GameData : MonoBehaviour
{
    public static GameData gameData;
    public SaveData saveData;

    public int currentLevel;
    public bool isAuthentificated = false;

    public readonly Dictionary<LevelReward, List<string>> allItemsId = new Dictionary<LevelReward, List<string>>();

    void Awake()
    {
        if (gameData == null)
        {
            DontDestroyOnLoad(this.gameObject);
            gameData = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        Load();

        allItemsId.Add(LevelReward.World, saveData.worldIds);
        allItemsId.Add(LevelReward.Boost, saveData.boostIds);
        allItemsId.Add(LevelReward.Trinket, saveData.trinketIds);
        allItemsId.Add(LevelReward.Title, saveData.titleIds);
        allItemsId.Add(LevelReward.Banner, saveData.bannerIds);
    }

    public void UnlockWorld(string id)
    {
        if (saveData.worldIds.Contains(id)) return;
        saveData.worldIds.Add(id);
        saveData.worldBestScores[id] = 0;
        Save();
    }
    public void UnlockBoost(string id)
    {
        if (saveData.boostIds.Contains(id)) return;
        saveData.boostIds.Add(id);
        saveData.boostLevels.Add(id, 1);
        Save();
    }
    public int GetBoostLevel(string boostId)
    {
        if (string.IsNullOrEmpty(boostId)) return 1;

        return saveData.boostLevels.ContainsKey(boostId) ? saveData.boostLevels[boostId] : 1;
    }
    public void UnlockTrinket(string id)
    {
        if (saveData.trinketIds.Contains(id)) return;
        saveData.trinketIds.Add(id);
        Save();
    }
    public void UnlockSlotForBoost(int index)
    {
        if (index < saveData.slotsForBoostsUnlocked.Length)
        {
            saveData.slotsForBoostsUnlocked[index] = true;
            Save();
        }
    }
    public void UnlockAllBoosts()
    {
        var allBoosts = CollectionController.Instance.boostInfos;
        for (int i = 0; i < allBoosts.Length; i++)
        {
            saveData.boostIds.Add(allBoosts[i].id);
            if (!saveData.boostLevels.ContainsKey(allBoosts[i].id))
                saveData.boostLevels.Add(allBoosts[i].id, 1);
        }
        saveData.slotsForBoostsUnlocked[1] = true;
        saveData.slotsForBoostsUnlocked[2] = true;
        Save();
    }
    public void UnlockAllWorlds()
    {
        var allWorlds = CollectionController.Instance.worlds;
        for (int i = 0; i < allWorlds.Length; i++)
        {
            saveData.worldIds.Add(allWorlds[i].id);
            if (!saveData.worldBestScores.ContainsKey(allWorlds[i].id))
                saveData.worldBestScores.Add(allWorlds[i].id, 0);
        }
        Save();
    }
    public void UnlockAllTrinkets()
    {
        var trinkets1 = CollectionController.Instance.trinkets;
        var trinkets2 = CollectionController.Instance.trinketsRank;
        var trinkets3 = CollectionController.Instance.trinketsShop;
        for (int i = 0; i < trinkets1.Length; i++)
        {
            if(!saveData.trinketIds.Contains(trinkets1[i].id))
            {
                saveData.trinketIds.Add(trinkets1[i].id);
            }
        }
        for (int i = 0; i < trinkets2.Length; i++)
        {
            if (!saveData.trinketIds.Contains(trinkets2[i].id))
            {
                saveData.trinketIds.Add(trinkets2[i].id);
            }
        }
        for (int i = 0; i < trinkets3.Length; i++)
        {
            if (!saveData.trinketIds.Contains(trinkets3[i].id))
            {
                saveData.trinketIds.Add(trinkets3[i].id);
            }
        }
        Save();
    }
    public void UnlockTitle(string id)
    {
        if (saveData.titleIds.Contains(id)) return;
        saveData.titleIds.Add(id);
        Save();
    }
    public void ChangeTitle(string title)
    {
        saveData.playerInfo.titleText = title;
        Save();

        DatabaseManager.ChangeTitle(title);
    }
    public void ChangeAvatar(string avatarPath)
    {
        saveData.playerInfo.spritePath = avatarPath;
        Save();

        DatabaseManager.ChangeAvatar(avatarPath);
    }
    public void UnlockBanner(string id)
    {
        if (saveData.bannerIds.Contains(id)) return;
        saveData.bannerIds.Add(id);
        Save();
    }
    public void ChangeBanner(string bannerPath, MatAnimatorValues animatorValues)
    {
        string json = JsonUtility.ToJson(animatorValues);
        bannerPath += "|" + json;

        saveData.playerInfo.bannerPath = bannerPath;
        Save();

        DatabaseManager.ChangeBanner(bannerPath);
    }
    public void UnlockCardSet(string id)
    {
        if (saveData.cardIds.Contains(id)) return;
        saveData.cardIds.Add(id);
        saveData.nextPossibleCardClaime = "";
        saveData.cardType = "";
        Save();
    }
    public void UpdateLastQuestClaim(DateTime dateTime)
    {
        gameData.saveData.nextPossibleQuestClaime = dateTime.ToString();
        Save();
    }
    public void UpdateCardClaim(DateTime dateTime, string cardType)
    {
        gameData.saveData.nextPossibleCardClaime = dateTime.ToString();
        gameData.saveData.cardType = cardType;
        Save();
    }
    public void RemoveAds()
    {
        saveData.adsRemoved = true;
        BannerAd.Instance.HideBannerAd();
        Save();
    }
    public static void Save()
    {
        string path = Application.persistentDataPath + "/GameData.data";

        SaveData data;
        data = gameData.saveData;

        string base64Str = ObjectToString(data);
        base64Str = Encryptor.EncryptDecrypt(base64Str, 50);
        File.WriteAllText(path, base64Str);
    }

    public void Load()
    {
        string path = Application.persistentDataPath + "/GameData.data";

        if (File.Exists(path))
        {
            string encodedBase64 = File.ReadAllText(path);
            encodedBase64 = Encryptor.EncryptDecrypt(encodedBase64, 50);
            saveData = StringToObject(encodedBase64) as SaveData;
        }
    }

    public static bool IsItemCollected(RewardTemplate reward)
    {
        List<string> ids = gameData.allItemsId[reward.reward];
        return ids.Contains(reward.id);
    }

    public void EraseGameData()
    {
        string path = Application.persistentDataPath + "/GameData.data";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        ProfileHandler.Instance.ResetBanner();
        ProfileHandler.Instance.ResetAvatar();

        PlayerPrefs.SetInt("WORLD TUTORIAL", 0);
        PlayerPrefs.SetInt("WORLD SELECT TUTORIAL", 0);
        gameData = null;

        AudioController.Instance.SetCurrentClip(null);
        FindObjectOfType<SceneLoader>().LoadSceneAsync(1);
    }

    public static string ObjectToString(object obj)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            new BinaryFormatter().Serialize(ms, obj);
            return Convert.ToBase64String(ms.ToArray());
        }
    }

    public object StringToObject(string base64String)
    {
        byte[] bytes = Convert.FromBase64String(base64String);
        using (MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length))
        {
            ms.Write(bytes, 0, bytes.Length);
            ms.Position = 0;
            return new BinaryFormatter().Deserialize(ms);
        }
    }
}