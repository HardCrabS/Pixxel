using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    move,
    wait
}

public class GridA : MonoBehaviour
{
    public GameState currState = GameState.move;
    [Header("Prefabs")]
    public GameObject[] boxPrefabs;

    [Header("Add for match")]
    [SerializeField] int pointsToAddperBox = 10;
    [SerializeField] float pointsXPforLevel = 1;

    [Header("Grid Settings")]
    public GameObject[,] allBoxes;
    public int width = 8;
    public int hight = 8;
    public int offset;

    [SerializeField] Transform parent;
    public Box currBox;

    private const float aspectRatioMultiplier = 9.0f / 16 * 7.5f;
    bool[,] blankSpaces;
    MatchFinder matchFinder;
    Score score;
    LevelSlider levelSlider;
    CoinsDisplay coinsDisplay;
    public string tempTagForTrinket;
    Vector2Int[] directions = new Vector2Int[]
    {
        Vector2Int.up,
        Vector2Int.one,
        Vector2Int.right,
        new Vector2Int(1, -1),
        Vector2Int.down,
        new Vector2Int(-1, -1),
        Vector2Int.left,
        new Vector2Int(-1, 1)
    };
    BackgroundActivity b;
    void Start()
    {
        Camera.main.orthographicSize = aspectRatioMultiplier / Camera.main.aspect;
        blankSpaces = new bool[width, hight];
        currState = GameState.move;
        coinsDisplay = FindObjectOfType<CoinsDisplay>();
        levelSlider = FindObjectOfType<LevelSlider>();
        matchFinder = FindObjectOfType<MatchFinder>();
        score = FindObjectOfType<Score>();
        CreateGrid();
        b = FindObjectOfType<BackgroundActivity>();
    }

    void CreateGrid()
    {
        allBoxes = new GameObject[width, hight];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < hight; y++)
            {
                if (!blankSpaces[x, y])
                {
                    Vector2 tempPos = new Vector3(x, y + offset);
                    int randIndex = Random.Range(0, boxPrefabs.Length);
                    while (MatchesAt(x, y, boxPrefabs[randIndex]))
                    {
                        randIndex = Random.Range(0, boxPrefabs.Length);
                    }
                    GameObject go = Instantiate(boxPrefabs[randIndex], tempPos, transform.rotation);
                    go.GetComponent<Box>().row = y;
                    go.GetComponent<Box>().column = x;
                    allBoxes[x, y] = go;
                    go.transform.SetParent(parent);
                    go.name = x + "," + y;
                }
            }
        }
    }

    bool MatchesAt(int column, int row, GameObject box)
    {
        if (column > 1 && row > 1)
        {
            if (allBoxes[column - 1, row] && allBoxes[column - 2, row])
            {
                if (allBoxes[column - 1, row].CompareTag(box.tag) && allBoxes[column - 2, row].CompareTag(box.tag))
                {
                    return true;
                }
            }
            if (allBoxes[column, row - 1] && allBoxes[column, row - 2])
            {
                if (allBoxes[column, row - 1].CompareTag(box.tag) && allBoxes[column, row - 2].CompareTag(box.tag))
                {
                    return true;
                }
            }
        }
        else if (column <= 1 && row > 1)
        {
            if (allBoxes[column, row - 1] && allBoxes[column, row - 2])
            {
                if (allBoxes[column, row - 1].CompareTag(box.tag) && allBoxes[column, row - 2].CompareTag(box.tag))
                {
                    return true;
                }
            }
        }
        if (column > 1 && row <= 1)
        {
            if (allBoxes[column - 1, row] && allBoxes[column - 2, row])
            {
                if (allBoxes[column - 1, row].CompareTag(box.tag) && allBoxes[column - 2, row].CompareTag(box.tag))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void DestroyMatched(int column, int row)
    {
        if (allBoxes[column, row].GetComponent<Box>() && allBoxes[column, row].GetComponent<Box>().isMatched)
        {
            //if (allBoxes[column, row].GetComponent<Box>().mainMatch == allBoxes[column, row].GetComponent<Box>())
            //    b.SpawnBackgroundActivity();
            CheckBomb();
            AddPointsForMatchedBlock();
            Destroy(allBoxes[column, row]);
            matchFinder.currentMatches.Remove(allBoxes[column, row]);
            allBoxes[column, row] = null;
            currBox = null;
        }
    }

    void CheckBomb()
    {
        if (matchFinder.currentMatches.Count == 4 || matchFinder.currentMatches.Count == 7)
        {
            if (currBox != null)
            {
                if (currBox.isMatched)
                {
                    currBox.isMatched = false;
                    currBox.GetComponent<SpriteRenderer>().color = Color.red;
                    StartCoroutine(CrossBomb(currBox));
                }
                else if (currBox.neighborBox != null && currBox.neighborBox.GetComponent<Box>().isMatched)
                {
                    currBox.neighborBox.GetComponent<Box>().isMatched = false;
                    currBox.neighborBox.GetComponent<SpriteRenderer>().color = Color.red;
                    StartCoroutine(CrossBomb(currBox.neighborBox.GetComponent<Box>()));
                }
            }
        }
        else if (matchFinder.currentMatches.Count == 5)
        {
            if (currBox != null)
            {
                if (currBox.isMatched)
                {
                    currBox.isMatched = false;
                    currBox.GetComponent<SpriteRenderer>().color = Color.blue;
                    StartCoroutine(DestroyAllSameColor(currBox.tag));
                }
                else if (currBox.neighborBox != null && currBox.neighborBox.GetComponent<Box>().isMatched)
                {
                    currBox.neighborBox.GetComponent<Box>().isMatched = false;
                    currBox.neighborBox.GetComponent<SpriteRenderer>().color = Color.red;
                    StartCoroutine(DestroyAllSameColor(currBox.neighborBox.tag));
                }
            }
        }
    }

    private IEnumerator CrossBomb(Box box)
    {
        yield return new WaitForSeconds(2f);
        foreach (Vector2Int dir in directions)
        {
            if (box.column + dir.x >= 0 && box.column + dir.x < width
                && box.row + dir.y >= 0 && box.row + dir.y < hight)
            {
                Destroy(allBoxes[box.column + dir.x, box.row + dir.y]);
                allBoxes[box.column + dir.x, box.row + dir.y] = null;
                AddXPandScorePoints();
            }
        }
        if (box.gameObject != null)
        {
            Destroy(box.gameObject);
        }
        allBoxes[box.column, box.row] = null;
        StartCoroutine(MoveBoxesDown());
    }

    public IEnumerator DestroyAllSameColor(string boxTag)
    {
        yield return new WaitForSeconds(2f);
        foreach (GameObject go in allBoxes)
        {
            if (go != null && go.CompareTag(boxTag))
            {
                yield return new WaitForSeconds(.1f);
                Destroy(go);
                AddXPandScorePoints();
            }
        }
        StartCoroutine(MoveBoxesDown());
    }

    public void FlameThrower(int j, int i)
    {
        if (allBoxes[j, i] != null)
        {
            Destroy(allBoxes[j, i]);
        }
    }

    void AddPointsForMatchedBlock()
    {
        TrinketManager trink = FindObjectOfType<TrinketManager>();
        if (matchFinder.currentMatches.Count == 0)
        {
            AddXPandScorePoints();
            if (tempTagForTrinket != null && tempTagForTrinket == trink.tagToDestroy.ToString())
            {
                for (int i = 0; i < 3; i++)
                {
                    if (trink.boxesToDestroy >= 0)
                        trink.DestroyBoxAmount();
                }
                tempTagForTrinket = null;
            }
        }
        else
        {
            AddXPandScorePoints();

            if (trink.boxesToDestroy >= 0 && matchFinder.currentMatches[0].CompareTag(trink.tagToDestroy.ToString()))
            {
                trink.DestroyBoxAmount();
            }
        }
    }
    void AddXPandScorePoints()
    {
        score.AddPoints(pointsToAddperBox);
        levelSlider.AddXPtoLevel(pointsXPforLevel);
        coinsDisplay.RandomizeCoin();
    }

    public void SetXPpointsPerBoxByProcent(float procent)
    {
        pointsXPforLevel *= procent;
    }

    public void DestroyAllMatches()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < hight; y++)
            {
                if (allBoxes[x, y] != null)
                {
                    DestroyMatched(x, y);
                }
            }
        }
        StartCoroutine(MoveBoxesDown());
    }

    public IEnumerator MoveBoxesDown()
    {
        int nullCount = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < hight; y++)
            {
                if (allBoxes[x, y] == null && !blankSpaces[x, y])
                {
                    nullCount++;
                }
                else if (nullCount > 0 && allBoxes[x, y])
                {
                    allBoxes[x, y].GetComponent<Box>().row -= nullCount;
                    allBoxes[x, y] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoard());
    }

    private void RespawnBoxes()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < hight; y++)
            {
                if (allBoxes[x, y] == null && !blankSpaces[x, y])
                {
                    Vector2 tempPos = new Vector2(x, y + offset);
                    int randIndex = Random.Range(0, boxPrefabs.Length);
                    GameObject box = Instantiate(boxPrefabs[randIndex], tempPos, transform.rotation);
                    box.GetComponent<Box>().column = x;
                    box.GetComponent<Box>().row = y;
                    box.transform.SetParent(parent);
                    allBoxes[x, y] = box;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < hight; y++)
            {
                if (allBoxes[x, y] != null && allBoxes[x, y].GetComponent<Box>().isMatched)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoard()
    {
        RespawnBoxes();
        yield return new WaitForSeconds(.5f);

        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyAllMatches();
        }
        matchFinder.currentMatches.Clear();
        yield return new WaitForSeconds(.5f);
        if (IsDeadlocked())
        {
            Debug.Log("Is deadlocked");
        }
        currState = GameState.move;
    }

    private void SwitchPieces(int column, int row, Vector2 direction)
    {
        GameObject holder = allBoxes[column + (int)direction.x, row + (int)direction.y];
        allBoxes[column + (int)direction.x, row + (int)direction.y] = allBoxes[column, row];
        allBoxes[column, row] = holder;
    }

    public bool CheckForMatches()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < hight; y++)
            {
                if (allBoxes[x, y] != null)
                {
                    if (x < width - 2)
                    {
                        if (allBoxes[x + 1, y] != null && allBoxes[x + 2, y] != null)
                        {
                            if (allBoxes[x + 1, y].CompareTag(allBoxes[x, y].tag)
                                && allBoxes[x + 2, y].CompareTag(allBoxes[x, y].tag))
                            {
                                return true;
                            }
                        }
                    }

                    if (y < hight - 2)
                    {
                        if (allBoxes[x, y + 1] != null && allBoxes[x, y + 2] != null)
                        {
                            if (allBoxes[x, y + 1].CompareTag(allBoxes[x, y].tag)
                                && allBoxes[x, y + 2].CompareTag(allBoxes[x, y].tag))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }
    public bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchPieces(column, row, direction);
        if (CheckForMatches())
        {
            SwitchPieces(column, row, direction);
            return true;
        }
        SwitchPieces(column, row, direction);
        return false;
    }

    private bool IsDeadlocked()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < hight; y++)
            {
                if (allBoxes[x, y] != null)
                {
                    if (x < width - 1)
                    {
                        if (SwitchAndCheck(x, y, Vector2.right))
                        {
                            return false;
                        }
                    }
                    if (y < hight - 1)
                    {
                        if (SwitchAndCheck(x, y, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }
}
