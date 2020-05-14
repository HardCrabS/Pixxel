using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BlockTags
{
    Red,
    Blue,
    Black,
    Yellow,
    Green,
    Pink
}
public enum GameState
{
    move,
    wait
}
public enum TileKind
{
    Breakable,
    Locked,
    Blank,
    Normal,
    Bomb
}

[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileKind tileKind;
}

public class GridA : MonoBehaviour
{
    [Header("Scriptable objects")]
    public WorldTemplate world;
    public int level;

    [Header("Prefabs")]
    public GameObject[] boxPrefabs;
    [SerializeField] GameObject blockDestroyParticle;
    [SerializeField] GameObject breakableTilePrefab;
    [SerializeField] GameObject bombTilePrefab;
    [SerializeField] GameObject lockTilePrefab;
    [SerializeField] GameObject fire;
    [SerializeField] GameObject smoke;

    [Header("Add for match")]
    [SerializeField] int pointsToAddperBox = 10;
    [SerializeField] float pointsXPforLevel = 1;

    [Header("Grid Settings")]
    [SerializeField] TileType[] boardLayout;
    public GameObject[,] allBoxes;
    public int width = 8;
    public int hight = 8;
    public int offset;
    public GameState currState = GameState.move;

    [SerializeField] Transform parent;
    public Box currBox;

    private const float aspectRatioMultiplier = 9.0f / 16 * 7.5f;
    public bool[,] blankSpaces;
    private BackgroundTile[,] breakableTiles;
    public BackgroundTile[,] lockedTiles;
    public BombTile[,] bombTiles;
    MatchFinder matchFinder;
    Score score;
    LevelSlider levelSlider;
    CoinsDisplay coinsDisplay;
    GoalManager goalManager;
    EndGameManager endGameManager;
    LevelSettingsKeeper levelSettings;
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
    public delegate void MyDelegate(int column, int row);
    public event MyDelegate onMatchedBlock;

    void Awake()
    {
        Level levelTemplate = FindObjectOfType<Level>();
        levelSettings = FindObjectOfType<LevelSettingsKeeper>();
        levelTemplate.LoadLevel();
        if (levelSettings != null)
        {
            if (levelSettings.levelTemplate != null)
            {
                width = levelSettings.levelTemplate.width;
                hight = levelSettings.levelTemplate.hight;
                offset = levelSettings.levelTemplate.offset;
                boardLayout = levelSettings.levelTemplate.boardLayout;
            }
        }
    }

    void Start()
    {
        Camera.main.orthographicSize = aspectRatioMultiplier / Camera.main.aspect;
        blankSpaces = new bool[width, hight];
        breakableTiles = new BackgroundTile[width, hight];
        lockedTiles = new BackgroundTile[width, hight];
        bombTiles = new BombTile[width, hight];
        currState = GameState.move;

        int startParentX = (8 - width) / 2;
        int startParentY = (8 - hight) / 2;
        parent.position = new Vector2(startParentX, startParentY);

        goalManager = FindObjectOfType<GoalManager>();
        coinsDisplay = FindObjectOfType<CoinsDisplay>();
        levelSlider = FindObjectOfType<LevelSlider>();
        matchFinder = FindObjectOfType<MatchFinder>();
        endGameManager = FindObjectOfType<EndGameManager>();
        score = FindObjectOfType<Score>();
        CreateGrid();
    }

    void GenerateBlankSpaces()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Blank)
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }

    void GenerateBreakableTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Breakable)
            {
                GameObject breakable = Instantiate(breakableTilePrefab,
                    parent.position + new Vector3(boardLayout[i].x, boardLayout[i].y), transform.rotation, parent);
                breakableTiles[boardLayout[i].x, boardLayout[i].y] = breakable.GetComponent<BackgroundTile>();
            }
        }
    }

    void GenerateLockedTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Locked)
            {
                GameObject locked = Instantiate(lockTilePrefab,
                    parent.position + new Vector3(boardLayout[i].x, boardLayout[i].y), transform.rotation, parent);
                lockedTiles[boardLayout[i].x, boardLayout[i].y] = locked.GetComponent<BackgroundTile>();
            }
        }
    }
    void GenerateBombTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Bomb)
            {
                Vector2 tempPos = new Vector3(boardLayout[i].x, boardLayout[i].y);
                GameObject bomb = Instantiate(bombTilePrefab, parent.position + new Vector3(boardLayout[i].x, boardLayout[i].y), transform.rotation, parent);
                bombTiles[boardLayout[i].x, boardLayout[i].y] = bomb.GetComponent<BombTile>();
                bomb.GetComponent<Box>().row = (int)tempPos.y;
                bomb.GetComponent<Box>().column = (int)tempPos.x;
                allBoxes[(int)tempPos.x, (int)tempPos.y] = bomb;
                bomb.name = "Bomb";
            }
        }
    }
    void CreateGrid()
    {
        GenerateBlankSpaces();
        GenerateBreakableTiles();
        GenerateLockedTiles();
        allBoxes = new GameObject[width, hight];
        GenerateBombTiles();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < hight; y++)
            {
                if (!blankSpaces[x, y] && !bombTiles[x, y])
                {
                    Vector2 tempPos = new Vector3(x, y + offset);
                    int randIndex = Random.Range(0, boxPrefabs.Length);
                    while (MatchesAt(x, y, boxPrefabs[randIndex]))
                    {
                        randIndex = Random.Range(0, boxPrefabs.Length);
                    }
                    GameObject go = Instantiate(boxPrefabs[randIndex], tempPos, transform.rotation, parent);
                    go.GetComponent<Box>().row = y;
                    go.GetComponent<Box>().column = x;
                    allBoxes[x, y] = go;
                    go.name = x + "," + y;
                }
            }
        }
    }

    public void SetBlankSpace(int x, int y, bool blank)
    {
        blankSpaces[x, y] = blank;
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
            if (onMatchedBlock != null)
            {
                onMatchedBlock(column, row);
                return;
            }
            if (goalManager != null)
            {
                goalManager.CompareGoal(allBoxes[column, row].tag);
                goalManager.UpdateGoals();
            }
            if (breakableTiles[column, row] != null)
            {
                breakableTiles[column, row].TakeDamage();
            }
            if (lockedTiles[column, row] != null)
            {
                lockedTiles[column, row].TakeDamage();
            }
            DestroyBombTile(column, row);

            GameObject particle = Instantiate(blockDestroyParticle,
                allBoxes[column, row].transform.localPosition + parent.position, transform.rotation);
            Destroy(particle, 0.5f);
            CheckBomb();
            AddPointsForMatchedBlock();
            Destroy(allBoxes[column, row]);
            matchFinder.currentMatches.Remove(allBoxes[column, row]);
            allBoxes[column, row] = null;
            currBox = null;
        }
    }

    void DestroyBombTile(int column, int row)
    {
        if (column > 0)
        {
            if (bombTiles[column - 1, row])
            {
                bombTiles[column - 1, row].DeleteBombByMatch();
                bombTiles[column - 1, row] = null;
                allBoxes[column - 1, row] = null;
            }
        }
        if (column < width - 1)
        {
            if (bombTiles[column + 1, row])
            {
                bombTiles[column + 1, row].DeleteBombByMatch();
                bombTiles[column + 1, row] = null;
                allBoxes[column + 1, row] = null;
            } 
        }
        if (row > 0)
        {
            if (bombTiles[column, row - 1])
            {
                bombTiles[column, row - 1].DeleteBombByMatch();
                bombTiles[column, row - 1] = null;
                allBoxes[column, row - 1] = null;
            }
        }
        if (row < hight - 1)
        {
            if (bombTiles[column, row + 1])
            {
                bombTiles[column, row + 1].DeleteBombByMatch();
                bombTiles[column, row + 1] = null;
                allBoxes[column, row + 1] = null;
            }
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
                    StartCoroutine(FiredUpBlock(currBox));
                }
                else if (currBox.neighborBox != null && currBox.neighborBox.GetComponent<Box>().isMatched)
                {
                    currBox.neighborBox.GetComponent<Box>().isMatched = false;
                    currBox.neighborBox.GetComponent<SpriteRenderer>().color = Color.red;
                    StartCoroutine(FiredUpBlock(currBox.neighborBox.GetComponent<Box>()));
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

    public IEnumerator FiredUpBlock(Box box)
    {
        GameObject smokeClone = Instantiate(smoke, box.gameObject.transform.position, smoke.transform.rotation, box.transform);
        GameObject fireClone = Instantiate(fire, box.gameObject.transform.position, transform.rotation, box.transform);
        box.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(2f);
        foreach (Vector2Int dir in directions)
        {
            if (box.column + dir.x >= 0 && box.column + dir.x < width
                && box.row + dir.y >= 0 && box.row + dir.y < hight)
            {
                GameObject particle = Instantiate(blockDestroyParticle,
                    box.transform.localPosition + parent.position + new Vector3(dir.x, dir.y), transform.rotation);
                Destroy(particle, 0.5f);
                Destroy(allBoxes[box.column + dir.x, box.row + dir.y]);
                allBoxes[box.column + dir.x, box.row + dir.y] = null;
                if (bombTiles[box.column + dir.x, box.row + dir.y])
                {
                    bombTiles[box.column + dir.x, box.row + dir.y].DeleteBombByMatch();
                    bombTiles[box.column + dir.x, box.row + dir.y] = null;
                }
                    AddXPandScorePoints();
            }
        }
        if (box != null)
        {
            GameObject particle = Instantiate(blockDestroyParticle,
                box.transform.localPosition + parent.position, transform.rotation);
            Destroy(particle, 0.5f);
            Destroy(box.gameObject);
        }
        Destroy(fireClone);
        Destroy(smokeClone);
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
                GameObject particle = Instantiate(blockDestroyParticle, go.transform.localPosition, transform.rotation);
                Destroy(particle, 0.5f);
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
        if (matchFinder.currentMatches.Count == 0)
        {
            AddXPandScorePoints();
        }
        else
        {
            AddXPandScorePoints();
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
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < hight; y++)
            {
                if (allBoxes[x, y] == null && !blankSpaces[x, y])
                {
                    for (int k = y + 1; k < hight; k++)
                    {
                        if (allBoxes[x, k] != null)
                        {
                            if(bombTiles[x,k])
                            {
                                bombTiles[x, y] = bombTiles[x, k];
                                bombTiles[x, k] = null;
                            }
                            allBoxes[x, k].GetComponent<Box>().row = y;
                            allBoxes[x, k] = null;
                            break;
                        }
                    }
                }
            }
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
                if(allBoxes[x, y] == null && levelSettings.levelTemplate.isLeaderboard)
                {
                    int bombChance = Random.Range(0, 101);
                    if(bombChance < levelSettings.levelTemplate.bombChance)
                    {
                        Vector2 tempPos = new Vector2(x, y + offset);
                        GameObject bomb = Instantiate(bombTilePrefab, parent.position + new Vector3(tempPos.x, tempPos.y), transform.rotation, parent);
                        bombTiles[x, y] = bomb.GetComponent<BombTile>();
                        bomb.GetComponent<Box>().row = y;
                        bomb.GetComponent<Box>().column = x;
                        allBoxes[x, y] = bomb;
                        bomb.name = "Bomb";
                        continue;
                    }
                }
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