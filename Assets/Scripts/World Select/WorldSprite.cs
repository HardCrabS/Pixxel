using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldSprite : MonoBehaviour
{
    [SerializeField] Material blackAndWhiteMat;
    [SerializeField] WorldInformation worldInformation;
    [SerializeField] GameObject bubbleWithText;

    WorldInfoDisplay infoDisplay;
    GameObject bubbleClone;

    void Start()
    {
        bool isUnlocked = true;
        if (worldInformation != null && GameData.gameData != null)
            isUnlocked = GameData.gameData.saveData.worldIds.Contains(worldInformation.id);
        if (worldInformation == null || !isUnlocked)
        {
            if (transform.childCount > 0)
            {
                transform.GetChild(0).gameObject.SetActive(true); //lock image
            }
            GetComponent<Image>().material = blackAndWhiteMat;
            if (!isUnlocked)
            {
                GetComponent<Button>().onClick.AddListener(delegate()
                {
                    string unlockReq = 
                    CollectionController.GetUnlockRequirement(LevelReward.World, worldInformation);
                    StartCoroutine(SpawnBubble(unlockReq));
                });
            }
        }
        else
        {
            infoDisplay = WorldInfoDisplay.Instance;
            GetComponent<Button>().onClick.AddListener(OpenWorldInfoPanel);
        }
    }
    void OpenWorldInfoPanel()
    {
        if (infoDisplay != null)
        {
            infoDisplay.SetInfoPanel(worldInformation);
        }
    }

    IEnumerator SpawnBubble(string unlockRequirement)
    {
        Destroy(bubbleClone);
        bubbleClone = Instantiate(bubbleWithText, transform.position, transform.rotation, transform);
        Text text = bubbleClone.GetComponentInChildren<Text>();
        while (!text.gameObject.activeInHierarchy)
        {
            yield return null;
        }
        text.text = unlockRequirement;
        //bubbleClone.GetComponentInChildren<SequentialText>().PlayMessage(unlockRequirement);
        Destroy(bubbleClone, 3);
    }
}