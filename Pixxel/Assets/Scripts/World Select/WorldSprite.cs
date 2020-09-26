﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldSprite : MonoBehaviour
{
    [SerializeField] Material blackAndWhiteMat;
    [SerializeField] WorldInformation worldInformation;

    public int worldNumber;
    public bool isUnlocked = false;
    WorldManager worldManager;
    WorldInfoDisplay infoDisplay;

    void Start()
    {
        if (worldInformation == null || worldInformation != null && !GameData.gameData.saveData.worldUnlocked[worldInformation.WorldIndex])
        {
            if (transform.childCount > 1)
            {
                transform.GetChild(1).gameObject.SetActive(true);
            }
            GetComponent<Image>().material = blackAndWhiteMat;
        }
        else
        {
            if(AllTrinketsEarned())
            {
                transform.GetChild(1).GetComponentInChildren<Text>().text = "<color=lime>COMPLETED</color>";
                transform.GetChild(1).Rotate(0, 0, 45);
                transform.GetChild(1).gameObject.SetActive(true);
            }
            infoDisplay = FindObjectOfType<WorldInfoDisplay>();
            GetComponent<Button>().onClick.AddListener(OpenWorldInfoPanel);
        }
    }

    bool AllTrinketsEarned()
    {
        bool[] allTrinkets = GameData.gameData.saveData.worldTrinkets[worldInformation.WorldIndex].trinkets;
        for (int i = 0; i < allTrinkets.Length; i++)
        {
            if (allTrinkets[i] == false)
                return false;
        }
        return true;
    }

    void OpenWorldInfoPanel()
    {
        infoDisplay.SetInfoPanel(worldInformation);
    }
}
