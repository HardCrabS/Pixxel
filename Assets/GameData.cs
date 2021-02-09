using System;
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
        { "Magic Potion", 1 }   //unlocked by default
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
    public int GetBoostLevel(string boostName)
    {
        return saveData.boostLevels.ContainsKey(boostName) ? saveData.boostLevels[boostName] : 1;
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
    public void UnlockAllBoosts()   //TODO JUST FOR TEST. REMOVE LATER
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
    public void ChangeBanner(string bannerPath)
    {
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
    public static void Save()
    {
        string path = Application.persistentDataPath + "/GameData.data";
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream stream = File.Open(path, FileMode.Create);

        SaveData data;
        data = gameData.saveData;

        binaryFormatter.Serialize(stream, data);
        stream.Close();
    }

    public void Load()
    {
        string path = Application.persistentDataPath + "/GameData.data";
        if (File.Exists(path))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream stream = File.Open(path, FileMode.Open);

            saveData = binaryFormatter.Deserialize(stream) as SaveData;
            stream.Close();
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
        PlayerPrefs.DeleteAll();
        gameData = null;

        AudioController.Instance.SetCurrentClip(null);
        FindObjectOfType<SceneLoader>().LoadSceneAsync(1);
    }
}