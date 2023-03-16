using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

/**
 * A MonoBehaviour class representing the grid of a game level.
 * 
 * @author Erin Ratelle
 * Reference: https://www.youtube.com/watch?v=ODLzYI4d-J8
 */
public class Level : MonoBehaviour
{
    public Tetromino.Data[] tetrominos;
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);
    public Vector2Int gridSize = new Vector2Int(10, 20);
    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-gridSize.x / 2, -gridSize.y / 2);
            return new RectInt(position, gridSize);
        }
    }

    public AudioSource audioMove;
    public AudioSource audioLock;
    public AudioSource audioScore;

    public float stepDelay = 1f;
    public float lockDelay = .5f;

    public TMP_Text timerText;
    public TMP_Text scoreText;
    public TMP_Text levelText;

    private Tilemap tilemap;
    private Tetromino activePiece;
    private List<Tetromino.Data> tetrominoBag = new List<Tetromino.Data> ();
    private int lastCleared = 0;
    private int gameLevel = 0;
    private int pointsThisLevel = 0;

    private static int _SCORE = 0;
    public static int SCORE
    {
        get { return _SCORE; }
        private set { _SCORE = value; }
    }
    private static float _DURATION = 0;
    public static float DURATION
    {
        get { return _DURATION; }
        private set { _DURATION = value; }
    }

    /**
     * Called when the scene is loaded.
     */
    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        OptionsMenu.EnableKeys(); // make sure at least standard keys are enabled!
    }

    /**
     * Called once all scene components are Awake.
     */
    private void Start()
    {
        //FillGrid();
        DURATION = 0; // reset, since Time.time tracks time of program, not scene!
        SpawnPiece();
        LevelUp();
    }

    /**
     * Frame update. 
     * Handles applicable user input.
     */
    void Update()
    {
        if (OptionsMenu.CheckMove(OptionsMenu.MoveAction.Pause))
        {
            PauseMenu.OpenPause();
        }

        IncrementTime();
    }

    /**
     * Debugging assistance method. 
     * Fills grid with tetrainbinow!
     */
    private void FillGrid()
    {
        RectInt bounds = this.Bounds;
        int i = 0;
        for (int row = bounds.yMin; row < bounds.yMax; row++)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int cell = new Vector3Int(col, row, 0);
                Tetromino.Data data = tetrominos[i];
                this.tilemap.SetTile(cell, data.tile);

                i = Tetromino.Wrap(i + 1, 0, tetrominos.Length);
            }
        }
    }

    /**
     * Generates and places a random Tetromino in this level.
     */
    public void SpawnPiece()
    {
        // avoid too many repeats by using "7-bag" technique
        if (tetrominoBag.Count == 0)
        {
            for (int i = 0; i < tetrominos.Length; i++)
            {
                tetrominoBag.Add(tetrominos[i]);
            }
        }

        int random = Random.Range(0, tetrominoBag.Count);
        Tetromino.Data data = tetrominoBag[random];
        tetrominoBag.RemoveAt(random);

        this.activePiece = gameObject.AddComponent<Tetromino>() as Tetromino;
        this.activePiece.Initialize(this, this.spawnPosition, data);

        if (IsValidPosition(this.activePiece, this.spawnPosition))
        {
            Set(this.activePiece);
        } else
        {
            //this.tilemap.ClearAllTiles();
            SceneManager.LoadScene("_GameOver");
        }
    }

    /**
     * Draws the given Tetromino on this level's tilemap.
     * 
     * @param piece     The Tetromino to draw.
     */
    public void Set(Tetromino piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    /**
     * Visually erases the given Tetromino from this level's tilemap.
     * 
     * @param piece     The Tetromino to erase.
     */
    public void Clear(Tetromino piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, null);
        }
    }

    /**
     * Validates the given potential position of the given piece by checking for collision with this level's existing tilemap.
     * 
     * @param piece     The Tetromino whose individual blocks need to be checked for collisions.
     * @param position  The potential position at which to check the Tetromino for collisions.
     * @return          True if no collisions are detected, False otherwise.
     */
    public bool IsValidPosition(Tetromino piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }

            if (this.tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

    /**
     * Clears any completed lines, adding to the user's score.
     */
    public void ClearLines()
    {
        RectInt bounds = this.Bounds;
        int row = bounds.yMin;
        int count = 0;

        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                ClearLine(row);
                count++;
            } else
            {
                row++;
            }
        }

        IncrementScore(GetPoints(count));
        if (count > 0)
        {
            lastCleared = count; 
            audioScore.Play();
        }
    }

    /**
     * Checks if a given row of this level's tilemap has a tile in every column.
     * 
     * @param row   The row to check.
     * @return      True if every column has a tile, False otherwise.
     */
    private bool IsLineFull(int row)
    {
        RectInt bounds = this.Bounds;
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int cell = new Vector3Int(col, row, 0);
            if (!this.tilemap.HasTile(cell)) { return false; }
        }
        return true;
    }

    /**
     * Clears a single full row from this level's tilemap.
     * 
     * @param row       The row to clear.
     */
    private void ClearLine(int row)
    {
        RectInt bounds = this.Bounds;

        // first clear the row
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int cell = new Vector3Int(col, row, 0);
            this.tilemap.SetTile(cell, null);
        }

        // now drop the rows above
        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int cell = new Vector3Int(col, row + 1, 0);
                TileBase above = this.tilemap.GetTile(cell);
                cell = new Vector3Int(col, row, 0);
                this.tilemap.SetTile(cell, above);
            }
            row++;
        }
    }

    /**
     * Calculates the points to award for the given number of lines cleared.
     * 
     * @param lines     The number of lines cleared.
     * @return          The points to award.
     */
    private int GetPoints(int lines)
    {
        if (lines <= 0)
        {
            return 0;
        }

        // base points
        int points = (2 * lines) - 1;

        // tetris?
        if (lines % 4 == 0) points += lines / 4;

        // back-to-back?
        if (lines >= 4 && lastCleared >= 4)
        {
            if (lines < 6)
            {
                points = 12;
            } else
            {
                points += ((lines - 4) * 2) - 1;
            }
        }

        return points;
    }

    /**
     * Handles clock tick.
     */
    private void IncrementTime()
    {
        DURATION += Time.deltaTime;
        int d = (int)DURATION;
        int minutes = d / 60;
        int seconds = d % 60;
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    /**
     * Updates game score and level.
     */
    private void IncrementScore(int points)
    {
        SCORE += points;
        scoreText.text = SCORE.ToString("#,0");

        pointsThisLevel += points;
        if (pointsThisLevel / 5 > gameLevel)
        {
            LevelUp();
        }
    }

    /**
     * Levels up and increases drop rate.
     */
    private void LevelUp()
    {
        gameLevel++;
        pointsThisLevel = 0;
        levelText.text = "Lvl " + gameLevel;
        stepDelay = (float)System.Math.Pow(0.8 - (0.007 * (gameLevel - 1)), gameLevel - 1);
    }
}
