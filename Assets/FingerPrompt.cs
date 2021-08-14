using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FingerPrompt : MonoBehaviour
{
    [SerializeField] RectTransform finger;
    [SerializeField] RectTransform boostsButton;
    [SerializeField] RectTransform GOButton;

    public void SetPromptFinger()
    {
        if(IsBoostNotEquipped())
        {
            //point finger on boosts button
            var offset = new Vector2(-boostsButton.rect.width/6.0f, boostsButton.rect.height / 2.0f);
            PointFingerToButton(boostsButton, offset);
        }
        else
        {
            //point finger on GO button
            var offset = new Vector2(-GOButton.rect.width / 2.0f, GOButton.rect.height / 2.0f);
            PointFingerToButton(GOButton, offset);
        }
    }

    void PointFingerToButton(RectTransform transformToPointTo, Vector2 offset)
    {
        finger.SetParent(transformToPointTo);
        finger.localPosition = Vector3.zero;
        finger.anchoredPosition += offset;
    }
    
    bool IsBoostNotEquipped()
    {
        var slots = GameData.gameData.saveData.slotsForBoostsUnlocked;
        var equippedBoosts = GameData.gameData.saveData.equipedBoosts;
        for (int i = 0; i < slots.Length; i++)
        {
            //if slot is unlocked and boost isn't equipped in it
            if (slots[i] && string.IsNullOrEmpty(equippedBoosts[i]))
            {
                //check if there is available boost to equip
                if (BoostToEquipExists())
                    return true;
            }
        }
        return false;
    }

    bool BoostToEquipExists()
    {
        var equippedBoosts = GameData.gameData.saveData.equipedBoosts;
        var allUnlockedBoosts = GameData.gameData.saveData.boostIds;

        for (int i = 0; i < allUnlockedBoosts.Count; i++)
        {
            //if unlocked boost isn't equpped
            if (!equippedBoosts.Contains(allUnlockedBoosts[i]))
                return true;
        }
        return false;
    }
}