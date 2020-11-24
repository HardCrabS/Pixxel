using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeChanger : MonoBehaviour
{
    [SerializeField] GameObject goalFrame;
    [SerializeField] GameObject heartsPanel;

    // Use this for initialization
    void Awake()
    {
        if (LevelSettingsKeeper.settingsKeeper != null)
        {
            bool mode = LevelSettingsKeeper.settingsKeeper.levelTemplate.isLeaderboard;
            SetGameMode(mode);
        }
    }
    void SetGameMode(bool isLeaderboard)
    {
        if (isLeaderboard)
        {
            goalFrame.SetActive(false);
            heartsPanel.SetActive(true);
        }
        else
        {
            goalFrame.SetActive(true);
            heartsPanel.SetActive(false);
        }
    }
}
