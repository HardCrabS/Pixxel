using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrinketManager : MonoBehaviour
{
    [SerializeField] Transform selectionFrame;
    [SerializeField] TrinketInfo[] trinketInfos;
    [SerializeField] Text trinketsCollectedText;

    public void SetTrinkets(WorldInformation worldInformation)
    {
        var trinketsUnlocked = GameData.gameData.saveData.trinketIds;
        int trinketsCollected = trinketsUnlocked.Count;
        LevelTemplate[] trinketTemplates = worldInformation.TrinketLevelTemplates;

        SetSprites(worldInformation.TrinketLevelTemplates);

        for (int i = 0; i < trinketTemplates.Length; i++)
        {
            if (trinketsUnlocked.Contains(trinketTemplates[i].id))
            {
                trinketInfos[i].MakeUnlocked();
            }
            trinketInfos[i].SetSelectionFrame(selectionFrame);
        }
        trinketsCollectedText.text = "Collected: " + trinketsCollected + "/" + trinketTemplates.Length;
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