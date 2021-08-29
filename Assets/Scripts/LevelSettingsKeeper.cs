using UnityEngine;

public class LevelSettingsKeeper : MonoBehaviour
{
    public static LevelSettingsKeeper settingsKeeper;

    public WorldInformation worldInformation;
    public WorldLoadInfo worldLoadInfo;

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