using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionController : MonoBehaviour 
{
    public static CollectionController Instance;

    [Header("Worlds Collection")]
    [SerializeField] Text worldDescription;
    [SerializeField] Text unlockNumber;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

	void Start () 
    {
        SetWorldsUnlocked();
	}

    public void SetWorldDescription(string worldName, string description, int index)
    {
        var worlds = GameData.gameData.saveData.worldUnlocked;
        if (worlds[index] == true)
        {
            worldDescription.text = "<color=purple><size=350>" + worldName + "</size></color>\n" + description;
        }
        else
        {
            int rankToUnlock = RewardForLevel.Instance.GetRankFromRewards(LevelReward.World, index);
            worldDescription.text = "<color=green>" + worldName + "</color>" 
                + "\n" + "<color=red>- LOCKED -</color>\n\n<color=black><size=250>Unlocked by reaching Player Rank " 
                + rankToUnlock + ".</size></color>";
        }
    }

    void SetWorldsUnlocked()
    {
        var worlds = GameData.gameData.saveData.worldUnlocked;

        int unlockedCount = 0;
        for (int i = 0; i < worlds.Length; i++)
        {
            if(worlds[i] == true)
            {
                unlockedCount++;
            }
        }
        unlockNumber.text = "<size=400>" + unlockedCount + "</size>/" + worlds.Length + "\n<color=blue>unlocked</color>";
    }
}
