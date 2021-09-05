using System.Collections;
using UnityEngine;
using DG.Tweening;

public enum BoxState
{
    Normal,
    FiredUp,
    Warped,
    Golden
}

public class Box : MonoBehaviour
{
    Vector3 firstMousePos;
    Vector3 finalMousePos;
    public Box mainMatch;
    public bool isMatched { get; private set; }
    public bool isMatchable { get; private set; }
    public int row { get; private set; }
    public int column { get; private set; }
    public int targetX;
    public string boxName;
    public int targetY;

    int prevColumn;
    int prevRow;
    float swipeResist = .5f;

    public BoxState currState = BoxState.Normal;

    public bool Mooving { get; set; } = false;

    GridA grid;
    MatchFinder matchFinder;
    public GameObject neighborBox;
    Coroutine movingCo;

    public delegate void OnClick(int x, int y);
    public event OnClick blockClicked;

    public delegate void OnRelease();
    public event OnRelease blockReleased;

    void Start()
    {
        isMatchable = true;
        grid = GridA.Instance;
        matchFinder = MatchFinder.Instance;
        blockClicked += grid.CrosshairToBlock;
        blockReleased += grid.CrosshairRelease;
    }

    public void UpdatePos(int row = -1, int column = -1, bool moveBoxInPosition = false)
    {
        if (row != -1)
            this.row = row;
        if (column != -1)
        {
            this.column = column;
        }
        if (moveBoxInPosition)
        {
            if(movingCo != null)//box is already moving
            {
                StopCoroutine(movingCo);//stop current movemnt
            }
            movingCo = StartCoroutine(MoveBoxInPosition());
        }
    }
    IEnumerator MoveBoxInPosition()
    {
        GridA.Instance.allBoxes[row, column] = this.gameObject;
        GridA.Instance.bombTiles[row, column] = gameObject.GetComponent<BombTile>();

        //move toward target
        Vector2 targetPosition = new Vector2(row, column);
        yield return transform.DOLocalMove(targetPosition, 0.15f).WaitForCompletion();

        matchFinder.FindAllMatches();
        ReturnBoxes();
    }
    public void SetMatched(bool matched)
    {
        if (!isMatchable) return;

        isMatched = matched;
        if (isMatched)
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer)
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        }
    }
    public void SetMatchable(bool matchable)
    {
        this.isMatchable = matchable;
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
        blockReleased?.Invoke();
    }

    void CalculateAngle()
    {
        if (Mathf.Abs(finalMousePos.y - firstMousePos.y) > swipeResist || Mathf.Abs(finalMousePos.x - firstMousePos.x) > swipeResist)
        {
            //grid.currState = GameState.wait;
            float finalAngle = Mathf.Atan2(finalMousePos.y - firstMousePos.y, finalMousePos.x - firstMousePos.x) * 180 / Mathf.PI;

            Vector2Int swipeDirection = GetSwipeDirection(finalAngle);
            if (swipeDirection != Vector2Int.zero)
                SwipeBoxesActual(swipeDirection);

            grid.currBox = this;
        }
        else
        {
            //grid.currState = GameState.move;
        }
    }
    void SwipeBoxesActual(Vector2Int direction)
    {
        GridA.Instance.SwipeBoxesSFX();
        neighborBox = grid.allBoxes[row + direction.x, column + direction.y];
        prevRow = row;
        prevColumn = column;

        //if (grid.lockedTiles[row, column] == null
        //   && grid.lockedTiles[row + direction.x, column + direction.y] == null)
        if (neighborBox != null)
        {
            var neighborBoxComp = neighborBox.GetComponent<Box>();
            UpdatePos(neighborBoxComp.row, neighborBoxComp.column, true);
            neighborBoxComp.UpdatePos(prevRow, prevColumn, true);
        }
        else
        {
            //grid.currState = GameState.move;
        }
    }

    Vector2Int GetSwipeDirection(float finalAngle)
    {
        if (finalAngle <= 45 && finalAngle >= -45 && row < grid.width - 1) //right swipe
        {
            return Vector2Int.right;
        }
        else if (finalAngle >= -135 && finalAngle <= -45 && column > 0) //down swipe
        {
            return Vector2Int.down;
        }
        else if ((finalAngle >= 135 || finalAngle <= -135) && row > 0) //left swipe
        {
            return Vector2Int.left;
        }
        else if (finalAngle <= 135 && finalAngle >= 45 && column < grid.hight - 1) //up swipe
        {
            return Vector2Int.up;
        }
       /* else
            grid.currState = GameState.move;*/
        return Vector2Int.zero;
    }

    void ReturnBoxes()
    {
        if (neighborBox != null)
        {
            if (!isMatched && !neighborBox.GetComponent<Box>().isMatched)
            {
                GridA.Instance.ReturnBoxesSFX();
                neighborBox.GetComponent<Box>().UpdatePos(row, column, true);
                UpdatePos(prevRow, prevColumn, true);

                grid.currBox = null;
                //grid.currState = GameState.move;
            }
            else
            {
                grid.DestroyAllMatches();

                EndGameManager.Instance.CallOnMatchDelegate();
            }
            neighborBox = null;
        }
    }
    public void MoveBoxDown()
    {
        StopAllCoroutines();
        transform.DOLocalMove(transform.localPosition + Vector3.down * 30, 1f);
    }
    public void ChangeBoxPosition(int _row, int _column)
    {
        prevRow = row;
        prevColumn = column;
        UpdatePos(_row, _column);
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