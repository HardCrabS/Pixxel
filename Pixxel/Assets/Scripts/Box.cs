using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField] GameObject columnArrow;
    [SerializeField] GameObject rowArrow;
    Vector3 firstMousePos;
    Vector3 finalMousePos;
    public Box mainMatch;
    public bool isMatched = false;
    public int row;
    public int column;
    int prevRow;
    int prevColumn;
    public int targetX;
    public int targetY;
    float finalAngle;
    float swipeResist = .5f;
    public bool isColumnBomb = false;
    public bool isRowBomb = false;
    GridA grid;
    MatchFinder matchFinder;
    public GameObject neighborBox;

    void Start()
    {
        grid = FindObjectOfType<GridA>().GetComponent<GridA>();
        matchFinder = FindObjectOfType<MatchFinder>().GetComponent<MatchFinder>();
    }

    void Update()
    {
        if (isMatched)
        {
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        }
        targetX = column;
        targetY = row;
        if (Mathf.Abs(targetX - transform.position.x) > 0.1f)
        {
            transform.position = Vector2.Lerp(transform.position, new Vector2(targetX, transform.position.y), 0.4f);
            if (grid.allBoxes[column, row] != this.gameObject)
            {
                grid.allBoxes[column, row] = this.gameObject;
            }
            matchFinder.FindAllMatches();
        }
        else
        {
            transform.position = new Vector2(targetX, transform.position.y);
            grid.allBoxes[column, row] = this.gameObject;
        }
        if (Mathf.Abs(targetY - transform.position.y) > 0.1f)
        {
            transform.position = Vector2.Lerp(transform.position, new Vector2(transform.position.x, targetY), 0.4f);
            if (grid.allBoxes[column, row] != this.gameObject)
            {
                grid.allBoxes[column, row] = this.gameObject;
            }
            matchFinder.FindAllMatches();
        }
        else
        {
            transform.position = new Vector2(transform.position.x, targetY);
            grid.allBoxes[column, row] = this.gameObject;
        }
    }

    void OnMouseDown()
    {
        if (grid.currState == GameState.move)
        {
            firstMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    void OnMouseUp()
    {
        if (grid.currState == GameState.move)
        {
            finalMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    void CalculateAngle()
    {
        if (Mathf.Abs(finalMousePos.y - firstMousePos.y) > swipeResist || Mathf.Abs(finalMousePos.x - firstMousePos.x) > swipeResist)
        {
            finalAngle = Mathf.Atan2(finalMousePos.y - firstMousePos.y, finalMousePos.x - firstMousePos.x) * 180 / Mathf.PI;
            SwipeBox();

            grid.currState = GameState.wait;
            grid.currBox = this;
        }
        else
        {
            grid.currState = GameState.move;
        }
    }

    void SwipeBox()
    {
        if (finalAngle <= 45 && finalAngle >= -45 && column < grid.width - 1) //right swipe
        {
            neighborBox = grid.allBoxes[column + 1, row];
            if (neighborBox != null)
            {
                neighborBox.GetComponent<Box>().column -= 1;
                prevColumn = column;
                prevRow = row;
                column += 1;
            }
            else
            {
                grid.currState = GameState.move;
            }
        }
        else if (finalAngle >= -135 && finalAngle <= -45 && row > 0) //down swipe
        {
            neighborBox = grid.allBoxes[column, row - 1];
            if (neighborBox != null)
            {
                neighborBox.GetComponent<Box>().row += 1;
                prevColumn = column;
                prevRow = row;
                row -= 1;
            }
            else
            {
                grid.currState = GameState.move;
            }
        }
        else if ((finalAngle >= 135 || finalAngle <= -135) && column > 0) //left swipe
        {
            neighborBox = grid.allBoxes[column - 1, row];
            if (neighborBox != null)
            {
                neighborBox.GetComponent<Box>().column += 1;
                prevColumn = column;
                prevRow = row;
                column -= 1;
            }
            else
            {
                grid.currState = GameState.move;
            }
        }
        else if (finalAngle <= 135 && finalAngle >= 45 && row < grid.hight - 1) //up swipe
        {
            neighborBox = grid.allBoxes[column, row + 1];
            if (neighborBox != null)
            {
                neighborBox.GetComponent<Box>().row -= 1;
                prevColumn = column;
                prevRow = row;
                row += 1;
            }
            else
            {
                grid.currState = GameState.move;
            }
        }

        StartCoroutine(ReturnBoxes());
    }

    IEnumerator ReturnBoxes()
    {
        yield return new WaitForSeconds(0.3f);
        if (neighborBox != null)
        {
            if (!isMatched && !neighborBox.GetComponent<Box>().isMatched)
            {
                neighborBox.GetComponent<Box>().row = row;
                neighborBox.GetComponent<Box>().column = column;
                row = prevRow;
                column = prevColumn;
                yield return new WaitForSeconds(.5f);
                grid.currBox = null;
                grid.currState = GameState.move;
            }
            else
            {
                grid.DestroyAllMatches();
            }
            neighborBox = null;
        }
    }
    void ChangeBlockSprite(string tag)
    {
        for (int i = 0; i < grid.boxPrefabs.Length; i++)
        {
            if (grid.boxPrefabs[i].CompareTag(tag))
            {
                GetComponent<SpriteRenderer>().sprite = grid.boxPrefabs[i].GetComponent<SpriteRenderer>().sprite;
                gameObject.tag = tag;
                break;
            }
        }
    }
}
