using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/**
 * A MonoBehaviour class representing an individual Tetromino (Tetris piece).
 * Contains static lookups for standard piece shapes, positions, and alternate 
 * rotation positions to combat intersections ("wall kicks").
 * 
 * @author Erin Ratelle
 * Reference: https://www.youtube.com/watch?v=ODLzYI4d-J8
 */
public class Tetromino : MonoBehaviour
{
    /**
     * Standard Tetromino Shapes
     */
    public enum Shape
    {
        I,
        O,
        T,
        J,
        L,
        S,
        Z
    }

    /**
     * Maps standard Tetromino Shapes to their positions at 0, 90, 180, and 270 degrees.
     */
    public static readonly Dictionary<Shape, Vector2Int[,]> CELLS = new Dictionary<Shape, Vector2Int[,]>()
    {
        { Shape.I, new Vector2Int[,] { { new Vector2Int(-1, 1), new Vector2Int( 0, 1), new Vector2Int( 1, 1), new Vector2Int( 2, 1) },
            { new Vector2Int(1, 2), new Vector2Int( 1, 1), new Vector2Int( 1, 0), new Vector2Int( 1, -1) },
            { new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int( 1, 0), new Vector2Int( 2, 0) },
            { new Vector2Int(0, 2), new Vector2Int( 0, 1), new Vector2Int( 0, 0), new Vector2Int( 0, -1) } } },
        { Shape.J, new Vector2Int[,] { { new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int( 1, 0) },
            { new Vector2Int(0, 1), new Vector2Int( 1, 1), new Vector2Int( 0, 0), new Vector2Int( 0, -1) },
            { new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int( 1, 0), new Vector2Int( 1, -1) },
            { new Vector2Int(0, 1), new Vector2Int( 0, 0), new Vector2Int(-1, -1), new Vector2Int( 0, -1) } } },
        { Shape.L, new Vector2Int[,] { { new Vector2Int( 1, 1), new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int( 1, 0) },
            { new Vector2Int(0, 1), new Vector2Int( 0, 0), new Vector2Int( 0, -1), new Vector2Int( 1, -1) },
            { new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int( 1, 0), new Vector2Int( -1, -1) },
            { new Vector2Int(-1, 1), new Vector2Int( 0, 1), new Vector2Int( 0, 0), new Vector2Int( 0, -1) } } },
        { Shape.O, new Vector2Int[,] { { new Vector2Int( 0, 1), new Vector2Int( 1, 1), new Vector2Int( 0, 0), new Vector2Int( 1, 0) },
            { new Vector2Int( 0, 1), new Vector2Int( 1, 1), new Vector2Int( 0, 0), new Vector2Int( 1, 0) },
            { new Vector2Int( 0, 1), new Vector2Int( 1, 1), new Vector2Int( 0, 0), new Vector2Int( 1, 0) },
            { new Vector2Int( 0, 1), new Vector2Int( 1, 1), new Vector2Int( 0, 0), new Vector2Int( 1, 0) } } },
        { Shape.S, new Vector2Int[,] { { new Vector2Int( 0, 1), new Vector2Int( 1, 1), new Vector2Int(-1, 0), new Vector2Int( 0, 0) },
            { new Vector2Int(0, 1), new Vector2Int( 0, 0), new Vector2Int( 1, 0), new Vector2Int( 1, -1) },
            { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( -1, -1), new Vector2Int( 0, -1) },
            { new Vector2Int(-1, 1), new Vector2Int( -1, 0), new Vector2Int( 0, 0), new Vector2Int( 0, -1) } } },
        { Shape.T, new Vector2Int[,] { { new Vector2Int( 0, 1), new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int( 1, 0) },
            { new Vector2Int(0, 1), new Vector2Int( 0, 0), new Vector2Int( 1, 0), new Vector2Int( 0, -1) },
            { new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int( 1, 0), new Vector2Int( 0, -1) },
            { new Vector2Int(0, 1), new Vector2Int( -1, 0), new Vector2Int( 0, 0), new Vector2Int( 0, -1) } } },
        { Shape.Z, new Vector2Int[,] { { new Vector2Int(-1, 1), new Vector2Int( 0, 1), new Vector2Int( 0, 0), new Vector2Int( 1, 0) },
            { new Vector2Int(1, 1), new Vector2Int( 0, 0), new Vector2Int( 1, 0), new Vector2Int( 0, -1) },
            { new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int( 0, -1), new Vector2Int( 1, -1) },
            { new Vector2Int(0, 1), new Vector2Int( -1, 0), new Vector2Int( 0, 0), new Vector2Int( -1, -1) } } },
    };

    /**
     * The array of positional adjustments I Tetrominoes should attempt if a rotation fails due to intersection.
     */
    private static readonly Vector2Int[,] WallKicksI = new Vector2Int[,] {
        { new Vector2Int(0, 0), new Vector2Int(-2, 0), new Vector2Int( 1, 0), new Vector2Int(-2,-1), new Vector2Int( 1, 2) },
        { new Vector2Int(0, 0), new Vector2Int( 2, 0), new Vector2Int(-1, 0), new Vector2Int( 2, 1), new Vector2Int(-1,-2) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int( 2, 0), new Vector2Int(-1, 2), new Vector2Int( 2,-1) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int(-2, 0), new Vector2Int( 1,-2), new Vector2Int(-2, 1) },
        { new Vector2Int(0, 0), new Vector2Int( 2, 0), new Vector2Int(-1, 0), new Vector2Int( 2, 1), new Vector2Int(-1,-2) },
        { new Vector2Int(0, 0), new Vector2Int(-2, 0), new Vector2Int( 1, 0), new Vector2Int(-2,-1), new Vector2Int( 1, 2) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int(-2, 0), new Vector2Int( 1,-2), new Vector2Int(-2, 1) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int( 2, 0), new Vector2Int(-1, 2), new Vector2Int( 2,-1) },
    };

    /**
     * The array of positional adjustments non-I Tetrominoes should attempt if a rotation fails due to intersection.
     */
    private static readonly Vector2Int[,] WallKicksJLOSTZ = new Vector2Int[,] {
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0,-2), new Vector2Int(-1,-2) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1,-1), new Vector2Int(0, 2), new Vector2Int( 1, 2) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1,-1), new Vector2Int(0, 2), new Vector2Int( 1, 2) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0,-2), new Vector2Int(-1,-2) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1, 1), new Vector2Int(0,-2), new Vector2Int( 1,-2) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1,-1), new Vector2Int(0, 2), new Vector2Int(-1, 2) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1,-1), new Vector2Int(0, 2), new Vector2Int(-1, 2) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1, 1), new Vector2Int(0,-2), new Vector2Int( 1,-2) },
    };

    /**
     * Maps standard Tetromino Shapes to the array of positional adjustments to be used if a rotation fails due to intersection.
     */
    public static readonly Dictionary<Shape, Vector2Int[,]> WallKicks = new Dictionary<Shape, Vector2Int[,]>()
    {
        { Shape.I, WallKicksI },
        { Shape.J, WallKicksJLOSTZ },
        { Shape.L, WallKicksJLOSTZ },
        { Shape.O, WallKicksJLOSTZ },
        { Shape.S, WallKicksJLOSTZ },
        { Shape.T, WallKicksJLOSTZ },
        { Shape.Z, WallKicksJLOSTZ },
    };

    /**
     * Struct to hold Tetromino data fields to be stored in Level array, 
     * primarily for material mapping (Tile tile).
     */
    [System.Serializable]
    public struct Data
    {
        public Shape shape;
        public Tile tile;
    }

    public Level level { get; private set; }
    public Data data { get; private set; }
    public Vector3Int position { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector2Int[,] wallKicks { get; private set; }
    public int rotationIndex { get; private set; }
    public bool isBomb { get; private set; }
    public bool isLocked { get; private set; }
    public bool wasHeld { get; private set; }

    private float stepTime;
    private float lockTime;

    private static bool HARD_DROPPED = false;
    private static bool HELD = false;

    /**
     * MonoBehaviour does not allow constructors - Initialize() fulfills that functionality.
     * 
     * @param level     The level/board where this piece is being initialized.
     * @param data      The Tetromino keys to use when initializing this piece.
     */
    public void Initialize(Level level, Data data)
    {
        this.level = level;
        this.data = data;
        this.isBomb = PlayerPrefs.HasKey("BombBlocks") && (Random.value < this.level.bombChance);
        this.wasHeld = false;
        this.wallKicks = WallKicks[this.data.shape];
    }

    /**
     * Handles position and state specific initialization.
     * 
     * @param position  The starting position where this piece is being initialized.
     * @param locked    The locked state of the Tetromino (so that it doesn't move unless we want it to!)
     */
    public void Activate(Vector3Int position, bool locked)
    {
        this.position = position;
        this.isLocked = locked;

        this.rotationIndex = 0;
        UpdateCells();

        this.stepTime = Time.time + this.level.stepDelay;
        this.lockTime = 0f;
    }

    /**
     * Updates this Tetromino's cell positions based on its shape and current rotation.
     */
    private void UpdateCells()
    {
        Vector2Int[,] stdCells = CELLS[this.data.shape];
        int length = stdCells.GetLength(1);
        this.cells = new Vector3Int[length];
        for (int i = 0; i < length; i++)
        {
            this.cells[i] = (Vector3Int)stdCells[this.rotationIndex, i];
        }
    }

    /**
     * Frame update.
     * Handles both user-controlled moves/rotations and level "gravity".
     */
    private void Update()
    {
        if (!isLocked)
        {
            // erase previously-drawn tiles for this Tetromino
            this.level.Clear(this);

            // increment lock time (will be reset if move succeeds)
            this.lockTime += Time.deltaTime;

            // check for user-prompted hold
            // static boolean prevents instant hold when piece switches over (at least one Update must occur between holds)
            if (OptionsMenu.CheckMove(OptionsMenu.MoveAction.Hold) && !HELD && !this.wasHeld)
            {
                this.level.HoldPiece();
                HELD = true;
                this.wasHeld = true; // each piece can be held only once!
            }
            else
            {
                HELD = false;
            }

            // attempt user-prompted rotations 
            if (OptionsMenu.CheckMove(OptionsMenu.MoveAction.Counterclockwise))
            {
                Rotate(-1);
            }
            else if (OptionsMenu.CheckMove(OptionsMenu.MoveAction.Clockwise))
            {
                Rotate(1);
            }

            // attempt user-prompted motion
            if (OptionsMenu.CheckMove(OptionsMenu.MoveAction.Left))
            {
                Move(Vector2Int.left);
            }
            else if (OptionsMenu.CheckMove(OptionsMenu.MoveAction.Right))
            {
                Move(Vector2Int.right);
            }

            if (OptionsMenu.CheckMove(OptionsMenu.MoveAction.Down))
            {
                Move(Vector2Int.down);
            }

            // attempt user-prompted hard drop
            // static boolean prevents instand hard drop when piece switches over (at least one Update must occur between hard drops)
            if (OptionsMenu.CheckMove(OptionsMenu.MoveAction.HardDrop) && !HARD_DROPPED)
            {
                while (Move(Vector2Int.down))
                {
                    continue;
                }
                Lock();
                HARD_DROPPED = true;
            } else
            {
                HARD_DROPPED = false;
            }

            // perform time-based drop, check for lock condition
            if (Time.time >= this.stepTime)
            {
                Step();
            }

            // re-draw tiles for this Tetromino
            if (!isLocked) this.level.Set(this);
        }
    }

    /**
     * Performs downward step do to level "gravity".
     */
    private void Step()
    {
        this.stepTime = Time.time + this.level.stepDelay;
        Move(Vector2Int.down);
        if (this.isBomb) this.level.audioTick.Play();
        if (this.lockTime >= this.level.lockDelay)
        {
            Lock();
        }
    }

    /**
     * Locks this piece, preventing further moves/rotates.
     */
    private void Lock()
    {
        this.level.audioLock.Play();
        this.isLocked = true;
        this.level.Set(this);
        this.level.ClearLines();
        this.level.SpawnPiece();
    }

    /**
     * Attempts to re-position this Tetromino using the given translation. 
     * Fails if level reports a collision.
     * 
     * @param translation   The 2D vector to translate this Tetromino along.
     * @return              True if move was successful, False if move would create collision.
     */
    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = this.position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool isValid = this.level.IsValidPosition(this, newPosition);
        
        if (isValid)
        {
            this.position = newPosition;
            this.lockTime = 0f; // reset lock time if we successfully moved
            this.level.audioMove.Play();
        }

        return isValid;
    }

    /**
     * Attempts to rotate this Tetromino in the given direction.
     * Attempts standardized alternate positions if level reports a collision.
     * Fails if all alternatives cause a collision.
     * 
     * @param direction     1 or -1, indicating clockwise or counter-clockwise rotation respectively.
     * @return              True if rotation was successful in any position, False otherwise.
     */
    private bool Rotate(int direction)
    {
        this.rotationIndex = Wrap(this.rotationIndex + direction, 0, 4);
        UpdateCells();

        bool isValid = TestWallKicks(direction);

        if (!isValid)
        {
            this.rotationIndex = Wrap(this.rotationIndex - direction, 0, 4);
            UpdateCells();
        }

        return isValid;
    }

    /**
     * Validates all standardized alternative positions for a given rotation.
     * Fails if all alternatives cause a collision.
     * 
     * @param direction     1 or -1, indicating clockwise or counter-clockwise rotation respectively.
     * @return              True if any tested position does not cause a collision, False otherwise.
     */
    private bool TestWallKicks(int direction)
    {
        int wallKickIndex = GetWallKickIndex(direction);

        for (int i = 0; i < this.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = this.wallKicks[wallKickIndex, i];
            if (Move(translation)) return true;
        }

        return false;
    }

    /**
     * Returns the index of the standardized alternative positions for a given rotation.
     * 
     * @param direction     1 or -1, indicating clockwise or counter-clockwise rotation respectively.
     * @return              The index in this Tetromino's WallKicks array which corresponds to the appropriate set of standardized alternative positions.
     */
    private int GetWallKickIndex(int direction)
    {
        int wallKickIndex = this.rotationIndex * 2;
        if (direction < 0)
        {
            wallKickIndex--;
        }
        return Wrap(wallKickIndex, 0, this.wallKicks.GetLength(0));
    }

    /**
     * Helper function. Almost identical to modulo, but ensures a value between min and max.
     * 
     * @param input     The dividend value.
     * @param min       The inclusive minimum value of the modulo range (typically 0).
     * @param max       The exclusive maximum value of the modulo range (typically the divisor).
     * @return          The remainder, as a value that is >= min and < max.
     */
    public static int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        } else
        {
            return min + (input - min) % (max - min);
        }
    }
}
