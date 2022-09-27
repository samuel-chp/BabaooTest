using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class PuzzleManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private PlayerControls playerControls;
    
    [Header("Boards")]
    [SerializeField] private Board mainBoard;
    [SerializeField] private Board[] otherBoards;
    [SerializeField] private Board boardForEndingAnimation;

    [Header("UI")]
    [SerializeField] private Timer timer;

    [Header("Parameters")] 
    [SerializeField] private int shuffleIterations = 1000;
    [SerializeField] private int timeLimitInSeconds = 180;
    
    [Header("Puzzle Sprites")]
    [SerializeField] private PuzzleSprites androidSprites;
    [SerializeField] private PuzzleSprites iosSprites;
    
    private Vector2Int _emptyTilePosition;

    public Vector2Int EmptyTilePosition => _emptyTilePosition;
    public Board MainBoard => mainBoard;

    private void Start()
    {
        PopulatePuzzle();
        ShufflePuzzle();
        
        playerControls.puzzleManager = this;
        playerControls.EnableInputs();

        timer.maxTimeInSeconds = timeLimitInSeconds;
        timer.ResetTimer();
        timer.UnpauseTimer();
    }

    [ContextMenu("Populate Puzzle")]
    public void PopulatePuzzle()
    {
        // Remove all tiles
        EmptyPuzzle();
        
        // Instantiate tiles
        // TODO: handle more sprites
        Sprite[] sprites = GetPuzzleSprites().sprites;
        int idx = 0;
        for (int j = 2; j >= 0; j--) // From up to bottom
        {
            for (int i = 0; i <= 2; i++) // From right to left
            {
                mainBoard.AddTile(idx, new Vector2Int(i, j), sprites[idx]);

                idx++;
            }
        }

        // Remove the sprite in the middle (not the tile)
        _emptyTilePosition = Vector2Int.one;
        mainBoard.GetTileAtPosition(_emptyTilePosition).SetSprite(null);
        
        // Refresh other boards
        foreach (Board board in otherBoards)
        {
            board.CopyBoard(mainBoard);
        }
    }

    [ContextMenu("Empty Puzzle")]
    public void EmptyPuzzle()
    {
        mainBoard.Empty();
        foreach (Board board in otherBoards)
        {
            board.Empty();
        }
    }

    public void MoveTile(Tile tile, bool checkForWin = true)
    {
        // Swap tiles position
        Vector2Int tilePos = tile.position;
        Vector2Int emptyTilePos = _emptyTilePosition;
        
        mainBoard.SwapTile(tilePos, emptyTilePos);
        _emptyTilePosition = tilePos;
        
        // Reproduce in other boards
        foreach (Board board in otherBoards)
        {
            board.SwapTile(tilePos, emptyTilePos);
        }
        
        if (checkForWin) CheckForWin();
    }

    private void CheckForWin()
    {
        // Check if puzzle finished
        bool win = true;
        int idx = 0;
        Tile[,] tileMap = mainBoard.TileMap;
        for (int j = tileMap.GetLength(1) - 1; j >= 0; j--)
        {
            for (int i = 0; i <= tileMap.GetLength(0) - 1; i++)
            {
                if (tileMap[i, j].index != idx)
                {
                    win = false;
                    break;
                }

                idx++;
            }
        }

        // TODO: add win
        if (win)
        {
            Win();
        }
    }

    public void Win()
    {
        playerControls.DisableInputs();
        timer.PauseTimer();
        GameManager.SaveBestScore(timer.GetScore());
        StartCoroutine(FinishGame());

        IEnumerator FinishGame()
        {
            Debug.Log("You win!");
            
            // Replace tile in the middle
            boardForEndingAnimation.RemoveTile(new Vector2Int(1, 1));
            boardForEndingAnimation.AddTile(-1, new Vector2Int(1, 1), GetPuzzleSprites().sprites[4]);
            
            boardForEndingAnimation.PlayWinAnimation();
            yield return new WaitForSeconds(5f);
            GameManager.GoToMainMenu();
        }
    }
    
    public void Lose()
    {
        playerControls.DisableInputs();
        timer.PauseTimer();
        StartCoroutine(FinishGame());
        
        IEnumerator FinishGame()
        {
            Debug.Log("You lose!");

            boardForEndingAnimation.PlayLoseAnimation();
            yield return new WaitForSeconds(5f);
            GameManager.GoToMainMenu();
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
        for (int i = 0; i < shuffleIterations; i++)
        {
            List<Tile> candidates = new List<Tile>();

            if (_emptyTilePosition.x > 0)
            {
                candidates.Add(mainBoard.GetTileAtPosition(new Vector2Int(_emptyTilePosition.x - 1, _emptyTilePosition.y)));
            }
            if (_emptyTilePosition.x < 2)
            {
                candidates.Add(mainBoard.GetTileAtPosition(new Vector2Int(_emptyTilePosition.x + 1, _emptyTilePosition.y)));
            }
            if (_emptyTilePosition.y > 0)
            {
                candidates.Add(mainBoard.GetTileAtPosition(new Vector2Int(_emptyTilePosition.x, _emptyTilePosition.y - 1)));
            }
            if (_emptyTilePosition.y < 2)
            {
                candidates.Add(mainBoard.GetTileAtPosition(new Vector2Int(_emptyTilePosition.x, _emptyTilePosition.y + 1)));
            }

            Tile rndTile = candidates[Random.Range(0, candidates.Count)];
            MoveTile(rndTile, false);
        }
    }
}
