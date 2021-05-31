using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldInfoDisplay : MonoBehaviour 
{
    [Header("Info Panel")]
    [SerializeField] GameObject infoPanel;
    [SerializeField] GameObject GOButton;
    [SerializeField] TextMeshProUGUI worldName;
    [SerializeField] Text musicInfoText;
    [SerializeField] Text worldStyleText;
    [SerializeField] Image worldBackgroundImage;

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
        leaderboardInfo.SetActive(true);
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
            worldBackgroundImage.sprite = worldInfo.BackgroundSprite;
            worldName.text = worldInfo.id.ToUpper();
            worldStyleText.text = worldInfo.Style;
            musicInfoText.text = "Music\n" + "\"" + worldInfo.MusicTitle + "\"\n" + "By " + worldInfo.MusicCreator;
            LevelSettingsKeeper.settingsKeeper.worldInfo = worldInfo;
            trinketsInfo.SetActive(false);
            leaderboardInfo.SetActive(true);
            SetLoadButton();
        }
    }

    void SetLoadButton()
    {
        GOButton.GetComponent<Button>().onClick.AddListener(LoadWorld);
    }
    void LoadWorld()
    {
        FindObjectOfType<SceneLoader>().LoadConcreteWorld("World", worldInformation.Song, worldInformation.MusicStartDelay);
        AudioController.Instance.Pause();
    }
}