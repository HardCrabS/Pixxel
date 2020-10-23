using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest")]
public class QuestTemplate : ScriptableObject 
{
    [TextArea(2, 5)] [SerializeField] string questDescription;
    [SerializeField] WorldName worldId;
    [SerializeField] string tag;
    [SerializeField] int numberNeeded;
    [SerializeField] reward rewardType;
    [SerializeField] int reward;

    public string QuestDescription { get { return questDescription; } }
    public string WorldId { get { return worldId.ToString(); } }
    public string Tag { get { return tag; } }
    public int NumberNeeded { get { return numberNeeded; } }
    public reward RewardType { get { return rewardType; } }
    public int Reward { get { return reward; } }
}
