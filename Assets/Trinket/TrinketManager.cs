using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrinketManager : MonoBehaviour
{
    [SerializeField] Transform selectionFrame;
    [SerializeField] TrinketInfo[] trinketInfos;
    [SerializeField] Text trinketsCollectedText;

    public void SetTrinkets(WorldLoadInfo worldLoadInformation)
    {
        var trinketsUnlocked = GameData.gameData.saveData.trinketIds;
        LevelTemplate[] trinketTemplates = worldLoadInformation.trinketTemplates;
        int unlockedInWorld = 0;

        SetSprites(worldLoadInformation.trinketTemplates);

        for (int i = 0; i < trinketTemplates.Length; i++)
        {
            if (trinketsUnlocked.Contains(trinketTemplates[i].id))
            {
                trinketInfos[i].MakeUnlocked();
                unlockedInWorld++;
            }
            trinketInfos[i].SetSelectionFrame(selectionFrame);
        }
        trinketsCollectedText.text = "Collected: " + unlockedInWorld + "/" + trinketTemplates.Length;
    }

    public void LockAllTrinkets()
    {
        selectionFrame.position = new Vector2(-500, 0);
        for (int i = 0; i < trinketInfos.Length; i++)
        {
            trinketInfos[i].LockTrinket();
        }
    }

    void SetSprites(LevelTemplate[] templates)
    {
        for (int i = 0; i < templates.Length; i++)
        {
            if (templates[i] != null)
            {
                trinketInfos[i].GetComponent<Image>().sprite = templates[i].trinketSprite;
                trinketInfos[i].levelTemplate = templates[i];
            }
        }
    }
}