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
        for (int x = 0; x < grid.width; x++)
        {
            for (int y = 0; y < grid.hight; y++)
            {
                GameObject currentBox = grid.allBoxes[x, y];
                if (currentBox == null) continue;

                // left and right
                if (x > 0 && x < grid.width - 1)
                {
                    CheckNearbyBlocks(x, y, Vector2Int.right);
                }
                // up and down
                if (y > 0 && y < grid.hight - 1)
                {
                    CheckNearbyBlocks(x, y, Vector2Int.up);
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

    void CheckNearbyBlocks(int x, int y, Vector2Int dirOfBlocks)
    {
        GameObject currentBox = grid.allBoxes[x, y];
        GameObject rightOrUpBox = grid.allBoxes[x + dirOfBlocks.x, y + dirOfBlocks.y];
        GameObject leftOrDownBox = grid.allBoxes[x - dirOfBlocks.x, y - dirOfBlocks.y];
        if (leftOrDownBox && rightOrUpBox)
        {
            bool leftDownSameTag = leftOrDownBox.CompareTag(currentBox.tag);
            bool leftDownWarped = leftOrDownBox.GetComponent<Box>().Warped;

            bool rightUpSameTag = rightOrUpBox.CompareTag(currentBox.tag);
            bool rightUpWarped = rightOrUpBox.GetComponent<Box>().Warped;

            bool leftDownSameAsRightUp = leftOrDownBox.CompareTag(rightOrUpBox.tag);
            bool currWarped = currentBox.GetComponent<Box>().Warped;

            bool nearbySameTag = leftDownSameTag && rightUpSameTag;
            bool warpedBlockMatch = (leftDownSameTag || leftDownWarped) && (rightUpSameTag || rightUpWarped)
                || (currWarped && leftDownSameAsRightUp);
            if (nearbySameTag || warpedBlockMatch)
            {
                if (warpedBlockMatch)
                {
                    //set warped block tag as nearby matched blocks
                    if (leftOrDownBox.GetComponent<Box>().Warped)
                        leftOrDownBox.tag = rightOrUpBox.tag;
                    else if (currentBox.GetComponent<Box>().Warped)
                        currentBox.tag = rightOrUpBox.tag;
                    else if (rightOrUpBox.GetComponent<Box>().Warped)
                        rightOrUpBox.tag = currentBox.tag;
                }
                Box curr = currentBox.GetComponent<Box>();

                if (!currentMatches.Contains(rightOrUpBox))
                {
                    currentMatches.Add(rightOrUpBox);
                }
                rightOrUpBox.GetComponent<Box>().SetMatched(true);
                rightOrUpBox.GetComponent<Box>().mainMatch = curr;

                if (!currentMatches.Contains(leftOrDownBox))
                {
                    currentMatches.Add(leftOrDownBox);
                }
                leftOrDownBox.GetComponent<Box>().SetMatched(true);
                leftOrDownBox.GetComponent<Box>().mainMatch = curr;

                if (!currentMatches.Contains(currentBox))
                {
                    currentMatches.Add(currentBox);
                }
                curr.SetMatched(true);
                if (curr.mainMatch == null)
                    curr.mainMatch = curr;
            }
        }
    }
}