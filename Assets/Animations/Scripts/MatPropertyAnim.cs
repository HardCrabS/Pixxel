using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Image))]
public class MatPropertyAnim : MonoBehaviour
{
    [SerializeField] string propertyName;
    [SerializeField] float startValue;
    [SerializeField] float endValue;
    [SerializeField] bool loopBackAndForth = false;

    [Header("Time")]
    [SerializeField] float durationInSec = 1;
    [SerializeField] float timeToWait = 1;

    Material mat;
    void Start()
    {
        mat = GetComponent<Image>().material;
        if (mat.HasProperty(propertyName))
        {
            if (loopBackAndForth)
                StartCoroutine(AnimatePropertyBackAndForth());
            else
                StartCoroutine(AnimateProperty());
        }
        else
            Debug.LogError(gameObject.name + ". No such property found in material: " + propertyName);
    }
    IEnumerator AnimateProperty()
    {
        while (true)
        {
            yield return mat.DOFloat(endValue, propertyName, durationInSec).WaitForCompletion();
            yield return new WaitForSeconds(timeToWait);
            mat.SetFloat(propertyName, startValue);
        }
    }
    IEnumerator AnimatePropertyBackAndForth()
    {
        while (true)
        {
            yield return mat.DOFloat(endValue, propertyName, durationInSec).WaitForCompletion();
            yield return new WaitForSeconds(timeToWait);
            yield return mat.DOFloat(startValue, propertyName, durationInSec).WaitForCompletion();
            yield return new WaitForSeconds(timeToWait);
        }
    }
}