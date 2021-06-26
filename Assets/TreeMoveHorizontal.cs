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

    [Header("Random values")]
    [Range(0, 100)] [SerializeField] float durationDeviationPercent = 0;
    [SerializeField] float upperYOffset = 0;//start position Y offsets
    [SerializeField] float lowerYOffset = 0;
    [Tooltip("Should destination change with Start position?")]
    [SerializeField] bool destinationIsRelative = false;
  
    Vector2 startPos;
    RectTransform rectTransform;

    void Start()
    {
        rectTransform = transform as RectTransform;
        float yRandOffset = CalculateYOffset();
        startPos = rectTransform.anchoredPosition;
        startPos.y += yRandOffset;
        rectTransform.anchoredPosition = startPos;

        if(destinationIsRelative)
        {
            destination.y += yRandOffset;
        }

        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBeforeStart);
            yield return rectTransform.DOAnchorPos(destination, CalculateDuration()).WaitForCompletion();
            yield return new WaitForSeconds(timeBeforeRestart); // THIS LOlllllllllllllllllllllllllllllllllllll
            rectTransform.anchoredPosition = startPos;
        }
    }
    float CalculateYOffset()
    {
        return Random.Range(lowerYOffset, upperYOffset);
    }
    float CalculateDuration()
    {
        float moveDuration = duration;
        float deviationPercent = Random.Range(-durationDeviationPercent, durationDeviationPercent);
        moveDuration += moveDuration * deviationPercent * 0.01f;
        return moveDuration;
    }
    Matrix4x4 GetCanvasMatrix()
    {
        Canvas _Canvas = GetComponentInParent<Canvas>();
        RectTransform rectTr = _Canvas.transform as RectTransform;
        Matrix4x4 canvasMatrix = rectTr.localToWorldMatrix;
        //canvasMatrix *= Matrix4x4.Translate(-rectTr.sizeDelta / 2);
        return canvasMatrix;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.matrix = GetCanvasMatrix();
        Gizmos.color = Color.green;

        //max Y position
        Gizmos.DrawLine(transform.localPosition, transform.localPosition + Vector3.up * upperYOffset);
        Gizmos.DrawSphere(transform.localPosition + Vector3.up * upperYOffset, 10);

        //min Y position
        Gizmos.DrawLine(transform.localPosition, transform.localPosition + Vector3.up * lowerYOffset);
        Gizmos.DrawSphere(transform.localPosition + Vector3.up * lowerYOffset, 10);
    }
}