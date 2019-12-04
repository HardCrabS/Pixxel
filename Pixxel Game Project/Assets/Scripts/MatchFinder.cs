﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MatchFinder : MonoBehaviour
{
    GridA grid;
    public List<GameObject> currentMatches = new List<GameObject>();
    void Start()
    {
        grid = FindObjectOfType<GridA>();
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
                            if (leftBox.tag == currentBox.tag && rightBox.tag == currentBox.tag)
                            {
                                if (!currentMatches.Contains(rightBox))
                                {
                                    currentMatches.Add(rightBox);
                                }
                                rightBox.GetComponent<Box>().isMatched = true;
                                if (!currentMatches.Contains(leftBox))
                                {
                                    currentMatches.Add(leftBox);
                                }
                                leftBox.GetComponent<Box>().isMatched = true;
                                if (!currentMatches.Contains(currentBox))
                                {
                                    currentMatches.Add(currentBox);
                                }
                                currentBox.GetComponent<Box>().isMatched = true;
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
                            if (upBox.tag == currentBox.tag && downBox.tag == currentBox.tag)
                            {
                                if (!currentMatches.Contains(upBox))
                                {
                                    currentMatches.Add(upBox);
                                }
                                upBox.GetComponent<Box>().isMatched = true;
                                if (!currentMatches.Contains(downBox))
                                {
                                    currentMatches.Add(downBox);
                                }
                                downBox.GetComponent<Box>().isMatched = true;
                                if (!currentMatches.Contains(currentBox))
                                {
                                    currentMatches.Add(currentBox);
                                }
                                currentBox.GetComponent<Box>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }

        if(currentMatches.Count > 0 && currentMatches.Count <= 3)
        {
            grid.tempTagForTrinket = currentMatches[0].tag;
            currentMatches.Clear();
        }
    }
}
