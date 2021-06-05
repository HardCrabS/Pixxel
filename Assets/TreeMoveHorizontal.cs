using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeMoveHorizontal : MonoBehaviour
{
    [SerializeField] Vector2 destination;
    [SerializeField] float duration = 1;

    Vector2 startPos;
    RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition;

        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        while (true)
        {
            yield return rectTransform.DOAnchorPos(destination, duration).WaitForCompletion();
            rectTransform.anchoredPosition = startPos;
        }
    }
}