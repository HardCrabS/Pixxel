﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldInfoDisplay : MonoBehaviour 
{
    [Header("Info Panel")]
    [SerializeField] GameObject infoPanel;
    [SerializeField] GameObject GOButton;
    [SerializeField] Text worldName;
    [SerializeField] Text musicInfoText;
    [SerializeField] Text worldStyleText;
    [SerializeField] Image worldBackgroundImage;
    [SerializeField] Image[] blockImages;

    [Header("Trinkets")]
    [SerializeField] GameObject trinketsInfo;
    [SerializeField] TrinketManager trinketManager;

    [Header("Leaderboard")]
    [SerializeField] GameObject leaderboardInfo;

    public WorldInformation worldInformation;

    // Use this for initialization
    void Start () 
    {
        infoPanel.SetActive(false);
        leaderboardInfo.SetActive(false);
        trinketsInfo.SetActive(false);
    }
	
	public void ShowTrinketsInfo()
    {
        leaderboardInfo.SetActive(false);
        trinketsInfo.SetActive(true);
        trinketManager.SetTrinkets(worldInformation);
    }
    public void ShowLeaderboardInfo()
    {
        trinketsInfo.SetActive(false);
        leaderboardInfo.SetActive(true);
        LevelSettingsKeeper.settingsKeeper.levelTemplate = worldInformation.LeaderboardLevelTemplate;
    }

    public void HideInfoPanel()
    {
        infoPanel.SetActive(false);
    }

    public void SetInfoPanel(WorldInformation worldInfo)
    {
        if (worldInfo != null)
        {
            worldInformation = worldInfo;
            infoPanel.SetActive(true);
            //SetBlockImages(worldInfo);
            worldBackgroundImage.sprite = worldInfo.BackgroundSprite;
            worldName.text = worldInfo.WorldName;
            worldStyleText.text = worldInfo.Style;
            musicInfoText.text = "Music\n" + "\"" + worldInfo.MusicTitle + "\"\n" + "By " + worldInfo.MusicCreator;
            LevelSettingsKeeper.settingsKeeper.worldId = worldInfo.GetRewardId();
            trinketsInfo.SetActive(false);
            leaderboardInfo.SetActive(false);
            SetLoadButton();
        }
    }

    void SetBlockImages(WorldInformation worldInfo)
    {
        Sprite[] blockSprites = worldInfo.BlockSprites;
        for (int i = 0; i < blockSprites.Length; i++)
        {
            blockImages[i].sprite = blockSprites[i];
        }
    }

    void SetLoadButton()
    {
        GOButton.GetComponent<Button>().onClick.AddListener(LoadWorld);
    }
    void LoadWorld()
    {
        FindObjectOfType<SceneLoader>().LoadConcreteWorld(worldInformation.WorldName, worldInformation.Song);
    }
}