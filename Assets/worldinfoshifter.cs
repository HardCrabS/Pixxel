using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class worldinfoshifter : MonoBehaviour
{
    [SerializeField] WorldInformation[] worldInfos;
    [SerializeField] WorldLoadInfo[] worldLoadInfos;

    public string id;
    public AudioClip clip;
    public GameObject canvas;

    public void ShiftValues()
    {
        for (int i = 0; i < worldInfos.Length; i++)
        {
            /*id = worldInfos[i].id;
            worldLoadInfos[i].id = id;

            clip = worldInfos[i].Song;
            worldLoadInfos[i].song = clip;
            LevelTemplate levelTemplate = worldInfos[i].LeaderboardLevelTemplate;
            worldLoadInfos[i].template = levelTemplate;
            worldLoadInfos[i].scoreWorldLevels = worldInfos[i].ScoreWorldLevels;
            worldLoadInfos[i].trinketTemplates = worldInfos[i].TrinketLevelTemplates;

            worldLoadInfos[i].scoreTextMaterial = worldInfos[i].ScoreTextMat;
            canvas = worldInfos[i].BackgroundCanvas;
            worldLoadInfos[i].backgroundCanvas = canvas;
            worldLoadInfos[i].blocksPanelSprite = worldInfos[i].BlocksPanelSprite;
            worldLoadInfos[i].blocksPanelColor = worldInfos[i].BlocksPanelColor;
            worldLoadInfos[i].blocksPanelGlowColor = worldInfos[i].BlocksPanelGlowColor;
            worldLoadInfos[i].visualizerColor = worldInfos[i].VisualizerColor;
            worldLoadInfos[i].visualizerMaterial = worldInfos[i].VisualizerMaterial;*/
            /*worldLoadInfos[i].name = (i+1) + worldLoadInfos[i].id + "_LoadInfo";*/
            /*string guid;
            long file;

            if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(worldLoadInfos[i], out guid, out file))
            {

            worldInfos[i].worldLoadInfoRef = new AssetReference(guid);
            }
            EditorUtility.SetDirty(worldInfos[i]);*/
        }
    }
}