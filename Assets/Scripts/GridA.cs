using System.Collections;
using TMPro;
using UnityEngine;

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
    [Header("Prefabs")]
    public GameObject[] boxPrefabs;
    [SerializeField] GameObject breakableTilePrefab;
    [SerializeField] GameObject bombTilePrefab;
    [SerializeField] GameObject lockTilePrefab;
    [SerializeField] GameObject fire;
    [SerializeField] GameObject smoke;
    [SerializeField] GameObject warpedPart;
    [SerializeField] Transform crosshair;
    [SerializeField] Material blackAndWhiteMat;

    [Header("Sounds")]
    [SerializeField] AudioClip blockDestroySFX;
    [SerializeField] AudioClip swipeBoxesSFX;
    [SerializeField] AudioClip returnBoxesSFX;
    [SerializeField] AudioClip firedUpSFX;
    [SerializeField] AudioClip warpedSFX;
    [SerializeField] AudioClip deadLock1;
    [SerializeField] AudioClip deadLock2;

    [Header("Special blocks")]
    [SerializeField] Material firedUpMat;
    [SerializeField] Material warpedMat;

    [Header("Add for match")]
    [SerializeField] int scorePointsToAddperBox = 10;
    [SerializeField] float pointsXPforLevel = 1;

    [Header("Grid Settings")]
    [SerializeField] LevelTemplate tutorialTemplate;
    [SerializeField] TileType[] boardLayout;
    public GameObject[,] allBoxes;
    [SerializeField] float gridCreateDelay = 3.5f;
    public int width = 8;
    public int hight = 8;
    public int offset;
    public GameState currState = GameState.move;

    public Transform boxesCenterPanel;
    [SerializeField] Transform parentOfAllBoxes;
    public Box currBox;

    private const float aspectRatioMultiplier = 9.0f / 16 * 7.5f;

    private int scoreStreak = 1;
    public bool[,] blankSpaces;
    private BackgroundTile[,] breakableTiles;
    public BackgroundTile[,] lockedTiles;
    public BombTile[,] bombTiles;
    MatchFinder matchFinder;
    LevelTemplate template;
    AudioSource audioSource;
    public int bombSpawnChance { private set; get; }
    bool boxDestroyed = false; //to check if sfx need to be played

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
    public delegate void MyDelegate(int row, int column);
    public event MyDelegate onGoldRushMatch;

    public static GridA Instance;

    void Awake()
    {
        Instance = this;
        if (LevelSettingsKeeper.settingsKeeper != null)
        {
            bombSpawnChance = LevelSettingsKeeper.settingsKeeper.worldInfo.BombSpawnChance;
            boxPrefabs = LevelSettingsKeeper.settingsKeeper.worldInfo.Boxes;
            template = LevelSettingsKeeper.settingsKeeper.worldInfo.LeaderboardLevelTemplate;
            if (template != null)
            {
                width = template.width;
                hight = template.hight;
                offset = template.offset;
                boardLayout = template.boardLayout;
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
        parentOfAllBoxes.position = new Vector2(startParentX, startParentY);

        matchFinder = MatchFinder.Instance;
        audioSource = GetComponent<AudioSource>();

        if (AudioController.Instance)
        {
            float sfxVolume = AudioController.Instance.SFXVolume;
            audioSource.volume = sfxVolume;
            AudioController.Instance.onSFXVolumeChange += ChangeSFXVolume;
        }

        if (CheckForTutorial())
        {
            return;
        }
        StartCoroutine(CreateGridDelayed(gridCreateDelay));
        //CreateGrid();
    }
    IEnumerator CreateGridDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        CreateGrid();
    }
    bool CheckForTutorial()
    {
        if (PlayerPrefs.GetInt("TUTORIAL", 0) == 1) //if not first time playing
        {
            return false;
        }

        currState = GameState.wait;
        return true;
    }

    public void SetDefaultTemplate()
    {
        var layout = new TileType[0];
        template = ScriptableObject.CreateInstance("LevelTemplate") as LevelTemplate;
        template.boardLayout = layout;
        boardLayout = layout;

        template.hight = 8;
        template.width = 8;

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (allBoxes[x, y] != null)
                {
                    Destroy(allBoxes[x, y]);
                    allBoxes[x, y] = null;
                }
                blankSpaces[x, y] = false;
            }
        }
        StartCoroutine(CreateGridDelayed(3.5f));
    }

    public void FillTutorialLayout()
    {
        allBoxes = new GameObject[8, 8];
        template = tutorialTemplate;
        boardLayout = template.boardLayout;

        template.hight = 8;
        template.width = 8;

        int layoutIndex = 0;
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                boardLayout[layoutIndex].x = x;
                boardLayout[layoutIndex].y = y;
                boardLayout[layoutIndex].tileKind = TileKind.Blank;
                layoutIndex++;
            }
        }

        Vector2 tempPos = new Vector3(1, 7 + offset);
        SpawnNormalBlock(1, 7, tempPos, 3); //yellow on the left
        for (int x = 2; x < 5; x += 2)
        {
            tempPos = new Vector3(x, 7 + offset); //red on 2 places
            SpawnNormalBlock(x, 7, tempPos, 4);
        }
        tempPos = new Vector3(3, 6 + offset); //red on 1 place below
        SpawnNormalBlock(3, 6, tempPos, 4);
        tempPos = new Vector3(3, 7 + offset); //blue between red ones
        SpawnNormalBlock(3, 7, tempPos, 0);
        tempPos = new Vector3(5, 7 + offset); //blue on the right
        SpawnNormalBlock(5, 7, tempPos, 0);

        GenerateBlankSpaces();
    }

    public void CrosshairToBlock(int x, int y)
    {
        crosshair.position = new Vector2(x, y);
    }

    #region generate_tiles
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
                    parentOfAllBoxes.position + new Vector3(boardLayout[i].x, boardLayout[i].y), transform.rotation, parentOfAllBoxes);
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
                    parentOfAllBoxes.position + new Vector3(boardLayout[i].x, boardLayout[i].y), transform.rotation, parentOfAllBoxes);
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
                GameObject bomb = Instantiate(bombTilePrefab, parentOfAllBoxes.position + new Vector3(boardLayout[i].x, boardLayout[i].y), transform.rotation, parentOfAllBoxes);
                bombTiles[boardLayout[i].x, boardLayout[i].y] = bomb.GetComponent<BombTile>();
                bomb.GetComponent<Box>().column = (int)tempPos.y;
                bomb.GetComponent<Box>().row = (int)tempPos.x;
                allBoxes[(int)tempPos.x, (int)tempPos.y] = bomb;
                bomb.name = "Bomb";
            }
        }
    }
    #endregion
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
                    SpawnNormalBlock(x, y, tempPos, randIndex);
                }
            }
        }
    }

    private void SpawnNormalBlock(int x, int y, Vector2 tempPos, int index)
    {
        GameObject go = Instantiate(boxPrefabs[index], tempPos, transform.rotation, parentOfAllBoxes);
        go.GetComponent<Box>().column = y;
        go.GetComponent<Box>().row = x;
        allBoxes[x, y] = go;
        go.name = x + "," + y;
    }

    public void SetBlankSpace(int x, int y, bool blank)
    {
        blankSpaces[x, y] = blank;
    }

    bool MatchesAt(int row, int column, GameObject box)
    {
        if (row > 1 && column > 1)
        {
            if (allBoxes[row - 1, column] && allBoxes[row - 2, column])
            {
                if (allBoxes[row - 1, column].CompareTag(box.tag) && allBoxes[row - 2, column].CompareTag(box.tag))
                {
                    return true;
                }
            }
            if (allBoxes[row, column - 1] && allBoxes[row, column - 2])
            {
                if (allBoxes[row, column - 1].CompareTag(box.tag) && allBoxes[row, column - 2].CompareTag(box.tag))
                {
                    return true;
                }
            }
        }
        else if (row <= 1 && column > 1)
        {
            if (allBoxes[row, column - 1] && allBoxes[row, column - 2])
            {
                if (allBoxes[row, column - 1].CompareTag(box.tag) && allBoxes[row, column - 2].CompareTag(box.tag))
                {
                    return true;
                }
            }
        }
        if (row > 1 && column <= 1)
        {
            if (allBoxes[row - 1, column] && allBoxes[row - 2, column])
            {
                if (allBoxes[row - 1, column].CompareTag(box.tag) && allBoxes[row - 2, column].CompareTag(box.tag))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void DestroyMatched(int row, int column)
    {
        if (allBoxes[row, column] != null && allBoxes[row, column].GetComponent<Box>().isMatched)
        {
            if (onGoldRushMatch != null)
            {
                onGoldRushMatch(row, column);
                return;
            }

            if (GoalManager.Instance != null)
            {
                GoalManager.Instance.CompareGoal(allBoxes[row, column].tag);
            }
            if (breakableTiles[row, column] != null)
            {
                breakableTiles[row, column].TakeDamage();
            }
            if (lockedTiles[row, column] != null)
            {
                lockedTiles[row, column].TakeDamage();
            }
            DestroyBombTile(row, column);

            Box matchedBox = allBoxes[row, column].GetComponent<Box>();
            if (matchedBox.FiredUp)
            {
                StartCoroutine(FiredUpBlock(matchedBox));
            }
            else if(matchedBox.Warped)
            {
                StartCoroutine(DestroyAllSameColor(matchedBox.tag));
            }

            CheckBomb();
            DestroyBlockAtPosition(row, column);

            matchFinder.currentMatches.Remove(allBoxes[row, column]);
            currBox = null;
            boxDestroyed = true;
        }
    }
    void DestroyBombTile(int row, int column)
    {
        if (row > 0)
        {
            if (bombTiles[row - 1, column])
            {
                DestroyBlockAtPosition(row - 1, column);
            }
        }
        if (row < width - 1)
        {
            if (bombTiles[row + 1, column])
            {
                DestroyBlockAtPosition(row + 1, column);
            }
        }
        if (column > 0)
        {
            if (bombTiles[row, column - 1])
            {
                DestroyBlockAtPosition(row, column - 1);
            }
        }
        if (column < hight - 1)
        {
            if (bombTiles[row, column + 1])
            {
                DestroyBlockAtPosition(row, column + 1);
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
                    DestroyBombTile(currBox.row, currBox.column);
                    SetBlockFiredUp(currBox);
                    currBox.GetComponent<SpriteRenderer>().color = Color.white;
                }
                else
                {
                    if (currBox.neighborBox != null && currBox.neighborBox.GetComponent<Box>().isMatched)
                    {
                        Box neighbor = currBox.neighborBox.GetComponent<Box>();
                        DestroyBombTile(neighbor.row, neighbor.column);
                        SetBlockFiredUp(neighbor);
                        neighbor.GetComponent<SpriteRenderer>().color = Color.white;
                    }
                }
            }
        }
        else if (matchFinder.currentMatches.Count == 5)
        {
            if (currBox != null)
            {
                if (currBox.isMatched)
                {
                    DestroyBombTile(currBox.row, currBox.column);
                    SetBlockWarped(currBox);
                    currBox.GetComponent<SpriteRenderer>().color = Color.white;
                }
                else if (currBox.neighborBox != null && currBox.neighborBox.GetComponent<Box>().isMatched)
                {
                    Box neighbor = currBox.neighborBox.GetComponent<Box>();
                    DestroyBombTile(neighbor.row, neighbor.column);
                    SetBlockWarped(neighbor);
                    neighbor.GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
        }
    }
    public IEnumerator FiredUpBlock(Box box)
    {
        yield return null;
        if (box != null)
        {
            foreach (Vector2Int dir in directions)
            {
                if (box.row + dir.x >= 0 && box.row + dir.x < width
                    && box.column + dir.y >= 0 && box.column + dir.y < hight)
                {
                    DestroyBlockAtPosition(box.row + dir.x, box.column + dir.y);
                }
            }
            var camShake = Camera.main.GetComponent<CameraShake>();
            StartCoroutine(camShake.Shake(0.07f, 0.04f));

            DestroyBlockAtPosition(box.row, box.column);
            BlockDestroyedSFX();
        }

        StartCoroutine(MoveBoxesDown());
    }
    public void SetBlockFiredUp(Box box)
    {
        box.isMatched = false;
        box.FiredUp = true;
        FiredUpVFX(box);
    }    
    public void SetBlockWarped(Box box)
    {
        box.gameObject.tag = "Untagged";
        box.isMatched = false;
        box.Warped = true;
        WarpBoxVFX(box);
    }
    public IEnumerator DestroyAllSameColor(string boxTag, float delay = 2)
    {
        audioSource.PlayOneShot(warpedSFX);
        foreach (GameObject go in allBoxes)
        {
            if (go != null && go.CompareTag(boxTag))
            {
                if (go != null)
                {
                    //mark all blocks with the same tag 
                    go.GetComponent<SpriteRenderer>().color = Color.gray;
                }
            }
        }
        yield return new WaitForSeconds(delay);
        foreach (GameObject go in allBoxes)
        {
            if (go != null && go.CompareTag(boxTag))
            {
                yield return new WaitForSeconds(.1f);
                if (go != null)
                {
                    Box boxToDestroy = go.GetComponent<Box>();
                    DestroyBlockAtPosition(boxToDestroy.row, boxToDestroy.column);
                    BlockDestroyedSFX();
                }
            }
        }
        StartCoroutine(MoveBoxesDown());
    }

    public void DestroyBlockAtPosition(int row, int column)
    {
        if (allBoxes[row, column] != null)
        {
            DynamicBlockSpriteDestruction(row, column);
            //BlockDestroyedSFX();  //sound is played once after all matched blocks are destroyed

            allBoxes[row, column] = null;
            bombTiles[row, column] = null;

            AddXPandScorePoints();
        }
    }
    //destroys block without blockFX
    public void DestroyBlockNoFX(int row, int column)
    {
        if (allBoxes[row, column] != null)
        {
            Destroy(allBoxes[row, column]);
            allBoxes[row, column] = null;
            bombTiles[row, column] = null;

            AddXPandScorePoints();
        }
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
        if (boxDestroyed)
        {
            BlockDestroyedSFX();
            boxDestroyed = false;
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
                            if (bombTiles[x, k])
                            {
                                bombTiles[x, y] = bombTiles[x, k];
                                bombTiles[x, k] = null;
                            }
                            allBoxes[x, k].GetComponent<Box>().column = y;
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
                if (allBoxes[x, y] == null && !blankSpaces[x, y])
                {
                    int bombChance = Random.Range(0, 100);
                    if (bombChance < bombSpawnChance)
                    {
                        Vector2 tempPos = new Vector2(x, y + offset);
                        GameObject bomb = Instantiate(bombTilePrefab, parentOfAllBoxes.position + new Vector3(tempPos.x, tempPos.y), transform.rotation, parentOfAllBoxes);
                        bombTiles[x, y] = bomb.GetComponent<BombTile>();
                        bomb.GetComponent<Box>().column = y;
                        bomb.GetComponent<Box>().row = x;
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
                    box.GetComponent<Box>().row = x;
                    box.GetComponent<Box>().column = y;
                    box.transform.SetParent(parentOfAllBoxes);
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

    float timeUntilStreakReset = 3;
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
        currBox = null;
        yield return new WaitForSeconds(.5f);
        if (IsDeadlocked())
        {
            Debug.Log("Is deadlocked");
            HandleDeadlock();
            yield break;
        }
        currState = GameState.move;
    }

    private void SwitchPieces(int row, int column, Vector2 direction)
    {
        GameObject holder = allBoxes[row + (int)direction.x, column + (int)direction.y];
        allBoxes[row + (int)direction.x, column + (int)direction.y] = allBoxes[row, column];
        allBoxes[row, column] = holder;
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
    public bool SwitchAndCheck(int row, int column, Vector2 direction)
    {
        SwitchPieces(row, column, direction);
        if (CheckForMatches())
        {
            SwitchPieces(row, column, direction);
            return true;
        }
        SwitchPieces(row, column, direction);
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

    bool handlingDeadlock = false;
    void HandleDeadlock()
    {
        if (PlayerPrefs.GetInt("TUTORIAL", 0) == 0)
        {
            currState = GameState.move;
            return; //if first time playing
        }
        if (handlingDeadlock) return;

        audioSource.PlayOneShot(deadLock1);
        handlingDeadlock = true;
        currState = GameState.wait;
        StartCoroutine(Camera.main.GetComponent<CameraShake>().Shake(0.2f, 0.2f));
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < hight; y++)
            {
                if (allBoxes[x, y] != null)
                {
                    allBoxes[x, y].GetComponent<SpriteRenderer>().material = blackAndWhiteMat;
                }
            }
        }
        audioSource.PlayOneShot(deadLock2);
        StartCoroutine(DeadlockMoveBoxesDown());
        EndGameManager.Instance.GameOver();
    }

    IEnumerator DeadlockMoveBoxesDown()
    {
        yield return new WaitForSeconds(0.5f);
        for (int y = 0; y < width; y++)
        {
            for (int x = 0; x < hight; x++)
            {
                if (allBoxes[x, y] != null)
                {
                    allBoxes[x, y].GetComponent<Box>().MoveBoxDown();
                    yield return new WaitForSeconds(0.01f);
                }
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void TurnBlocksOff()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < hight; j++)
            {
                if (allBoxes[i, j] != null)
                {
                    var collider = allBoxes[i, j].GetComponent<Collider2D>();
                    if (collider)
                        Destroy(collider);
                }
            }
        }
    }

    #region add_game_params
    public void IncreaseBombSpawnChance(int value)
    {
        bombSpawnChance += value;
    }
    void AddXPandScorePoints()
    {
        if (Score.Instance == null || LevelSlider.Instance == null) return;
        Score.Instance.AddPoints(scorePointsToAddperBox * scoreStreak);
        LevelSlider.Instance.AddXPtoLevel(pointsXPforLevel);
        CoinsDisplay.Instance.RandomizeCoin();
    }
    public void SetXPpointsPerBoxByProcent(float procent)
    {
        pointsXPforLevel *= procent;
    }
    #endregion
    #region VFX
    private void DynamicBlockSpriteDestruction(int row, int column)
    {
        var explodable = allBoxes[row, column].GetComponent<Explodable>();
        if (!explodable)
        {
            Destroy(allBoxes[row, column]);
            return;
        }

        explodable.explode();
        ExplosionForce ef = ExplosionForce.Instance;
        ef.doExplosion(allBoxes[row, column].transform.position);
    }
    void FiredUpVFX(Box box)
    {
        box.GetComponent<SpriteRenderer>().material = firedUpMat;
        Instantiate(smoke, box.gameObject.transform.position, smoke.transform.rotation, box.transform);
        Instantiate(fire, box.gameObject.transform.position, transform.rotation, box.transform);
        audioSource.PlayOneShot(firedUpSFX);
    }
    void WarpBoxVFX(Box box)
    {
        box.GetComponent<SpriteRenderer>().material = warpedMat;
        Instantiate(warpedPart, box.gameObject.transform.position, transform.rotation, box.transform);
        audioSource.PlayOneShot(warpedSFX);
    }
    #endregion
    #region SFX
    void ChangeSFXVolume(float volume)
    {
        audioSource.volume = volume;
    }
    void BlockDestroyedSFX()
    {
        audioSource.PlayOneShot(blockDestroySFX);
    }
    public void ReturnBoxesSFX()
    {
        audioSource.PlayOneShot(returnBoxesSFX);
    }
    public void SwipeBoxesSFX()
    {
        audioSource.PlayOneShot(swipeBoxesSFX);
    }
    #endregion
}