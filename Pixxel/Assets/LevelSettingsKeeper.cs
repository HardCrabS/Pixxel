using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSettingsKeeper : MonoBehaviour
{
    public static LevelSettingsKeeper settingsKeeper;
    public LevelTemplate levelTemplate;

    public int worldIndex;
    public string worldName;
    public int trinketIndex;

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

    public void SetLevelTemplate(LevelTemplate _levelTemplate)
    {
        levelTemplate = _levelTemplate;
    }
}
