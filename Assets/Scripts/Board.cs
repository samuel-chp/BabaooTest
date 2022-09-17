using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private Transform tilesHolder;

    [SerializeField] private Vector2 cellSize = Vector2.one;
    [SerializeField] private float step = 0f;
    
    [SerializeField] private int boardWidth = 3;
    [SerializeField] private int boardHeight = 3;

    public void AddTile(Tile tile)
    {
        tile.transform.SetParent(tilesHolder);
        tile.transform.localRotation = Quaternion.identity;
        SetTileLocalPosition(tile, tile.position);
    }
    
    /// <summary>
    /// Position the tile in the world with the given board position.
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="newBoardPosition">The position in the board.</param>
    public void SetTileLocalPosition(Tile tile, Vector2 newBoardPosition)
    {
        Vector3 localPos = new Vector3(
            newBoardPosition.x * cellSize.x + newBoardPosition.x * step,
            newBoardPosition.y * cellSize.y + newBoardPosition.y * step,
            0f
        );
        tile.transform.localPosition = localPos;
    }

    public void Empty()
    {
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

    public Vector2 GetBoardPositionFromWorldPosition(Vector3 worldPos)
    {
        Vector2 localPos = tilesHolder.transform.InverseTransformPoint(worldPos);
        localPos.Scale(new Vector3(1/(cellSize.x + step/2), 1/(cellSize.y + step/2), 1f));
        return localPos;
    }
}
