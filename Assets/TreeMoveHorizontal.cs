using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeMoveHorizontal : MonoBehaviour
{
    [SerializeField] Vector2 destination;
    [SerializeField] float duration = 1;
    [SerializeField] float timeBeforeStart = 1; // THIS LOlllllllllllllllllllllllllllllllllllll
    [SerializeField] float timeBeforeRestart = 1; // THIS LOlllllllllllllllllllllllllllllllllllll

  
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
            yield return new WaitForSeconds(timeBeforeStart);
            yield return rectTransform.DOAnchorPos(destination, duration).WaitForCompletion();
            yield return new WaitForSeconds(timeBeforeRestart); // THIS LOlllllllllllllllllllllllllllllllllllll
            rectTransform.anchoredPosition = startPos;
           
        }
    }
}