using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EquipButton : MonoBehaviour
{
    public Button[] equipeButtons = new Button[3];
    public IConcreteBonus currentBonus;
    public Sprite bonusSprite;

    private void Start()
    {
        ButtonData[] loadedBonuses = SaveSystem.LoadEquipedBonuses();
        for (int i = 0; i < equipeButtons.Length; i++)
        {
            if (loadedBonuses[i] != null)
            {
                equipeButtons[i].GetComponent<Image>().sprite = AssignSpriteToBonus(loadedBonuses[i]);
            }
        }
    }

    public static Sprite AssignSpriteToBonus(ButtonData loadedBonus)
    {
        if(loadedBonus == null) { return null; }
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

    public void Equip()
    {
        foreach (Button button in equipeButtons)
        {
            button.interactable = !button.IsInteractable();
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