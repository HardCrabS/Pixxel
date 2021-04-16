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
    public Image[] equipFrames = new Image[3];
    public BoostBase currentBonus;
    public Sprite bonusSprite;

    private void Start()
    {
        if (bonusManager != null)
        {
            //Sprite[] boostSprites = bonusManager.GetEquipedBoosts();
            Tuple<Sprite, Sprite>[] boostSprites = bonusManager.GetEquipedBoostSprites();
            Boost[] boostInfos = bonusManager.GetEquipedBoostInfos();
            for (int i = 0; i < equipeButtons.Length; i++)
            {
                bool slotUnlocked = GameData.gameData.saveData.slotsForBoostsUnlocked[i];
                if (!slotUnlocked)
                {
                    equipeButtons[i].interactable = false;
                    equipeButtons[i].GetComponent<Image>().color = new Color(1, 1, 1, 0);
                    if (equipeButtons[i].transform.childCount > 0)
                    {
                        equipeButtons[i].transform.GetChild(0).gameObject.SetActive(true);  //lock image
                    }
                    equipFrames[i].sprite = bonusManager.lockedFrame;
                    equipeButtons[i] = null;
                }
                else
                {
                    if (boostSprites[i] != null)
                    {
                        equipeButtons[i].GetComponent<Image>().sprite = boostSprites[i].Item1;
                        equipeButtons[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        equipFrames[i].sprite = boostSprites[i].Item2;
                        equipeButtons[i].GetComponent<BonusButton>().boostInfo = boostInfos[i];
                    }
                    else
                        equipeButtons[i].GetComponent<Image>().color = new Color(1, 1, 1, 0);
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
                {
                    equipeButtons[i].GetComponent<Image>().sprite = boostSprite;
                    bonusManager.SetBoostFrame(boostInfo.id, i);
                }
            }
        }
    }

    public void UpdateWorldDisplayBoosts()  //boost panels in world select screen
    {
        for (int i = 0; i < worldDisplayEqupped.equipeButtons.Length; i++)
        {
            if (worldDisplayEqupped.equipeButtons[i] != null)
            {
                var worldDisplBoostImage = worldDisplayEqupped.equipeButtons[i].GetComponent<Image>();
                var boostScreenBoostImage = equipeButtons[i].GetComponent<Image>();
                worldDisplBoostImage.sprite = boostScreenBoostImage.sprite;
                worldDisplBoostImage.color = boostScreenBoostImage.color;
            }
            worldDisplayEqupped.equipFrames[i].sprite = bonusManager.boostPanels[i].sprite; //change boost panels which are childed
        }
    }
}