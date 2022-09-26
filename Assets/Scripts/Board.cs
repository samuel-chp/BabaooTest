using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Board : MonoBehaviour
{
    [SerializeField] private Transform tilesHolder;

    [SerializeField] private Vector2 cellSize = Vector2.one;
    [SerializeField] private float step = 0f;

    // Private
    private Tile[,] _tileMap = new Tile[3,3];

    // Properties
    public Tile[,] TileMap => _tileMap;

    /// <summary>
    /// Add a tile to the board an position the tile according to its position.
    /// </summary>
    /// <param name="tile"></param>
    public void AddTile(Tile tile)
    {
        if (_tileMap[tile.position.x, tile.position.y] != null)
        {
            Debug.LogError("Existing tile already in the given position.");
            return;
        }

        _tileMap[tile.position.x, tile.position.y] = tile;
        tile.transform.SetParent(tilesHolder);
        tile.transform.localRotation = Quaternion.identity;
        SetTilePosition(tile, tile.position);
    }

    public void RemoveTile(Tile tile)
    {
        if (_tileMap[tile.position.x, tile.position.y] == tile)
        {
            _tileMap[tile.position.x, tile.position.y] = null;
        }
    }

    public Tile GetTileAtPosition(Vector2Int tilePosition)
    {
        return _tileMap[tilePosition.x, tilePosition.y];
    }
    
    /// <summary>
    /// Copy by cloning an existing board.
    /// </summary>
    /// <param name="board</param>
    public virtual void CopyBoard(Board board)
    {
        Empty();
        
        Tile[,] tileMap = board.TileMap;
        foreach (Tile tile in tileMap)
        {
            Tile tileCopy = Instantiate(tile);
            AddTile(tileCopy);
        }
    }

    public void SwapTile(Vector2Int tilePositionA, Vector2Int tilePositionB)
    {
        Tile tileA = _tileMap[tilePositionA.x, tilePositionA.y];
        Tile tileB = _tileMap[tilePositionB.x, tilePositionB.y];

        if (tileA == null || tileB == null)
        {
            Debug.LogError("No existing tile at the given position.");
            return;
        }
        
        // Swap tiles in map
        _tileMap[tileA.position.x, tileA.position.y] = tileB;
        _tileMap[tileB.position.x, tileB.position.y] = tileA;
        
        // Swap transform position
        SetTilePosition(tileA, tileB.position);
        SetTilePosition(tileB, tileA.position);

        // Update properties
        (tileA.position, tileB.position) = (tileB.position, tileA.position);
    }

    /// <summary>
    /// Remove all tiles from board
    /// </summary>
    [ContextMenu("Remove all tiles")]
    public void Empty()
    {
        _tileMap = new Tile[3, 3];
        for (int i = tilesHolder.transform.childCount-1; i >= 0; i--)
        {
            if (Application.isPlaying)
            {
                Destroy(tilesHolder.transform.GetChild(i).gameObject);
            }
            else
            {
                DestroyImmediate(tilesHolder.transform.GetChild(i).gameObject);
            }
        }
    }
    
    /// <summary>
    /// Position the tile in the world with the given board position.
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="newBoardPosition">The position in the board.</param>
    public void SetTilePosition(Tile tile, Vector2 newBoardPosition)
    {
        Vector3 localPos = new Vector3(
            newBoardPosition.x * cellSize.x + newBoardPosition.x * step,
            newBoardPosition.y * cellSize.y + newBoardPosition.y * step,
            0f
        );
        tile.transform.localPosition = localPos;
    }

    public Vector2 GetBoardPositionFromWorldPosition(Vector3 worldPos)
    {
        Vector2 localPos = tilesHolder.transform.InverseTransformPoint(worldPos);
        localPos.Scale(new Vector3(1/(cellSize.x + step/2), 1/(cellSize.y + step/2), 1f));
        return localPos;
    }
}
