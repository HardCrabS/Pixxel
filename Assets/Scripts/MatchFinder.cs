using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchFinder : MonoBehaviour
{
    GridA grid;
    public List<GameObject> currentMatches = new List<GameObject>();

    public static MatchFinder Instance;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        grid = GridA.Instance;
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCoroutine());
    }

    private IEnumerator FindAllMatchesCoroutine()
    {
        yield return new WaitForSeconds(.2f);
        for (int x = 0; x < grid.width; x++)
        {
            for (int y = 0; y < grid.hight; y++)
            {
                GameObject currentBox = grid.allBoxes[x, y];
                if (grid.allBoxes[x, y] != null)
                {
                    // left and right
                    if (x > 0 && x < grid.width - 1)
                    {
                        GameObject leftBox = grid.allBoxes[x - 1, y];
                        GameObject rightBox = grid.allBoxes[x + 1, y];
                        if (leftBox && rightBox)
                        {
                            bool leftRightSameTag = leftBox.tag == currentBox.tag && rightBox.tag == currentBox.tag;
                            bool warpedBlockMatch = (leftBox.tag == currentBox.tag || leftBox.GetComponent<Box>().Warped)
                                && (rightBox.tag == currentBox.tag || rightBox.GetComponent<Box>().Warped)
                                || (currentBox.GetComponent<Box>().Warped && (leftBox.tag == rightBox.tag));
                            if (leftRightSameTag || warpedBlockMatch)
                            {
                                if(warpedBlockMatch)
                                {
                                    //set warped block tag as nearby matched blocks
                                    if(leftBox.GetComponent<Box>().Warped)
                                        leftBox.tag = rightBox.tag;
                                    else if (currentBox.GetComponent<Box>().Warped)
                                        currentBox.tag = rightBox.tag;
                                    else if (rightBox.GetComponent<Box>().Warped)
                                        rightBox.tag = currentBox.tag;
                                }
                                if (!currentMatches.Contains(rightBox))
                                {
                                    currentMatches.Add(rightBox);
                                }
                                Box curr = currentBox.GetComponent<Box>();
                                rightBox.GetComponent<Box>().isMatched = true;
                                rightBox.GetComponent<Box>().mainMatch = curr;
                                if (!currentMatches.Contains(leftBox))
                                {
                                    currentMatches.Add(leftBox);
                                }
                                leftBox.GetComponent<Box>().isMatched = true;
                                leftBox.GetComponent<Box>().mainMatch = curr;
                                if (!currentMatches.Contains(currentBox))
                                {
                                    currentMatches.Add(currentBox);
                                }
                                currentBox.GetComponent<Box>().isMatched = true;
                                if (currentBox.GetComponent<Box>().mainMatch == null)
                                    currentBox.GetComponent<Box>().mainMatch = curr;
                            }
                        }
                    }
                    // up and down
                    if (y > 0 && y < grid.hight - 1)
                    {
                        GameObject upBox = grid.allBoxes[x, y - 1];
                        GameObject downBox = grid.allBoxes[x, y + 1];
                        if (upBox && downBox)
                        {
                            bool upDownSameTag = upBox.tag == currentBox.tag && downBox.tag == currentBox.tag;
                            bool warpedBlockMatch = (upBox.tag == currentBox.tag || upBox.GetComponent<Box>().Warped)
                                && (downBox.tag == currentBox.tag || downBox.GetComponent<Box>().Warped)
                                || (currentBox.GetComponent<Box>().Warped && (upBox.tag == downBox.tag));
                            if (upDownSameTag || warpedBlockMatch)
                            {
                                if (warpedBlockMatch)
                                {
                                    //set warped block tag as nearby matched blocks
                                    if (downBox.GetComponent<Box>().Warped)
                                        downBox.tag = currentBox.tag;
                                    else if (currentBox.GetComponent<Box>().Warped)
                                        currentBox.tag = downBox.tag;
                                    else if (upBox.GetComponent<Box>().Warped)
                                        upBox.tag = currentBox.tag;
                                }
                                if (!currentMatches.Contains(upBox))
                                {
                                    currentMatches.Add(upBox);
                                }
                                Box curr = currentBox.GetComponent<Box>();
                                upBox.GetComponent<Box>().isMatched = true;
                                upBox.GetComponent<Box>().mainMatch = curr;
                                if (!currentMatches.Contains(downBox))
                                {
                                    currentMatches.Add(downBox);
                                }
                                downBox.GetComponent<Box>().mainMatch = curr;
                                downBox.GetComponent<Box>().isMatched = true;
                                if (!currentMatches.Contains(currentBox))
                                {
                                    currentMatches.Add(currentBox);
                                }
                                currentBox.GetComponent<Box>().isMatched = true;
                                if (currentBox.GetComponent<Box>().mainMatch == null)
                                    currentBox.GetComponent<Box>().mainMatch = curr;
                            }
                        }
                    }
                }
            }
        }
        ComboManager.Instance.CheckCombo();

        if (currentMatches.Count > 0 && currentMatches.Count <= 3)
        {
            grid.tempTagForTrinket = currentMatches[0].tag;
            currentMatches.Clear();
        }
    }
}