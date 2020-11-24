using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSettingsKeeper : MonoBehaviour
{
    public static LevelSettingsKeeper settingsKeeper;
    public LevelTemplate levelTemplate;

    public string worldId;

    void Awake()
    {
        if (settingsKeeper == null)
        {
            DontDestroyOnLoad(this.gameObject);
            settingsKeeper = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}