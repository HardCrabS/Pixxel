using System;
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
    public bool[] worldUnlocked;
    public int[] worldsBestScores;
    public WorldTrinkets[] worldTrinkets;

    public int[] equipedBoostIndexes;
    public bool[] boostsUnlocked;
    public int[] boostLevels;
    public bool[] avatars;
    public bool[] titlesUnlocked;
    public bool[] bannersUnlocked;

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

    public void UnlockWorld(int index)
    {
        if (currentLevel < saveData.worldUnlocked.Length)
        {
            saveData.worldUnlocked[index] = true;
            Save();
        }
    }
    public void UnlockTrinket(int worldIndex, int trinketIndex)
    {
        if (worldIndex < saveData.worldTrinkets.Length && trinketIndex < saveData.worldTrinkets[worldIndex].trinkets.Length)
        {
            saveData.worldTrinkets[worldIndex].trinkets[trinketIndex] = true;
            Save();
        }
    }
    public void UnlockBoost(int bonusIndex)
    {
        if(bonusIndex < saveData.boostsUnlocked.Length)
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
    public void UnlockTitle(int index)
    {
        saveData.titlesUnlocked[index] = true;
        Save();
    }
    public void ChangeTitle(string title)
    {
        saveData.playerInfo.titleText = title;
        Save();

        DatabaseManager.ChangeTitle(title);
    }
    public void UnlockBanner(int index)
    {
        saveData.bannersUnlocked[index] = true;
        Save();
    }
    public void ChangeBanner(string bannerPath)
    {
        saveData.playerInfo.bannerSpritePath = bannerPath;
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