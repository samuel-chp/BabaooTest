using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PuzzleManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Board board;

    [Header("Puzzles")]
    [SerializeField] private PuzzleSprites androidSprites;
    [SerializeField] private PuzzleSprites iosSprites;

    private Tile[,] _tileMap = new Tile[3,3];
    private Tile _emptyTile;

    public Tile EmptyTile => _emptyTile;
    public Board Board => board;

    private void Start()
    {
        PopulatePuzzle();
        ShufflePuzzle();
    }

    [ContextMenu("Populate Puzzle")]
    public void PopulatePuzzle()
    {
        // Remove all tiles
        EmptyPuzzle();
        
        // Instantiate sprites
        // TODO: handle more sprites
        int idx = 0;
        for (int j = 2; j >= 0; j--)
        {
            for (int i = 0; i <= 2; i++)
            {
                GameObject tileGO = Instantiate(
                    tilePrefab, 
                    Vector3.zero, 
                    Quaternion.identity);
                Tile tile = tileGO.GetComponent<Tile>();
                
                tile.transform.name = idx.ToString();
                tile.position = new Vector2Int(i, j);
                tile.index = idx;
                board.AddTile(tile);

                _tileMap[i, j] = tile;
                
                idx++;
            }
        }

        // Set sprites
        // TODO: handle wrong puzzle sprites number
        Sprite[] sprites = GetPuzzleSprites().sprites;
        idx = 0;
        for (int j = 2; j >= 0; j--)
        {
            for (int i = 0; i <= 2; i++)
            {
                _tileMap[i, j].SetSprite(sprites[idx]);
                idx++;
            }
        }
        
        // Remove the sprite in the middle (not the tile)
        _emptyTile = _tileMap[1, 1];
        _emptyTile.SetSprite(null);
    }

    [ContextMenu("Empty Puzzle")]
    public void EmptyPuzzle()
    {
        _tileMap = new Tile[3,3];
        board.Empty();
    }

    public void SwapTile(Tile tile, bool checkForWin = true)
    {
        // Swap transform position
        board.SetTileLocalPosition(tile, _emptyTile.position);
        board.SetTileLocalPosition(_emptyTile, tile.position);

        // Swap in list
        _tileMap[tile.position.x, tile.position.y] = _emptyTile;
        _tileMap[_emptyTile.position.x, _emptyTile.position.y] = tile;
        
        // Update properties
        (tile.position, _emptyTile.position) = (_emptyTile.position, tile.position);

        // Check if puzzle finished
        bool win = true;
        int idx = 0;
        for (int j = 2; j >= 0; j--)
        {
            for (int i = 0; i <= 2; i++)
            {
                if (_tileMap[i, j].index != idx)
                {
                    win = false;
                    break;
                }

                idx++;
            }
        }

        // TODO: add win
        if (win && checkForWin)
        {
            Debug.Log("You won!");
        }
    }

    /// <summary>
    /// Return the targeted PuzzleSprites object.
    /// </summary>
    /// <returns></returns>
    private PuzzleSprites GetPuzzleSprites()
    {
        // TODO: handle empty object
        switch (Application.platform)
        {
            default:
                return androidSprites;
            case RuntimePlatform.Android:
                return androidSprites;
            case RuntimePlatform.IPhonePlayer:
                return iosSprites;
        }
    }

    [ContextMenu("Shuffle Puzzle")]
    private void ShufflePuzzle()
    {
        for (int i = 0; i < 1; i++)
        {
            List<Tile> candidates = new List<Tile>();

            if (_emptyTile.position.x > 0)
            {
                candidates.Add(_tileMap[_emptyTile.position.x - 1, _emptyTile.position.y]);
            }
            if (_emptyTile.position.x < 2)
            {
                candidates.Add(_tileMap[_emptyTile.position.x + 1, _emptyTile.position.y]);
            }
            if (_emptyTile.position.y > 0)
            {
                candidates.Add(_tileMap[_emptyTile.position.x, _emptyTile.position.y - 1]);
            }
            if (_emptyTile.position.y < 2)
            {
                candidates.Add(_tileMap[_emptyTile.position.x, _emptyTile.position.y + 1]);
            }

            Tile rndTile = candidates[Random.Range(0, candidates.Count)];
            SwapTile(rndTile, false);
        }
    }
}
