using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldManager : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] WorldSprite[] worlds = new WorldSprite[10];
    SerializedLevel[] allWorlds;

    void Awake()
    {
        allWorlds = SaveSystem.LoadAllWorldsInfo();

        for (int i = 0; i < allWorlds.Length; i++)
        {
            worlds[i].worldNumber = i;
            if (allWorlds[i] != null && allWorlds[i]._isUnlocked)
            {
                if (worlds[i] != null)
                {
                    worlds[i].isUnlocked = true;
                }
            }
        }
    }

    public void ActivateBlockingPanel()
    {
        panel.active = true;
    }
    public void DeactivatePanel()
    {
        panel.active = false;
    }
}
