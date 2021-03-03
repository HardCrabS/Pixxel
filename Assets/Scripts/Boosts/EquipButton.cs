using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EquipButton : MonoBehaviour
{
    [SerializeField] BonusManager bonusManager;
    [SerializeField] EquipButton worldDisplayEqupped;
    public Button[] equipeButtons = new Button[3];
    public BoostBase currentBonus;
    public Sprite bonusSprite;

    private void Start()
    {
        if (bonusManager != null)
        {
            Sprite[] boostSprites = bonusManager.GetEquipedBoosts();
            for (int i = 0; i < equipeButtons.Length; i++)
            {
                bool slotUnlocked = GameData.gameData.saveData.slotsForBoostsUnlocked[i];
                if (!slotUnlocked)
                {
                    equipeButtons[i].interactable = false;
                    if (equipeButtons[i].transform.childCount > 0)
                        equipeButtons[i].transform.GetChild(0).gameObject.SetActive(true);
                    equipeButtons[i] = null;
                }
                else
                {
                    if (boostSprites[i] != null)
                    {
                        equipeButtons[i].GetComponent<Image>().sprite = boostSprites[i];
                    }
                }
            }
        }
    }

    public void UpdateEquipedBoosts(Boost boostInfo)
    {
        List<string> equipedBoostIds = GameData.gameData.saveData.equipedBoosts;
        for (int i = 0; i < equipeButtons.Length; i++)
        {
            if (boostInfo.id == equipedBoostIds[i])
            {
                Sprite boostSprite = BonusManager.GetBoostImage(boostInfo);
                if (equipeButtons[i] != null)
                    equipeButtons[i].GetComponent<Image>().sprite = boostSprite;
            }
        }
    }

    public void UpdateWorldDisplayBoosts()
    {
        for (int i = 0; i < worldDisplayEqupped.equipeButtons.Length; i++)
        {
            if (worldDisplayEqupped.equipeButtons[i] != null)
                worldDisplayEqupped.equipeButtons[i].GetComponent<Image>().sprite = equipeButtons[i].GetComponent<Image>().sprite;
        }
    }
}