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
    const float defaultDistBetweenObj = 75;

    void Start()
    {
        //MoveToFirstObject();
    }
    IEnumerator SetSpeedDelayed()
    {
        if (allObjects.Count < 2) yield break;
        yield return new WaitForSeconds(1f);
        float distBetwObjects = Mathf.Abs(allObjects[1].transform.position.x - allObjects[0].transform.position.x);
        scrollSpeed *= distBetwObjects / defaultDistBetweenObj; //for different screen resolutions, i.e. distBetwObjects bigger on large screens
        print(distBetwObjects);
        print(scrollSpeed);
    }
    public void MoveToFirstObject()
    {
        GameObject objToMove = allObjects[0];
        Vector3 centerOfScrolling = transform.position;
        Vector3 currPos = containerToMove.position;
        Vector3 dif = centerOfScrolling - objToMove.transform.position;

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
            //StartCoroutine(ScrollToObject(allObjects[currObjIndex], dir));  //scroll to next object
            StartCoroutine(ScaleCurrentObj(allObjects[currObjIndex], scaleMultiplier));
        }
    }

    IEnumerator ScrollToObject(GameObject objToMove, int dir)
    {
        Vector3 centerOfScrolling = transform.position;
        Vector3 currPos = containerToMove.position;
        while ((Mathf.Abs(centerOfScrolling.x) - Mathf.Abs(objToMove.transform.position.x)) * (-dir) > 0)
        {
            float step = scrollSpeed * Time.deltaTime;
            currPos += new Vector3(-dir * step, 0);
            print("step is: " + step);
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
            targetScale = Vector2.one; //scale down
            objIsScaled = false;
        }
        else
        {
            targetScale = new Vector2(scaleMultiplier, scaleMultiplier); //scale up and save init scale
            objIsScaled = true;
        }

        while (Vector2.Distance(currScale, targetScale) > 0)
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

    void OnEnable()
    {
        StartCoroutine(SetSpeedDelayed());
    }
}