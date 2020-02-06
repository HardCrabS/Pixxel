using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveCoinsAmount(CoinsDisplay coinsDisplay)
    {
        string path = Path.Combine(Application.persistentDataPath, "coins.data");
        FileStream stream = new FileStream(path, FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, coinsDisplay.GetCoins());
        stream.Close();
    }
    public static int LoadCoinsAmount()
    {
        string path = Path.Combine(Application.persistentDataPath, "coins.data");
        if(File.Exists(path))
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            int coins = (int)formatter.Deserialize(stream);
            stream.Close();
            return coins;
        }
        else
        {
            return 0;
        }
    }
    public static void SaveGlobalData(LevelSlider levelSlider)
    {
        string path = Path.Combine(Application.persistentDataPath, "global.data");

        LevelData levelData = new LevelData(levelSlider);
        string json = JsonUtility.ToJson(levelData);
        File.WriteAllText(path, json);
    }

    public static LevelData LoadGlobalData()
    {
        string path = Path.Combine(Application.persistentDataPath, "global.data");

        if (File.Exists(path))
        {
            LevelData data = JsonUtility.FromJson<LevelData>(File.ReadAllText(path));
            return data;
        }
        else
        {
            Debug.LogError("File does not exist in " + path);
            return null;
        }
    }
    public static void SaveChosenBoosts(ButtonData[] bonuses)
    {
        string path = Path.Combine(Application.persistentDataPath, "buttons.data");

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, bonuses);
        stream.Close();
    }

    public static ButtonData[] LoadEquipedBonuses()
    {
        string path = Path.Combine(Application.persistentDataPath, "buttons.data");

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            ButtonData[] types = formatter.Deserialize(stream) as ButtonData[];
            stream.Close();
            return types;
        }
        else
        {
            Debug.LogError("File does not exist in " + path);
            return new ButtonData[3];
        }
    }

    public static void SaveLocalLevelData(SerializedLevel level)
    {
        string path = Path.Combine(Application.persistentDataPath, "localLevel.data");
        LevelData levelData;
        if (File.Exists(path))
        {
            levelData = JsonUtility.FromJson<LevelData>(File.ReadAllText(path));
            levelData.SaveLevelData(level);
        }
        else
        {
            levelData = new LevelData();
            levelData.SaveLevelData(level);
        }
        string json = JsonUtility.ToJson(levelData);
        File.WriteAllText(path, json);
    }

    public static SerializedLevel[] LoadAllWorldsInfo()
    {
        string path = Path.Combine(Application.persistentDataPath, "localLevel.data");

        if (File.Exists(path))
        {
            LevelData data = JsonUtility.FromJson<LevelData>(File.ReadAllText(path));
            return data.levels;
        }
        else
        {
            Debug.LogError("File does not exist in " + path);
            LevelData levelData = new LevelData();
            return levelData.levels;
        }
    }

    public static SerializedLevel LoadLocalLevelData(int levelIndex)
    {
        string path = Path.Combine(Application.persistentDataPath, "localLevel.data");

        if (File.Exists(path))
        {
            LevelData data = JsonUtility.FromJson<LevelData>(File.ReadAllText(path));
            return data.levels[levelIndex];
        }
        else
        {
            Debug.LogError("File does not exist in " + path);
            return null;
        }
    }

    public static void SaveBoost(SerializableBoost newBoost)
    {
        string path = Path.Combine(Application.persistentDataPath, "allBoosts.data");
        Bonus allBonus;

        if (File.Exists(path))
        {
            allBonus = JsonUtility.FromJson<Bonus>(File.ReadAllText(path));
            allBonus.SaveBonus(newBoost);
        }
        else
        {
            allBonus = new Bonus();
            allBonus.SetAllBonusDefault();
            allBonus.SaveBonus(newBoost);
        }
        string json = JsonUtility.ToJson(allBonus);
        File.WriteAllText(path, json);
    }
    public static Bonus LoadAllBonuses()
    {
        string path = Path.Combine(Application.persistentDataPath, "allBoosts.data");
        Bonus allBonuses;
        if (File.Exists(path))
        {
            allBonuses = JsonUtility.FromJson<Bonus>(File.ReadAllText(path));
        }
        else
        {
            allBonuses = new Bonus();
            allBonuses.SetAllBonusDefault();
        }
        return allBonuses;
    }
}