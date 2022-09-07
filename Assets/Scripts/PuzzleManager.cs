using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Main class for handling puzzle related actions.
/// </summary>
public class PuzzleManager : MonoBehaviour
{
    public bool debug = true;
    
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private float tileSize = 5.12f;
    [SerializeField] private float tileStep = 0.1f;
    [SerializeField] private Transform tileHolder;
    
    [SerializeField] private PuzzleSpriteContainer[] puzzleSpritesContainers;
    [SerializeField] [Tooltip("Final complete image of the puzzle.")] private GameObject resultImage;
    [SerializeField] [Tooltip("Game over text UI.")] private GameObject loseObject;

    // Private
    private Sprite[] orderedPuzzleSprites;
    private Tile[,] tileGrid = new Tile[3, 3];

    // Properties
    /// <summary>
    /// Position of the current free tile.
    /// </summary>
    public Vector2Int FreeTile
    {
        get;
        private set;
    }

    private void Start()
    {
        // Select set of sprites depending on platform
        if (puzzleSpritesContainers.Length < 1)
        {
            Debug.LogError("Missing puzzle sprites.");
            Application.Quit();
        }
        switch (Application.platform)
        {
            default:
                orderedPuzzleSprites = puzzleSpritesContainers[0].sprites;
                break;
            case RuntimePlatform.Android:
                foreach (PuzzleSpriteContainer container in puzzleSpritesContainers)
                {
                    if (container.containerName == "Android")
                    {
                        orderedPuzzleSprites = container.sprites;
                        break;
                    }
                }
                break;
            case RuntimePlatform.IPhonePlayer:
                foreach (PuzzleSpriteContainer container in puzzleSpritesContainers)
                {
                    if (container.containerName == "IOS")
                    {
                        orderedPuzzleSprites = container.sprites;
                        break;
                    }
                }
                break;
        }

        if (orderedPuzzleSprites == null)
        {
            Debug.LogError("Missing sprites for current platform.");
            orderedPuzzleSprites = puzzleSpritesContainers[0].sprites;
        }
        
        EmptyGrid();
        PopulateGrid();
        RandomizeGrid();
        
        // Ensure result image is disabled.
        resultImage.SetActive(false);
    }

    /// <summary>
    /// Remove all tiles from the grid.
    /// </summary>
    [ContextMenu("Empty puzzle")]
    private void EmptyGrid()
    {
        for (int i = tileHolder.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying)
            {
                Destroy(tileHolder.GetChild(i).gameObject);
            }
            else
            {
                DestroyImmediate(tileHolder.GetChild(i).gameObject);
            }
        }
    }
    
    /// <summary>
    /// Instantiate tiles in the grid.
    /// </summary>
    [ContextMenu("Populate puzzle")]
    private void PopulateGrid()
    {
        int nRows = 3;
        int nCols = 3;
        
        // Set Positions
        for (int i = 0; i < nCols; i++)
        {
            for (int j = 0; j < nRows; j++)
            {
                // Vector3 position = new Vector3(i * (tileSize + tileStep), -j * (tileSize + tileStep), 0f);
                Vector2 position = GetGridToWorldPosition(new Vector2Int(i, j));
                tileGrid[i, j] = Instantiate(
                    tilePrefab,
                    position,
                    Quaternion.identity,
                    tileHolder);
                tileGrid[i, j].GridPosition = new Vector2Int(i, j);
            }
        }

        // Set Sprite
        for (int j = 0; j < nRows; j++)
        {
            for (int i = 0; i < nCols; i++)
            {
                tileGrid[i, j].SetSprite(orderedPuzzleSprites[j*nRows + i]);
                tileGrid[i, j].GridIndex = j * nRows + i;
            }
        }

        // Remove the tile in the center
        if (Application.isPlaying)
        {
            Destroy(tileGrid[1, 1].gameObject);
        }
        else
        {
            DestroyImmediate(tileGrid[1, 1].gameObject);
        }
        FreeTile = Vector2Int.one;
    }

    /// <summary>
    /// Perform a random number of permutations of tiles to get a solvable puzzle.
    /// </summary>
    private void RandomizeGrid()
    {
        // TODO: improve this algorithm
        int n = debug ? 1 : Random.Range(100, 10000);
        for (int i = 0; i < n; i++)
        {
            // Get a random adjacent tile to the FreeTile
            List<Vector2Int> adjacentGridPos = new List<Vector2Int>();
            if (FreeTile.x >= 1)
            {
                adjacentGridPos.Add(new Vector2Int(FreeTile.x - 1, FreeTile.y));
            }
            if (FreeTile.x <= 1)
            {
                adjacentGridPos.Add(new Vector2Int(FreeTile.x + 1, FreeTile.y));
            }
            if (FreeTile.y >= 1)
            {
                adjacentGridPos.Add(new Vector2Int(FreeTile.x, FreeTile.y - 1));
            }
            if (FreeTile.y <= 1)
            {
                adjacentGridPos.Add(new Vector2Int(FreeTile.x, FreeTile.y + 1));
            }
            Vector2Int _selectedTilePosition = adjacentGridPos[Random.Range(0, adjacentGridPos.Count)];
            if (_selectedTilePosition != FreeTile && (FreeTile - _selectedTilePosition).magnitude < 1.01f)
            {
                SwapTile(tileGrid[_selectedTilePosition.x, _selectedTilePosition.y], false);
            }
        }
    }

    /// <summary>
    /// Check if the puzzle is completed.
    /// If true, trigger win.
    /// </summary>
    private void CheckForCompletion()
    {
        bool win = true;
        int n = 0;
        for (int j = 0; j < 3; j++)
        {
            for (int i = 0; i < 3; i++)
            {
                // Check if all tiles are in order
                if (tileGrid[i, j] != null && tileGrid[i, j].GridIndex != n)
                {
                    win = false;
                    break;
                }
                n++;
            }
        }

        if (win)
        {
            // Debug.Log("Win!");
            StartCoroutine(Win());
        }
    }

    /// <summary>
    /// Exchange the tileToSwap with the current FreeTile.
    /// Careful: remember to check if the swap is possible before calling this.
    /// </summary>
    /// <param name="tileToSwap"></param>
    /// <param name="check"></param>
    public void SwapTile(Tile tileToSwap, bool check = true)
    {
        (tileToSwap.GridPosition, FreeTile) = (FreeTile, tileToSwap.GridPosition);
        tileToSwap.transform.position = GetGridToWorldPosition(tileToSwap.GridPosition);
        tileGrid[tileToSwap.GridPosition.x, tileToSwap.GridPosition.y] = tileToSwap;
        tileGrid[FreeTile.x, FreeTile.y] = null;
        if (check) CheckForCompletion();
    }

    /// <summary>
    /// Get the world position of a tile given its grid coordinates.
    /// </summary>
    /// <param name="gridPosition"></param>
    /// <returns></returns>
    public Vector2 GetGridToWorldPosition(Vector2Int gridPosition)
    {
        Vector2 position = new Vector2(
            gridPosition.x * (tileSize + tileStep),
            -gridPosition.y * (tileSize + tileStep));
        position -= new Vector2(tileSize + tileStep, -(tileSize + tileStep));
        position.Scale(transform.localScale);
        return (Vector2)tileHolder.position + position;
    }

    /// <summary>
    /// Refresh all tiles world position.
    /// Useful when re-scaling the puzzle.
    /// </summary>
    public void RefreshGrid()
    {
        for (int j = 0; j < 3; j++)
        {
            for (int i = 0; i < 3; i++)
            {
                if (tileGrid[i, j] != null) 
                    tileGrid[i, j].transform.position = GetGridToWorldPosition(tileGrid[i, j].GridPosition);
            }
        }
    }

    /// <summary>
    /// Win events such as showing the completed puzzle, animations and going back to main menu.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Win()
    {
        // Disable player inputs
        FindObjectOfType<PlayerManager>().gameObject.SetActive(false);
        
        // Save score
        Timer timer = FindObjectOfType<Timer>();
        GameManager.Instance.SaveScore(timer.ElapsedTime);
        
        // Disable Timer
        timer.gameObject.SetActive(false);
        
        Vector3 position = GetGridToWorldPosition(Vector2Int.one);
        tileGrid[1, 1] = Instantiate(
            tilePrefab,
            position,
            Quaternion.identity,
            transform);
        tileGrid[1, 1].SetSprite(orderedPuzzleSprites[4]);
        yield return new WaitForSeconds(1f);
        
        EmptyGrid();
        
        resultImage.SetActive(true);
    }

    /// <summary>
    /// Lose events such as showing the losing text.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Lose()
    {
        // Disable player inputs
        FindObjectOfType<PlayerManager>().gameObject.SetActive(false);
        
        // Disable Timer
        FindObjectOfType<Timer>().gameObject.SetActive(false);
        
        loseObject.SetActive(true);
        
        yield return new WaitForSeconds(5f);
        
        ExitGame();
    }

    public void ExitGame()
    {
        GameManager.Instance.GoToMainMenu();
    }
}
