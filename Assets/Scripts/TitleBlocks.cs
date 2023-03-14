using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TitleBlocks : MonoBehaviour
{
    public Tetromino.Data data;
    public Vector3Int spawnPosition = new Vector3Int(1, 1, 0);
    public float initialDelay = 0f;

    private Tilemap tilemap;
    private AudioSource audioSource;

    /**
     * Called when the scene is loaded.
     */
    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
    }

    // Called once all scene components are Awake.
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // wait long enough for title to be typed
        Invoke("DrawTetromino", initialDelay);
    }
    
    /**
     * Draws this component's Tetromino.
     */
    private void DrawTetromino()
    {
        // learn cells for given shape
        Vector2Int[,] stdCells = Tetromino.CELLS[data.shape];
        int length = stdCells.GetLength(1);
        Vector3Int[] cells = new Vector3Int[length];
        for (int i = 0; i < length; i++)
        {
            cells[i] = (Vector3Int)stdCells[0, i];
        }

        // now draw tetromino
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i] + spawnPosition;
            tilemap.SetTile(tilePosition, data.tile);
        }

        audioSource.Play();
    }
}
