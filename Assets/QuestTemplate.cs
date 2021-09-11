using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestTemplate : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] Text rewardText;
    [SerializeField] Text amountNeededText;
    [SerializeField] Image worldImage;
    [SerializeField] Image blockImage;
    [SerializeField] Image rewardImage;
    [SerializeField] GameObject questCompletedImage;

    [SerializeField] Sprite xpSprite;
    [SerializeField] Sprite coinSprite;

    public void SetQuestInfo(string descr, int reward, int amountNeeded, int amountCollected, Sprite world, Sprite block, reward qReward)
    {
        description.text = descr;
        rewardText.text = "x" + reward;
        amountNeededText.text = amountCollected 
            + SequentialText.ColorString("/" + amountNeeded, new Color(1, 0.9f, 0));

        worldImage.sprite = world;
        blockImage.sprite = block;
        rewardImage.sprite = qReward == global::reward.coins ? coinSprite : xpSprite;
    }
    public void HideQuestInfo()
    {
        description.transform.parent.gameObject.SetActive(false);
        questCompletedImage.SetActive(true);
    }
}