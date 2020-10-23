using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class WorldTrinkets
{
    public bool[] trinkets;
}

[Serializable]
public class SaveData
{
    public int currentLevel;
    public float levelXP;
    public float maxXPforLevelUp;
    public WorldTrinkets[] worldTrinkets;

    public int[] equipedBoostIndexes;
    public bool[] boostsUnlocked;
    public int[] boostLevels;
    public bool[] avatars;

    //lists of unlocked rewards
    public List<string> worldIds;
    public List<string> trinketIds;
    public List<string> titleIds;
    public List<string> bannerIds;
    public Dictionary<string, int> worldBestScores = new Dictionary<string, int>()
    {
        { "TwilightCity", 0 }
    };

    public int cardInfoIndex;
    public string cardType;
    public string lastTimeCardClaimed;

    public string lastFacebookShare;
    public string lastTwitterShare;

    public bool[] slotsForBoostsUnlocked;
    public string lastTimeQuestClaimed;
    public QuestProgress[] dailyQuests;

    public User playerInfo;
}

public class GameData : MonoBehaviour
{
    public static GameData gameData;
    public SaveData saveData;

    public int currentLevel;
    public bool isAuthentificated = false;

    // Use this for initialization
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
    }

    public void UnlockWorld(string id)
    {
        saveData.worldIds.Add(id);
        saveData.worldBestScores[id] = 0;
        Save();
    }
    public void UnlockTrinket(string id)
    {
        saveData.trinketIds.Add(id);
        Save();
    }
    public void UnlockBoost(int bonusIndex)
    {
        if (bonusIndex < saveData.boostsUnlocked.Length)
        {
            saveData.boostsUnlocked[bonusIndex] = true;
            Save();
        }
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
        for (int i = 0; i < saveData.boostsUnlocked.Length; i++)
        {
            saveData.boostsUnlocked[i] = true;
        }
        saveData.slotsForBoostsUnlocked[1] = true;
        saveData.slotsForBoostsUnlocked[2] = true;
        Save();
    }
    public void UnlockAvatar(int index)
    {
        saveData.avatars[index] = true;
        Save();
    }
    public void UnlockTitle(string id)
    {
        saveData.titleIds.Add(id);
        Save();
    }
    public void ChangeTitle(string title)
    {
        saveData.playerInfo.titleText = title;
        Save();

        DatabaseManager.ChangeTitle(title);
    }
    public void UnlockBanner(string id)
    {
        saveData.bannerIds.Add(id);
        Save();
    }
    public void ChangeBanner(string bannerPath)
    {
        saveData.playerInfo.bannerPath = bannerPath;
        Save();

        DatabaseManager.ChangeBanner(bannerPath);
    }
    public void UpdateLastQuestClaim(DateTime dateTime)
    {
        gameData.saveData.lastTimeQuestClaimed = dateTime.ToString();
        Save();
    }
    public void UpdateCardClaim(DateTime dateTime, int cardIndex, string cardType)
    {
        gameData.saveData.cardInfoIndex = cardIndex;
        gameData.saveData.lastTimeCardClaimed = dateTime.ToString();
        gameData.saveData.cardType = cardType;
        Save();
    }
    public void Save()
    {
        string path = Application.persistentDataPath + "/GameData.data";
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream stream = File.Open(path, FileMode.Create);

        SaveData data;
        data = saveData;

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

    public void EraseGameData() //For testing TODO remove
    {
        string path = Application.persistentDataPath + "/GameData.data";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}