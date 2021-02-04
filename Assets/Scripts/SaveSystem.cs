using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveCoinsAmount(int coins)
    {
        string path = Path.Combine(Application.persistentDataPath, "coins.data");
        FileStream stream = new FileStream(path, FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, coins);
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
}