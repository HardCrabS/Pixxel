using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum reward
{
    coins,
    XP
}

[System.Serializable]
public struct QuestProgress
{
    public QuestProgress(string _tag, int _numberNeeded, int _worldIndex, int _savedArrayIndex, int _questTemplateIndex, int _numberCollected = 0, bool _rewardClaimed = false)
    {
        tag = _tag;
        numberNeeded = _numberNeeded;
        numberCollected = _numberCollected;
        worldIndex = _worldIndex;
        savedArrayIndex = _savedArrayIndex;
        questTemplateIndex = _questTemplateIndex;
        rewardClaimed = _rewardClaimed;
    }
    public string tag;
    public int numberNeeded;
    public int numberCollected;
    public int worldIndex;
    public bool rewardClaimed;

    public int questTemplateIndex;
    public int savedArrayIndex;
}
public class DailyQuestManager : MonoBehaviour
{
    [SerializeField] CoinsDisplay coinsDisplay;
    [SerializeField] GameObject questPanel;
    [SerializeField] Sprite clickable;
    [SerializeField] Sprite nonClickable;

    [SerializeField] QuestTemplate[] allQuests;
    [SerializeField] Text[] QuestsText;
    [SerializeField] GameObject[] claimButtons;
    [SerializeField] GameObject[] claimedImages;

    [SerializeField] WorldInformation[] allWorlds;

    QuestProgress[] quests;

    public void ShowQuestPanel()
    {
        questPanel.SetActive(true);
        PickRandomQuests();
    }

    private void PickRandomQuests()
    {
        System.DateTime lastClaim;
        if (string.IsNullOrEmpty(GameData.gameData.saveData.lastTimeQuestClaimed))
        {
            lastClaim = System.DateTime.Now;
        }
        else
        {
            lastClaim = System.Convert.ToDateTime(GameData.gameData.saveData.lastTimeQuestClaimed);
        }

        if (System.DateTime.Now.CompareTo(lastClaim) >= 0)
        {
            for (int i = 0; i < QuestsText.Length; i++)
            {
                int randQuest = Random.Range(0, allQuests.Length);
                QuestTemplate questTemplate = allQuests[randQuest];
                QuestProgress quest = new QuestProgress(questTemplate.Tag, questTemplate.NumberNeeded, questTemplate.WorldIndex, i, randQuest);

                FillQuestText(QuestsText[i], questTemplate, quest);

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
            for (int i = 0; i < QuestsText.Length; i++)
            {
                if (quests[i].rewardClaimed)
                {
                    QuestsText[i].text = "";
                    claimButtons[i].SetActive(false);
                    claimedImages[i].SetActive(true);
                }
                else
                {
                    FillQuestText(QuestsText[i], allQuests[quests[i].questTemplateIndex], quests[i]);
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

    void FillQuestText(Text text, QuestTemplate questTemplate, QuestProgress questProgress)
    {
        text.text = questTemplate.QuestDescription + "\n";
        text.text += "<color=red>WORLD:</color> " + allWorlds[questProgress.worldIndex].WorldName + "\n";
        text.text += "<color=red>PROGRESS:</color> " + questProgress.numberCollected + "/" + questTemplate.NumberNeeded + "\n";
        text.text += "<color=red>REWARD:</color> " + questTemplate.Reward + " " + questTemplate.RewardType;
    }

    public void ClaimReward(int i)
    {
        QuestTemplate questTemplate = allQuests[quests[i].questTemplateIndex];
        int reward = questTemplate.Reward;
        if (questTemplate.RewardType == global::reward.coins)
        {
            coinsDisplay.AddCoinsAmount(reward);
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
        GameData.gameData.Save();
        QuestsText[i].text = "";
        claimButtons[i].SetActive(false);
        claimedImages[i].SetActive(true);
    }   
    /*
    private void GenerateQuests()
    {
        System.DateTime lastClaim;
        if (string.IsNullOrEmpty(GameData.gameData.saveData.lastTimeQuestClaimed))
        {
            lastClaim = System.DateTime.Now;
        }
        else
        {
            lastClaim = System.Convert.ToDateTime(GameData.gameData.saveData.lastTimeQuestClaimed);
        }

        if (System.DateTime.Now.CompareTo(lastClaim) >= 0)
        {
            for (int i = 0; i < requirementsTexts.Length; i++)
            {
                QuestProgress quest = GenerateRandomQuest(i);
                requirementsTexts[i].text = "Requirements: " + quest.requirements;
                rewardTexts[i].text = "Reward: " + quest.reward + " " + quest.rewardType;
                progressTexts[i].text = quest.numberCollected + "/" + quest.numberNeeded;
                checkMarks[i].SetActive(false);
                GameData.gameData.saveData.dailyQuests[i] = quest;
            }

            GameData.gameData.UpdateLastQuestClaim(System.DateTime.Now.AddHours(12));
        }
        else
        {
            QuestProgress[] quests = GameData.gameData.saveData.dailyQuests;
            for (int i = 0; i < requirementsTexts.Length; i++)
            {
                requirementsTexts[i].text = "Requirements: " + quests[i].requirements;
                rewardTexts[i].text = "Reward: " + quests[i].reward + " " + quests[i].rewardType;
                if (quests[i].numberCollected >= quests[i].numberNeeded)
                    checkMarks[i].SetActive(true);
                else
                    progressTexts[i].text = quests[i].numberCollected + "/" + quests[i].numberNeeded;
            }
        }
    }

    private QuestProgress GenerateRandomQuest(int arrIndex)
    {
        int randomValue = Random.Range(0, 8);
        int randomWorld = Random.Range(0, allWorlds.Length);
        string blockName = "";
        rewardType = GetRandomEnum<reward>();
        numberNeeded = Random.Range(minNumberNeeded, maxNumberNeeded);
        reward = Random.Range(minReward, maxReward);

        if (randomValue > 1)
        {
            int randTag = Random.Range(0, allWorlds[randomWorld].Boxes.Length);
            tag = allWorlds[randomWorld].Boxes[randTag].tag;
            blockName = allWorlds[randomWorld].Boxes[randTag].GetComponent<Box>().blockName;
        }
        else
        {
            if (randomValue == 0)
            {
                tag = "Score";
                numberNeeded *= 5;
            }
            else
            {
                tag = "Breakable";
                blockName = "Breakable";
            }
        }
        if (rewardType == global::reward.XP)
            reward *= 2;

        if (tag != "Score")
        {
            requirements = "Destroy " + numberNeeded + " " + blockName + " in " + allWorlds[randomWorld].WorldName + " world!";
        }
        else
        {
            requirements = "Reach " + numberNeeded + " points in " + allWorlds[randomWorld].WorldName + " world!";
        }
        QuestProgress quest = new QuestProgress(requirements, tag, rewardType, numberNeeded, randomWorld, reward, arrIndex);
        return quest;
    }
    static T GetRandomEnum<T>()
    {
        System.Array A = System.Enum.GetValues(typeof(T));
        T V = (T)A.GetValue(UnityEngine.Random.Range(0, A.Length));
        return V;
    }*/
}