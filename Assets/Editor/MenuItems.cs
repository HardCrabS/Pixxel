using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class MenuItems : MonoBehaviour 
{
    [MenuItem("Tools/Clear GameData")]
    private static void EraseGameData()
    { 
        string path = Application.persistentDataPath + "/GameData.data";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    [MenuItem("Tools/Clear PlayerPrefs")]
    private static void ErasePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
