using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum reward
{
    coins,
    XP
}

[System.Serializable]
public class QuestProgress
{
    public string questDescription;
    public string tag;
    public int numberNeeded;
    public int numberCollected;
    public string worldId;

    public reward rewardType;
    public int rewardAmount;

    public bool rewardClaimed = false;
    public int savedArrayIndex;
}
public class DailyQuestManager : MonoBehaviour
{
    [SerializeField] GameObject exclamationBubble;
    [SerializeField] GameObject questPanel;
    [SerializeField] Sprite clickable;
    [SerializeField] Sprite nonClickable;

    [SerializeField] TextMeshProUGUI[] questsTexts;
    [SerializeField] GameObject[] claimButtons;
    [SerializeField] GameObject[] claimedImages;

    [Header("Quest Info")]
    [SerializeField] TextAsset questTemplates;
    [SerializeField] TextAsset names;
    [SerializeField] TextAsset occupations;
    [SerializeField] TextAsset things;
    [Range(10, 200)] [SerializeField] int rewardAmountMax;
    [Range(10, 200)] [SerializeField] int rewardAmountMin;

    QuestProgress[] quests;

    Dictionary<string, TextAsset> questVariables = new Dictionary<string, TextAsset>();

    const string QUESTS = "[Quest]";
    const string WORLD = "[World]";
    const string NAME = "[Name]";
    const string OCCUPATION = "[Occupation]";
    const string THING = "[Thing]";
    const string NUM_OF_BLOCKS = "[Number of Blocks]";
    const string BLOCKS = "[Blocks]";
    const string BLOCK_TYPE = "[Block Type]";

    List<string> questKeys = new List<string>()
    {
        WORLD,
        NAME,
        OCCUPATION,
        THING,
        NUM_OF_BLOCKS,
        BLOCKS,
        BLOCK_TYPE
    };

    void Start()
    {
        if(IsTimeToClaim() || IsUnfinishedQuest())
        {
            exclamationBubble.SetActive(true);
        }

        questVariables.Add(QUESTS, questTemplates);
        questVariables.Add("[Occupation]", occupations);
        questVariables.Add("[Name]", names);
        questVariables.Add("[Thing]", things);
    }

    QuestProgress GetQuest()
    {
        QuestProgress questProgress = new QuestProgress();
        string template = GetRandomLine(QUESTS, ref questProgress);
        foreach (var key in questKeys)
        {
            while (template.Contains(key))
            {
                string questVariable = GetRandomLine(key, ref questProgress);
                questVariable = questVariable.TrimEnd('\r', '\n');
                var regex = new Regex(Regex.Escape(key));
                template = regex.Replace(template, questVariable, 1); //replaces only first occurrence
            }
        }
        questProgress.questDescription = template;
        if (string.IsNullOrEmpty(questProgress.worldId))
        {
            WorldRelatedInfo(WORLD, ref questProgress);
        }
        if (string.IsNullOrEmpty(questProgress.tag))
        {
            WorldRelatedInfo(BLOCK_TYPE, ref questProgress);
        }
        if (questProgress.numberNeeded == 0)
        {
            GetRandomLine(NUM_OF_BLOCKS, ref questProgress);
        }
        int rewardAmount = (int)Random.Range(rewardAmountMin * 0.2f, (rewardAmountMax + 1) * 0.2f) * 5;
        reward rewardType = Random.Range(0, 2) == 0 ? reward.coins : reward.XP;
        questProgress.rewardAmount = rewardAmount;
        questProgress.rewardType = rewardType;

        return questProgress;
    }

    string GetRandomLine(string key, ref QuestProgress questProgress)
    {
        if (questVariables.ContainsKey(key))
        {
            string[] strSeparated = questVariables[key].text.Split('\n');
            return strSeparated[Random.Range(0, strSeparated.Length)];
        }
        else if (key.CompareTo(NUM_OF_BLOCKS) == 0)
        {
            int blocksNeeded = (int)Random.Range(15 * 0.2f, (80 + 1) * 0.2f) * 5;
            questProgress.numberNeeded = blocksNeeded;  //needed blocks for quest
            return blocksNeeded.ToString();
        }
        else
        {
            return WorldRelatedInfo(key, ref questProgress);
        }
    }

    string WorldRelatedInfo(string key, ref QuestProgress questProgress)
    {
        string worldId;
        if (string.IsNullOrEmpty(questProgress.worldId))
        {
            var unlockedWorlds = GameData.gameData.saveData.worldIds;
            worldId = unlockedWorlds[Random.Range(0, unlockedWorlds.Count)];
            questProgress.worldId = worldId;    //world id
        }
        else
        {
            worldId = questProgress.worldId;
        }

        if (key.CompareTo(WORLD) == 0)
        {
            return worldId;
        }
        else
        {
            var allWorlds = CollectionController.Instance.worlds;
            var worldInfo = CollectionController.FindItemWithId(allWorlds, worldId) as WorldInformation;
            if (key.CompareTo(BLOCKS) == 0)
            {
                return worldInfo.BlocksName;
            }
            else //block type
            {
                GameObject block = worldInfo.Boxes[Random.Range(0, worldInfo.Boxes.Length)];
                string tag = block.tag;
                string boxName = block.GetComponent<Box>().boxName;
                questProgress.tag = tag;    //block tag to collect
                return boxName;
            }
        }
    }

    public void ShowQuestPanel()
    {
        questPanel.SetActive(true);
        PickRandomQuests();
    }

    private void PickRandomQuests()
    {
        if (IsTimeToClaim())
        {
            for (int i = 0; i < questsTexts.Length; i++)
            {
                QuestProgress quest = GetQuest();
                quest.savedArrayIndex = i;

                FillQuestText(questsTexts[i], quest);

                claimButtons[i].GetComponent<Image>().sprite = nonClickable;
                claimButtons[i].GetComponentInChildren<Text>().text = "Not Complete";
                claimButtons[i].GetComponent<Button>().interactable = false;

                GameData.gameData.saveData.dailyQuests[i] = quest;
            }
            GameData.gameData.UpdateLastQuestClaim(System.DateTime.Now.AddHours(12));
        }
        else
        {
            quests = GameData.gameData.saveData.dailyQuests;
            for (int i = 0; i < questsTexts.Length; i++)
            {
                if (quests[i].rewardClaimed)
                {
                    questsTexts[i].text = "";
                    claimButtons[i].SetActive(false);
                    claimedImages[i].SetActive(true);
                }
                else
                {
                    FillQuestText(questsTexts[i], quests[i]);
                    if (quests[i].numberCollected >= quests[i].numberNeeded)
                    {
                        claimButtons[i].GetComponent<Image>().sprite = clickable;
                        claimButtons[i].GetComponentInChildren<Text>().text = "Turn in now!";
                        claimButtons[i].GetComponent<Button>().interactable = true;
                    }
                    else
                    {
                        claimButtons[i].GetComponent<Image>().sprite = nonClickable;
                        claimButtons[i].GetComponentInChildren<Text>().text = "Not Complete";
                        claimButtons[i].GetComponent<Button>().interactable = false;
                    }
                }
            }
        }
    }

    void FillQuestText(TextMeshProUGUI text, QuestProgress questProgress)
    {
        string worldId = questProgress.worldId;
        var worldInfo = CollectionController.FindItemWithId(CollectionController.Instance.worlds, worldId) as WorldInformation;
        text.text = questProgress.questDescription + "\n";
        text.text += SequentialText.ColorString("WORLD: ", Color.red) + worldId + "\n";
        text.text += SequentialText.ColorString("PROGRESS: ", Color.red) + questProgress.numberCollected + "/"
            + questProgress.numberNeeded + " <size=230>" + questProgress.tag + " " + worldInfo.BlocksName + "</size>\n";
        string rewardTypeStr = questProgress.rewardType == reward.coins ? "G" : "XP";
        text.text += SequentialText.ColorString("REWARD: ", Color.red) + questProgress.rewardAmount + rewardTypeStr;
    }

    public void ClaimReward(int i)
    {
        int reward = quests[i].rewardAmount;
        if (quests[i].rewardType == global::reward.coins)
        {
            CoinsDisplay.Instance.AddCoinsAmount(reward);
        }
        else
        {
            float maxXp = GameData.gameData.saveData.maxXPforLevelUp;
            float currXp = GameData.gameData.saveData.levelXP;
            if (currXp + reward >= maxXp)
            {
                float extra = reward - (maxXp - currXp);
                GameData.gameData.saveData.currentLevel++;
                GameData.gameData.saveData.levelXP += extra;
            }
            else
            {
                GameData.gameData.saveData.levelXP += reward;
            }
        }
        GameData.gameData.saveData.dailyQuests[i].rewardClaimed = true;
        GameData.Save();
        questsTexts[i].text = "";
        claimButtons[i].SetActive(false);
        claimedImages[i].SetActive(true);
    }
    bool IsTimeToClaim()
    {
        System.DateTime nextClaimTime;
        if (string.IsNullOrEmpty(GameData.gameData.saveData.nextPossibleQuestClaime))
        {
            nextClaimTime = System.DateTime.Now;
        }
        else
        {
            nextClaimTime = System.Convert.ToDateTime(GameData.gameData.saveData.nextPossibleQuestClaime);
        }

        return System.DateTime.Now.CompareTo(nextClaimTime) >= 0;
    }
    bool IsUnfinishedQuest()
    {
        quests = GameData.gameData.saveData.dailyQuests;
        for (int i = 0; i < quests.Length; i++)
        {
            if (!quests[i].rewardClaimed) return true;
        }
        return false;
    }
}