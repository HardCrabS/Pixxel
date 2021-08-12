using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Collection/World Load Info")]
public class WorldLoadInfo : ScriptableObject
{
    public string id;

    public AudioClip song;
    public LevelTemplate template;
    public GameObject[] boxes;
    public int[] scoreWorldLevels = new int[10];
    public LevelTemplate[] trinketTemplates;

    [Header("Game UI")]
    public Material scoreTextMaterial;
    public GameObject backgroundCanvas;
    public Sprite blocksPanelSprite;
    public Color blocksPanelColor;
    public Color blocksPanelGlowColor;
    public Color visualizerColor;
    public Material visualizerMaterial;
}