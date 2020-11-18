﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldSprite : MonoBehaviour
{
    [SerializeField] Material blackAndWhiteMat;
    [SerializeField] WorldInformation worldInformation;
    [SerializeField] Transform selectionGlow;

    public int worldNumber;
    WorldInfoDisplay infoDisplay;

    void Start()
    {
        //if scriptObject not set or world isn't unlocked
        if (worldInformation == null || 
            !GameData.gameData.saveData.worldIds.Contains(worldInformation.WorldName))
        {
            if (transform.childCount > 0)
            {
                transform.GetChild(0).gameObject.SetActive(true); //lock image
            }
            GetComponent<Image>().material = blackAndWhiteMat;
        }
        else
        {
            infoDisplay = FindObjectOfType<WorldInfoDisplay>();
            GetComponent<Button>().onClick.AddListener(OpenWorldInfoPanel);
        }

        if(selectionGlow != null)
        {
            GetComponent<Button>().onClick.AddListener(SetSelectionGlow);
        }
    }

    public void ShowDescription()
    {
        CollectionController.Instance.SetWorldDescription(worldInformation.GetRewardId(),
            worldInformation.Description);
    }

    public void SetSelectionGlow()
    {
        selectionGlow.position = transform.position;
    }
    void OpenWorldInfoPanel()
    {
        if (infoDisplay != null)
            infoDisplay.SetInfoPanel(worldInformation);
    }
}
