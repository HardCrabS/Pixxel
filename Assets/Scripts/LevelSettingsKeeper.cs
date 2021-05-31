using UnityEngine;

public class LevelSettingsKeeper : MonoBehaviour
{
    public static LevelSettingsKeeper settingsKeeper;

    public WorldInformation worldInfo;

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