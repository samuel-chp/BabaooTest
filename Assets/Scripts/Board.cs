using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] protected Transform tilesHolder;

    [SerializeField] private Vector2 cellSize = Vector2.one;
    [SerializeField] private float step = 0f;

    // Private
    protected Tile[,] _tileMap = new Tile[3,3];

    // Properties
    public Tile[,] TileMap => _tileMap;

    /// <summary>
    /// Add a copy of the tile to the board an position the tile according to its position.
    /// </summary>
    /// <param name="tile"></param>
    public virtual void AddTile(int tileIndex, Vector2Int tilePosition, Sprite tileSprite)
    {
        if (_tileMap[tilePosition.x, tilePosition.y] != null)
        {
            Debug.LogError("Existing tile already in the given position.");
            return;
        }
        
        // Instantiate a new tile GO
        GameObject tileGO = Instantiate(tilePrefab, transform.position, Quaternion.identity);
        Tile tile = tileGO.GetComponent<Tile>();

        // Copy the tile parameters
        tile.transform.name = tileIndex.ToString();
        tile.index = tileIndex;
        tile.position = tilePosition;
        tile.SetSprite(tileSprite);

        // Position the tile
        _tileMap[tile.position.x, tile.position.y] = tile;
        tile.transform.SetParent(tilesHolder);
        tile.transform.localRotation = Quaternion.identity;
        SetTilePosition(tile, tile.position);
    }

    public void RemoveTile(Vector2Int tilePosition)
    {
        if (_tileMap[tilePosition.x, tilePosition.y] != null)
        {
            Tile tile = _tileMap[tilePosition.x, tilePosition.y];
            _tileMap[tilePosition.x, tilePosition.y] = null;
            Destroy(tile.gameObject);
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
            AddTile(tile.index, tile.position, tile.GetSprite());
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

    public virtual void PlayWinAnimation()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Win");
        }
    }
    
    public virtual void PlayLoseAnimation()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Lose");
        }
    }
}
