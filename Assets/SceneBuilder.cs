using TMPro;
using UnityEngine;

public class SceneBuilder : MonoBehaviour
{
    [SerializeField] Camera mainCamera; //set to BG Screen space Canvas
    [Header("Game UI")]
    [SerializeField] GameObject scoreText;
    [SerializeField] GameObject backgroundCanvas;

    [Header("Blocks Behind Panel")]
    [SerializeField] SpriteRenderer blocksPanel;
    [SerializeField] SpriteRenderer blockPanelGlow;

    [Header("Visualizer")]
    [SerializeField] SetVisualizer visualizer;

    LevelSettingsKeeper settingsKeeper;
    void Awake()
    {
        if (!LevelSettingsKeeper.settingsKeeper) return;
        settingsKeeper = LevelSettingsKeeper.settingsKeeper;
        SpawnGameUI();
    }

    public void SpawnGameUI()
    {
        if (!settingsKeeper) return;

        Material scoreMaterial = settingsKeeper.worldLoadInfo.scoreTextMaterial;
        SetScoreTextMat(scoreMaterial);

        GameObject bgCanvas = settingsKeeper.worldLoadInfo.backgroundCanvas;
        SpawnBGCanvas(bgCanvas);

        Sprite panelSprite = settingsKeeper.worldLoadInfo.blocksPanelSprite;
        Color panelColor = settingsKeeper.worldLoadInfo.blocksPanelColor;
        Color panelGlowColor = settingsKeeper.worldLoadInfo.blocksPanelGlowColor;
        SetBlocksPanel(panelSprite, panelColor, panelGlowColor);

        Material visualizerMaterial = settingsKeeper.worldLoadInfo.visualizerMaterial;
        if (visualizerMaterial)
        {
            visualizer.SetMaterial(visualizerMaterial);
        }
        else
        {
            //set color if no visualizer material provided
            Color visualizerColor = settingsKeeper.worldLoadInfo.visualizerColor;
            visualizer.SetColor(visualizerColor);
        }
    }
    void SpawnBGCanvas(GameObject bgCanvas)
    {
        if (!bgCanvas)
        {
            Debug.LogWarning("Background has not been assigned in World Info SO.");
            return;
        }
        if (backgroundCanvas)
            Destroy(backgroundCanvas);//destroy existing background canvas
        GameObject canvas = Instantiate(bgCanvas);
        canvas.GetComponent<Canvas>().worldCamera = mainCamera;
    }
    void SetScoreTextMat(Material scoreMaterial)
    {
        if (!scoreMaterial)
        {
            Debug.LogWarning("Score material has not been assigned in World Info SO.");
            return;
        }
        TextMeshProUGUI text = scoreText.GetComponent<TextMeshProUGUI>();
        text.fontMaterial = scoreMaterial;
    }
    void SetBlocksPanel(Sprite panelSprite, Color panelColor, Color panelGlowColor)
    {
        if (panelSprite)
            blocksPanel.sprite = panelSprite;
        else
            Debug.LogWarning("Blocks background panel Texture has not been assigned in World Info SO.");

        blocksPanel.color = panelColor;
        blockPanelGlow.color = panelGlowColor;
    }
}