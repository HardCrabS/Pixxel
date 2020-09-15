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
    public IConcreteBonus currentBonus;
    public Sprite bonusSprite;

    private void Start()
    {
        Sprite[] boostSprites = bonusManager.GetEquipedBoosts();
        for (int i = 0; i < equipeButtons.Length; i++)
        {
            if (!GameData.gameData.saveData.slotsForBoostsUnlocked[i])
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

    public static Sprite AssignSpriteToBonus(ButtonData loadedBonus)
    {
        if (loadedBonus == null) { return null; }
        Sprite[] allSprites = Resources.LoadAll<Sprite>("Sprites/BoostSprites");
        Sprite sprite = null;

        for (int k = 0; k < allSprites.Length; k++)
        {
            if (allSprites[k].name == loadedBonus.Name)
            {
                sprite = allSprites[k];
                break;
            }
        }
        return sprite;
    }

    public void UpdateEquipedBoosts(Boost boostInfo)
    {
        int[] equipedBoostsIndex = GameData.gameData.saveData.equipedBoostIndexes;
        for (int i = 0; i < equipeButtons.Length; i++)
        {
            if (boostInfo.Index == equipedBoostsIndex[i])
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

[Serializable]
public class ButtonData
{
    private Type type;
    private string name;

    public ButtonData(Type type, string name)
    {
        this.type = type;
        this.name = name;
    }
    public Type Type
    {
        get { return type; }
    }
    public string Name
    {
        get { return name; }
    }
}