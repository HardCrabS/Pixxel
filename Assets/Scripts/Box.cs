using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    Vector3 firstMousePos;
    Vector3 finalMousePos;
    public Box mainMatch;
    public bool isMatched = false;
    public int row;
    public int column;
    int prevColumn;
    int prevRow;
    public int targetX;
    public int targetY;
    float finalAngle;
    float swipeResist = .5f;
    public string boxName;

    GridA grid;
    MatchFinder matchFinder;
    public GameObject neighborBox;

    public delegate void OnClick(int x, int y);
    public OnClick blockClicked;

    void Start()
    {
        grid = GridA.Instance;
        matchFinder = MatchFinder.Instance;
        blockClicked += grid.CrosshairToBlock;
        StartCoroutine(MainBlockLogic());
    }

    IEnumerator MainBlockLogic()
    {
        while (true)
        {
            if (isMatched)
            {
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            }
            targetX = row;
            targetY = column;
            if (Mathf.Abs(targetX - transform.localPosition.x) > 0.1f)
            {
                transform.localPosition = Vector2.Lerp(transform.localPosition, new Vector2(targetX, transform.localPosition.y), 0.4f);
                if (grid.allBoxes[row, column] != this.gameObject)
                {
                    grid.allBoxes[row, column] = this.gameObject;
                }
                matchFinder.FindAllMatches();
            }
            else
            {
                transform.localPosition = new Vector2(targetX, transform.localPosition.y);
                grid.allBoxes[row, column] = this.gameObject;
            }
            if (Mathf.Abs(targetY - transform.localPosition.y) > 0.1f)
            {
                transform.localPosition = Vector2.Lerp(transform.localPosition, new Vector2(transform.localPosition.x, targetY), 0.4f);
                if (grid.allBoxes[row, column] != this.gameObject)
                {
                    grid.allBoxes[row, column] = this.gameObject;
                }
                matchFinder.FindAllMatches();
            }
            else
            {
                transform.localPosition = new Vector2(transform.localPosition.x, targetY);
                grid.allBoxes[row, column] = this.gameObject;
            }

            yield return null;
        }
    }

    void OnMouseDown()
    {
        if (grid.currState == GameState.move)
        {
            firstMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        blockClicked?.Invoke(row, column);
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
            grid.currState = GameState.wait;
            finalAngle = Mathf.Atan2(finalMousePos.y - firstMousePos.y, finalMousePos.x - firstMousePos.x) * 180 / Mathf.PI;
            SwipeBox();

            grid.currBox = this;
        }
        else
        {
            grid.currState = GameState.move;
        }
    }

    void SwipeBoxesActual(Vector2 direction)
    {
        neighborBox = grid.allBoxes[row + (int)direction.x, column + (int)direction.y];
        prevRow = row;
        prevColumn = column;

        if (grid.lockedTiles[row, column] == null
            && grid.lockedTiles[row + (int)direction.x, column + (int)direction.y] == null)
        {
            if (neighborBox != null)
            {
                neighborBox.GetComponent<Box>().row += -1 * (int)direction.x;
                neighborBox.GetComponent<Box>().column += -1 * (int)direction.y;
                row += (int)direction.x;
                column += (int)direction.y;
                StartCoroutine(ReturnBoxes());
            }
            else
            {
                grid.currState = GameState.move;
            }
        }
        else
        {
            grid.currState = GameState.move;
        }
    }

    void SwipeBox()
    {
        GridA.Instance.SwipeBoxesSFX();
        if (finalAngle <= 45 && finalAngle >= -45 && row < grid.width - 1) //right swipe
        {
            SwipeBoxesActual(Vector2.right);
        }
        else if (finalAngle >= -135 && finalAngle <= -45 && column > 0) //down swipe
        {
            SwipeBoxesActual(Vector2.down);
        }
        else if ((finalAngle >= 135 || finalAngle <= -135) && row > 0) //left swipe
        {
            SwipeBoxesActual(Vector2.left);
        }
        else if (finalAngle <= 135 && finalAngle >= 45 && column < grid.hight - 1) //up swipe
        {
            SwipeBoxesActual(Vector2.up);
        }
        else
            grid.currState = GameState.move;
    }

    IEnumerator ReturnBoxes()
    {
        yield return new WaitForSeconds(0.3f);
        if (neighborBox != null)
        {
            if (!isMatched && !neighborBox.GetComponent<Box>().isMatched)
            {
                GridA.Instance.ReturnBoxesSFX();
                neighborBox.GetComponent<Box>().column = column;
                neighborBox.GetComponent<Box>().row = row;
                column = prevColumn;
                row = prevRow;
                yield return new WaitForSeconds(.5f);
                grid.currBox = null;
                grid.currState = GameState.move;
            }
            else
            {
                if (GetComponent<BombTile>())
                {
                    grid.bombTiles[row, column] = grid.bombTiles[prevRow, prevColumn];
                    grid.bombTiles[prevRow, prevColumn] = null;
                }
                else if (neighborBox.GetComponent<BombTile>())
                {
                    grid.bombTiles[prevRow, prevColumn] = grid.bombTiles[row, column];
                    grid.bombTiles[row, column] = null;
                }
                grid.DestroyAllMatches();

                EndGameManager.Instance.CallOnMatchDelegate();
            }
            neighborBox = null;
        }
    }
    public void MoveBoxDown()
    {
        StopAllCoroutines();
        StartCoroutine(MoveDown());
    }
    IEnumerator MoveDown()
    {
        float smoothTime = 0.3f;
        Vector3 velocity = Vector3.zero;
        Vector3 targetPos = new Vector3(transform.position.x, transform.position.y - 30, transform.position.z);

        while (transform.position != targetPos)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
            yield return null;
        }
    }

    //switcheroo boost
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