using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[System.Serializable]
public class MatAnimatorValues
{
    public string propertyName;
    public float startValue;
    public float endValue;
    public bool loopBackAndForth = false;
    public float durationInSec = 1;
    public float timeToWait = 1;

    public MatAnimatorValues(string propertyName, float startValue, float endValue,
        bool loopBackAndForth, float durationInSec, float timeToWait)
    {
        this.propertyName = propertyName;
        this.startValue = startValue;
        this.endValue = endValue;
        this.loopBackAndForth = loopBackAndForth;
        this.durationInSec = durationInSec;
        this.timeToWait = timeToWait;
    }
}

[RequireComponent(typeof(Image))]
public class MatPropertyAnim : MonoBehaviour
{
    [SerializeField] string propertyName;
    [SerializeField] float startValue;
    [SerializeField] float endValue;
    [SerializeField] bool loopBackAndForth = false;
    [SerializeField] bool maskedMat = false;
    [SerializeField] bool animateOnStart = true;

    [Header("Time")]
    [SerializeField] float durationInSec = 1;
    [SerializeField] float timeToWait = 1;

    Material mat;
    void Start()
    {
        if (!maskedMat)
            mat = GetComponent<Image>().material;
        else
            mat = GetComponent<Image>().materialForRendering;

        if(animateOnStart)
            Animate();
    }

    public void Animate()
    {
        if(!mat)
        {
            print(gameObject.name);
            return;
        }
        if (mat.HasProperty(propertyName))
        {
            if (loopBackAndForth)
                StartCoroutine(AnimatePropertyBackAndForth());
            else
                StartCoroutine(AnimateProperty());
        }
        //else
            //Debug.LogError(gameObject.name + ". No such property found in material: " + propertyName);
    }

    public void SetValues(MatAnimatorValues animatorValues)
    {
        if (!maskedMat)
            mat = GetComponent<Image>().material;
        else
            mat = GetComponent<Image>().materialForRendering;

        maskedMat = true;
        animateOnStart = false;
        this.propertyName = animatorValues.propertyName;
        this.startValue = animatorValues.startValue;
        this.endValue = animatorValues.endValue;
        this.loopBackAndForth = animatorValues.loopBackAndForth;
        this.durationInSec = animatorValues.durationInSec;
        this.timeToWait = animatorValues.timeToWait;
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
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}