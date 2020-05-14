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
        int trinketsCollected = 0;
        int worldIndex = worldInformation.WorldIndex;
        LevelTemplate[] trinketTemplates = worldInformation.TrinketLevelTemplates;

        SetSprites(worldInformation.TrinketLevelTemplates);

        bool[] trinketLockStatus = GameData.gameData.saveData.worldTrinkets[worldIndex].trinkets;

        int trinketArrayLenght = trinketLockStatus.Length;
        for (int i = 0; i < trinketArrayLenght; i++)
        {
            if (trinketLockStatus[i])
            {
                trinketsCollected++;
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