using UnityEngine;
using UnityEngine.Tilemaps;

/**
 * A MonoBehaviour class representing the grid of a game level.
 * 
 * @author Erin Ratelle
 * Reference: https://www.youtube.com/watch?v=ODLzYI4d-J8
 */
public class Level : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Tetromino activePiece { get; private set; }
    public Tetromino.Data[] tetrominoes;
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

    /**
     * Called when the scene is loaded.
     */
    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
    }

    /**
     * Called once all scene components are Awake.
     */
    private void Start()
    {
        SpawnPiece();
    }

    /**
     * Generates and places a random Tetromino in this level.
     */
    public void SpawnPiece()
    {
        int random = Random.Range(0, this.tetrominoes.Length);
        Tetromino.Data data = this.tetrominoes[random];

        this.activePiece = gameObject.AddComponent<Tetromino>() as Tetromino;
        this.activePiece.Initialize(this, this.spawnPosition, data);

        if (IsValidPosition(this.activePiece, this.spawnPosition))
        {
            Set(this.activePiece);
        } else
        {
            // GAME OVER
            this.tilemap.ClearAllTiles(); // placeholder!!
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
        int inARow = 0;

        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                ClearLine(row);
                inARow++;
                if (inARow == 4)
                {
                    //TETRIS bonus!
                    inARow = 0;
                } 
                // single-line score
            } else
            {
                row++;
                inARow = 0;
            }
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
        while (row < bounds.xMax)
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
}
