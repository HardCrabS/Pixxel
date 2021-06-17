using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeBetweenLayers : MonoBehaviour
{
    [SerializeField] float timeBetweenFades = 10;
    [SerializeField] float fadeDuration = 2;
    [SerializeField] Image[] layers;
    // Start is called before the first frame update
    void Start()
    {
        if (layers.Length <= 1) return;
        SortBySiblingIndex(layers);
        SetLayersColor();
        StartCoroutine(FadeLayers());
    }

    IEnumerator FadeLayers()
    {
        int currLayerIndex = layers.Length - 1;
        int targetLayerIndex = currLayerIndex - 1;
        int incr = -1;
        while(true)
        {
            yield return new WaitForSeconds(timeBetweenFades);

            //fade in next layer
            Image layerToFadeIn = layers[targetLayerIndex];
            layerToFadeIn.material.DOFade(1, fadeDuration);

            //fade to 0 if alpha = 1, to 1 if alpha = 0
            float currTargetFadeValue = layers[currLayerIndex].material.GetColor("_Color").a == 1 ? 0 : 1;
            //fading away slower so an empty background isn't visible 
            yield return layers[currLayerIndex].material.DOFade(currTargetFadeValue, fadeDuration + 3).WaitForCompletion();

            //loop from 0 to length and back
            currLayerIndex += incr;
            if (targetLayerIndex - 1 < 0) incr = 1;
            if (targetLayerIndex + 1 >= layers.Length) incr = -1;
            targetLayerIndex = currLayerIndex + incr;
        }
    }
    void SetLayersColor()
    {
        for (int i = 0; i < layers.Length; i++)
        {
            layers[i].material.SetColor("_Color", Color.white);
        }
    }
    //sort array of images based on their hierarchy index
    //because UI element visibility depends on it's position in hierarchy
    void SortBySiblingIndex(Image[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            for (int j = 0; j < arr.Length; j++)
            {
                if(arr[j].transform.GetSiblingIndex() > arr[i].transform.GetSiblingIndex())
                {
                    var temp = arr[j];
                    arr[j] = arr[i];
                    arr[i] = temp;
                }
            }
        }
    }
}