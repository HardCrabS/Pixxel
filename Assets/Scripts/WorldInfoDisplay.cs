using TMPro;
using UnityEngine;
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

    // Use this for initialization
    void Start () 
    {
        //infoPanel.SetActive(false);
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
    public void SetInfoPanel(WorldInformation worldInfo)
    {
        if (worldInfo != null)
        {
            worldInformation = worldInfo;
            worldBackgroundImage.sprite = worldInfo.BackgroundSprite;
            worldName.text = worldInfo.id.ToUpper();
            SetBlockSprites(worldInfo);
            worldStyleText.text = worldInfo.Style;
            musicInfoText.text = worldInfo.MusicCreator + " - " + worldInfo.MusicTitle;
            LevelSettingsKeeper.settingsKeeper.worldInfo = worldInfo;
            trinketsInfo.SetActive(false);
            leaderboardInfo.SetActive(true);
            UI_System.Instance.SwitchScreens(infoPanel);
            SetLoadButton();
        }
    }
    void SetBlockSprites(WorldInformation worldInfo)
    {
        var allBlocks = worldInfo.Boxes;
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
        FindObjectOfType<SceneLoader>().LoadConcreteWorld("World", worldInformation.Song, worldInformation.MusicStartDelay, useLoadingPanel:true);
        AudioController.Instance.Pause();
    }
}