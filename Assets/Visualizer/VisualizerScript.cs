using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualizerScript : MonoBehaviour
{
    public float minHeight = 15.0f;
    public float maxHeight = 425.0f;
    public float updateSentivity = 10.0f;
    public Vector2 gravity = new Vector2(0, 0.25f);
    public GameObject upperImage;
    [Space(15)]
    public AudioClip audioClip;
    public bool loop = true;
    [Space(15), Range(64, 8192)]
    public int visualizerSimples = 64;
    public float[] spectrumData;
    VisualizerObjectScript[] visualizerObjects;
    AudioSource audioSource;

    RectTransform[] upperImages;
    float visualObjHalfHeight;

    void Awake()
    {
        if(PlayerPrefsController.GetMasterVisualizer() == 0)
        {
            GameObject canvas = GetComponentInParent<Canvas>().gameObject;
            canvas.SetActive(false);
        }
    }

    // Use this for initialization
    void Start()
    {
        visualizerObjects = GetComponentsInChildren<VisualizerObjectScript>();
        upperImages = new RectTransform[visualizerObjects.Length];
        visualObjHalfHeight = visualizerObjects[0].GetComponent<RectTransform>().rect.height / 2;
        audioSource = AudioController.Instance.gameObject.GetComponent<AudioSource>();
        for (int i = 0; i < visualizerObjects.Length; i++)
        {
            var go = Instantiate(upperImage, visualizerObjects[i].transform);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -visualObjHalfHeight);
            upperImages[i] = go.GetComponent<RectTransform>();
        }
        spectrumData = new float[visualizerSimples];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);
        
        for (int i = 0; i < visualizerObjects.Length; i++)
        {
            Vector2 newSize = visualizerObjects[i].GetComponent<RectTransform>().rect.size;

            newSize.y = Mathf.Clamp(Mathf.Lerp(newSize.y, spectrumData[i] * (1300 + i * i * 13), updateSentivity * 0.5f), minHeight, maxHeight);
            //newSize.y = Mathf.Clamp(Mathf.Lerp(newSize.y, minHeight + (spectrumData[i] * muliplier * (maxHeight - minHeight) * 5.0f), updateSentivity * 0.5f), minHeight, maxHeight);
            visualizerObjects[i].GetComponent<RectTransform>().sizeDelta = newSize;
            float yPos = Mathf.Clamp(spectrumData[i] * (50 + i * i), 0, 50);
            if (-newSize.y / 2 < upperImages[i].anchoredPosition.y)
            {
                upperImages[i].anchoredPosition = new Vector2(0, -newSize.y / 2);
            }
            else
            {
                upperImages[i].anchoredPosition += gravity;
            }
        }
    }
}