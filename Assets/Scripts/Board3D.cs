using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board3D : Board
{
    [SerializeField] private GameObject tile3DPrefab;
    
    public override void CopyBoard(Board board)
    {
        Empty();
        
        Tile[,] tileMap = board.TileMap;
        foreach (Tile tile in tileMap)
        {
            GameObject tileGO = Instantiate(tile3DPrefab, transform.position, Quaternion.identity);
            Tile tileCopy = tileGO.GetComponent<Tile>();

            tileCopy.transform.name = tile.transform.name;
            tileCopy.index = tile.index;
            tileCopy.position = tile.position;
            tileCopy.SetSprite(tile.GetSprite());

            AddTile(tileCopy);
            tileCopy.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
        }
    }
}
