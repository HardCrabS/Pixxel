using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollWithButtons : MonoBehaviour
{
    [SerializeField] float scrollSpeed = 1;
    [SerializeField] float scaleSpeed = 1;
    [SerializeField] float scaleMultiplier = 1.5f;
    [SerializeField] Transform containerToMove;

    public List<GameObject> allObjects = new List<GameObject>();

    int currObjIndex = 0;
    bool objIsScaled = false;
    Vector2 initObjScale;

    public void MoveToFirstObject()
    {
        GameObject objToMove = allObjects[0];
        Vector3 centerOfScrolling = transform.position;
        Vector3 currPos = containerToMove.position;
        Vector3 dif = centerOfScrolling - objToMove.transform.position;
        containerToMove.position = new Vector2(containerToMove.position.x + dif.x - 60, containerToMove.position.y);
        float dir = Mathf.Sign(Vector2.Distance(objToMove.transform.position, centerOfScrolling));

        initObjScale = objToMove.transform.localScale;
        objToMove.transform.localScale *= scaleMultiplier;
        objIsScaled = true;
    }

    public void ScrollInDirection(int dir)
    {
        if (currObjIndex + dir < allObjects.Count && currObjIndex + dir >= 0)
        {
            StopAllCoroutines();
            if (objIsScaled)
            {
                StartCoroutine(ScaleCurrentObj(allObjects[currObjIndex])); //scale down object
            }
            currObjIndex += dir;
            allObjects[currObjIndex].GetComponent<Button>().onClick.Invoke(); //trigger button which is on every object
            StartCoroutine(ScrollToObject(allObjects[currObjIndex], dir));  //scroll to next object
        }
    }

    IEnumerator ScrollToObject(GameObject objToMove, int dir)
    {
        Vector3 centerOfScrolling = transform.position;
        Vector3 currPos = containerToMove.position;
        while (Mathf.Abs(Mathf.Abs(objToMove.transform.position.x) - Mathf.Abs(centerOfScrolling.x)) > 1)
        {
            float step = scrollSpeed * Time.deltaTime;
            currPos = Vector3.MoveTowards(currPos,
                new Vector3(centerOfScrolling.x + (-dir * 100), containerToMove.position.y, 0), step);
            containerToMove.position = currPos;  //moving whole container to match object to center

            yield return null;
        }
        StartCoroutine(ScaleCurrentObj(objToMove, scaleMultiplier));
    }

    IEnumerator ScaleCurrentObj(GameObject objToScale, float multiplier = 0)
    {
        Vector2 currScale = objToScale.transform.localScale;
        Vector2 targetScale;
        if (multiplier == 0)
        {
            targetScale = initObjScale; //scale down if multiplier 0
            objIsScaled = false;
        }
        else
        {
            targetScale = objToScale.transform.localScale * multiplier; //scale up and save init scale
            initObjScale = objToScale.transform.localScale;
            objIsScaled = true;
        }

        while (Vector2.Distance(currScale, targetScale) > Mathf.Epsilon)
        {
            float step = scaleSpeed * Time.deltaTime;
            currScale = Vector3.MoveTowards(currScale, targetScale, step);
            objToScale.transform.localScale = currScale;

            yield return null;
        }
    }

    public void AddObject(GameObject ob)
    {
        allObjects.Add(ob);
    }
}