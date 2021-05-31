using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Image))]
public class MatGradientAnim : MonoBehaviour
{
    [SerializeField] float switchSpeedInSec = 1;

    [SerializeField] Gradient[] gradients = new Gradient[2];

    Material mat;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Image>().material;
        StartCoroutine(SwitchGradients());
    }

    IEnumerator SwitchGradients()
    {
        while (true)
        {
            for (int i = 0; i < gradients.Length; i++)
            {
                yield return mat.DOGradientColor(gradients[i], switchSpeedInSec).WaitForCompletion();
            }
        }
    }
}