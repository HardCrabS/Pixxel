using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetVisualizer : MonoBehaviour
{
    public GameObject upperImage;
    public Vector2 gravity = new Vector2(0, 0.25f);

    RectTransform[] upperImages;
    Color imageColor;
    // Start is called before the first frame update
    void Start()
    {
        upperImages = new RectTransform[transform.childCount];
        int freq = 0;
        int i = 0;
        foreach (Transform child in transform)
        {
            var scaler = child.GetComponent<MSS_ScaleChange>();
            var alpha = child.GetComponent<MSS_Alpha>();
            if (scaler)
                scaler.Freq = freq;
            if (alpha)
                alpha.Freq = freq;
            freq++;

            var go = Instantiate(upperImage, child);
            float halfHeight = child.GetComponent<RectTransform>().rect.height / 2;
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -halfHeight);
            upperImages[i] = go.GetComponent<RectTransform>();
            i++;
        }
        imageColor = upperImage.GetComponent<Image>().color;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int i = 0;
        foreach (Transform child in transform)
        {
            float halfHeight = child.GetComponent<RectTransform>().rect.height / 2;
            if (halfHeight > upperImages[i].anchoredPosition.y)
            {
                upperImages[i].anchoredPosition = new Vector2(0, halfHeight);
            }
            else
            {
                upperImages[i].anchoredPosition += gravity;
            }
            Image image = upperImages[i].GetComponent<Image>();
            imageColor.a = child.GetComponent<Image>().color.a;
            image.color = imageColor;
            i++;
        }
    }
}