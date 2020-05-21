using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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
    public bool[] worldUnlocked;
    public int[] worldsBestScores;
    public WorldTrinkets[] worldTrinkets;
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
    }
    void Start()
    {
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
    public void Save()
    {
        string path = Application.persistentDataPath + "/levels.data";
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream stream = File.Open(path, FileMode.Create);

        SaveData data = new SaveData();
        data = saveData;

        binaryFormatter.Serialize(stream, data);
        stream.Close();
    }

    public void Load()
    {
        string path = Application.persistentDataPath + "/levels.data";
        if (File.Exists(path))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream stream = File.Open(path, FileMode.Open);

            saveData = binaryFormatter.Deserialize(stream) as SaveData;
            stream.Close();
        }
    }
}