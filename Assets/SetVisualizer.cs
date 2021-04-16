using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetVisualizer : MonoBehaviour
{
    public GameObject upperImage;
    public Vector2 gravity = new Vector2(0, 0.25f);

    RectTransform[] upperImageRects;
    Color imageColor;

    Vector3 startHeight;
    Vector3 startColor;
    RectTransform[] visualizerObjectRects;
    MSS_ScaleChange[] scaleComponents;
    Image[] imageComponents;
    // Start is called before the first frame update
    void Start()
    {
        imageComponents = GetComponentsInChildren<Image>(); //get main images before spawning new ones

        upperImageRects = new RectTransform[transform.childCount];
        int i = 0;
        foreach (Transform child in transform)
        {
            var go = Instantiate(upperImage, child);
            float halfHeight = child.GetComponent<RectTransform>().rect.height / 2;
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -halfHeight);
            upperImageRects[i] = go.GetComponent<RectTransform>();
            i++;
        }
        imageColor = upperImage.GetComponent<Image>().color;

        visualizerObjectRects = new RectTransform[transform.childCount];
        i = 0;
        foreach (RectTransform child in transform)
        {
            visualizerObjectRects[i] = child;
            i++;
        }
        scaleComponents = GetComponentsInChildren<MSS_ScaleChange>();

        startHeight = visualizerObjectRects[0].sizeDelta;
        startColor = new Vector3(imageComponents[0].color.r, imageComponents[0].color.g, imageComponents[0].color.b);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int j = 0; j < visualizerObjectRects.Length; j++)
        {
            visualizerObjectRects[j].sizeDelta = Vector3.Lerp(visualizerObjectRects[j].sizeDelta, startHeight
                    + (MSS_SpectrumManager.SpectrumData[j] * 10 * scaleComponents[j].EffectMultiplier), 
                    scaleComponents[j].Smoothness * Time.deltaTime);

            imageComponents[j].color = new Color(startColor.x, startColor.y, startColor.z, Mathf.Lerp(imageComponents[j].color.a, 
                MSS_SpectrumManager.SpectrumData[j] * 10 * 5, scaleComponents[j].Smoothness * Time.deltaTime));
        }
        int i = 0;
        foreach (RectTransform child in visualizerObjectRects)
        {
            float halfHeight = child.rect.height / 2;
            if (halfHeight > upperImageRects[i].anchoredPosition.y)
            {
                upperImageRects[i].anchoredPosition = new Vector2(0, halfHeight);
            }
            else
            {
                upperImageRects[i].anchoredPosition += gravity;
            }
            Image image = upperImageRects[i].GetComponent<Image>(); //upper image
            imageColor.a = imageComponents[i].color.a;  //main visualizer object image
            image.color = imageColor;
            i++;
        }
    }
}