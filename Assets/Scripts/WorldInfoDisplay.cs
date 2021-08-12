using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WorldInfoDisplay : MonoBehaviour 
{
    [Header("Info Panel")]
    [SerializeField] UI_Screen infoPanel;
    [SerializeField] GameObject GOButton;
    [SerializeField] TextMeshProUGUI worldName;
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
    WorldLoadInfo worldLoadInfo;

    public delegate void OnWorldClicked(string worldId);
    public event OnWorldClicked onWorldClicked;

    public static WorldInfoDisplay Instance;

    WorldLoader worldLoader = new WorldLoader();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    // Use this for initialization
    void Start () 
    {
        leaderboardInfo.SetActive(true);
        trinketsInfo.SetActive(false);
    }
	
	public void ShowTrinketsInfo()
    {
        leaderboardInfo.SetActive(false);
        trinketsInfo.SetActive(true);
        trinketManager.SetTrinkets(worldLoadInfo);
    }
    public void ShowLeaderboardInfo()
    {
        trinketsInfo.SetActive(false);
        leaderboardInfo.SetActive(true);
    }
    public void SetInfoPanel(WorldInformation worldInfo)
    {
        if (worldInfo != null)
        {
            worldLoader.LoadWorldInfoAsync(worldInfo.id, SetWorldLoadInfo);//load world info

            worldInformation = worldInfo;
            worldBackgroundImage.sprite = worldInfo.BackgroundSprite;
            worldName.text = worldInfo.id.ToUpper();
            worldStyleText.text = worldInfo.Style;
            musicInfoText.text = worldInfo.MusicCreator + " - " + worldInfo.MusicTitle;
            trinketsInfo.SetActive(false);
            leaderboardInfo.SetActive(true);
            UI_System.Instance.SwitchScreens(infoPanel);
            SetLoadButton();
        }
    }
    void SetWorldLoadInfo(WorldLoadInfo worldLoadInfo)
    {
        this.worldLoadInfo = worldLoadInfo;//save loaded world

        //set whoever needs it
        SetBlockSprites(worldLoadInfo);
        LevelSettingsKeeper.settingsKeeper.worldInformation = worldInformation;
        LevelSettingsKeeper.settingsKeeper.worldLoadInfo = worldLoadInfo;
    }
    void SetBlockSprites(WorldLoadInfo worldLoadInfo)
    {
        var allBlocks = worldLoadInfo.boxes;
        int i = 0;
        for (; i < allBlocks.Length; i++)
        {
            blockImages[i].gameObject.SetActive(true);
            blockImages[i].sprite = allBlocks[i].GetComponent<SpriteRenderer>().sprite;
        }
        for (; i < blockImages.Length; i++)
        {
            //turn off left images
            blockImages[i].gameObject.SetActive(false);
        }
    }
    void SetLoadButton()
    {
        var goButton = GOButton.GetComponent<Button>();
        goButton.onClick.RemoveListener(LoadWorld);
        goButton.onClick.AddListener(LoadWorld);
    }
    void LoadWorld()
    {
        FindObjectOfType<SceneLoader>().LoadConcreteWorld("World", worldLoadInfo.song, worldInformation.MusicStartDelay);
        AudioController.Instance.Pause();
    }
}