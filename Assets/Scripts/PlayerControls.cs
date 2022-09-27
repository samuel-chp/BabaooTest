using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] private Camera activeCamera;

    private PlayerPuzzleActions _actions;
    private Tile _selectedTile;
    private Vector3 _deltaPos;

    private Vector3 CursorWorldPos
    {
        get
        {
            Vector2 cursorPos = _actions.Game.Move.ReadValue<Vector2>();
            
            // Translate in world pos
            Vector3 cursorWorldPos = cursorPos;
            if (!activeCamera.orthographic)
            {
                if (_selectedTile == null)
                {
                    cursorWorldPos.z = Mathf.Clamp(puzzleManager.MainBoard.transform.position.z - activeCamera.transform.position.z, 0f, 1000f);
                }
                else
                {
                    cursorWorldPos.z = Mathf.Clamp(_selectedTile.transform.position.z - activeCamera.transform.position.z, 0f, 1000f);
                }
            }
            
            cursorWorldPos = activeCamera.ScreenToWorldPoint(cursorWorldPos);
            return cursorWorldPos;
        }
    }
    
    public PuzzleManager puzzleManager { get; set; }

    private void Awake()
    {
        _actions = new PlayerPuzzleActions();
    }

    public void EnableInputs()
    {
        _actions.Enable();
    }
    
    public void DisableInputs()
    {
        _actions.Disable();
    }

    private void Start()
    {
        _actions.Game.Select.performed += SelectTile;
        _actions.Game.Move.performed += MoveTile;
    }

    private void SelectTile(InputAction.CallbackContext context)
    {
        if (_actions.Game.Select.IsPressed())
        {
            // RaycastHit2D hit = Physics2D.Raycast(activeCamera.ScreenToWorldPoint(cursorPos), Vector2.zero);
            RaycastHit hit;
            Ray ray = new Ray(activeCamera.transform.position,
                (CursorWorldPos - activeCamera.transform.position).normalized);
            Physics.Raycast(ray, out hit, 100f);

            if (hit.collider != null)
            {
                Tile tile = hit.collider.GetComponent<Tile>();
                if (tile != null && puzzleManager.MainBoard.GetTileAtPosition(tile.position) == tile) // Select only from main board
                {
                    // If the empty tile is a neighbor
                    if (Vector2Int.Distance(tile.position, puzzleManager.EmptyTilePosition) <= 1.01f)
                    {
                        _selectedTile = tile;
                        _deltaPos = CursorWorldPos - _selectedTile.transform.position;
                    }
                }
            }
        }
        else
        {
            if (_selectedTile != null)
            {
                // Release tile to the nearest position
                Vector2 boardPos =
                    puzzleManager.MainBoard.GetBoardPositionFromWorldPosition(_selectedTile.transform.position);
                if (Vector2.Distance(puzzleManager.EmptyTilePosition, boardPos) <
                    Vector2.Distance(_selectedTile.position, boardPos))
                {
                    puzzleManager.MoveTile(_selectedTile);
                }
                else
                {
                    puzzleManager.MainBoard.SetTilePosition(_selectedTile, _selectedTile.position);
                }
            }
            
            _selectedTile = null;
        }
    }
    
    private void MoveTile(InputAction.CallbackContext context)
    {
        if (_selectedTile != null)
        {
            Vector2 cursorPos = puzzleManager.MainBoard.GetBoardPositionFromWorldPosition(CursorWorldPos - _deltaPos); // Board coords

            // Movement in only one direction
            Vector2 normal =  puzzleManager.EmptyTilePosition - _selectedTile.position;
            Vector2 newPos = 
                Vector2.Dot(cursorPos - _selectedTile.position, normal) * normal;
            newPos += _selectedTile.position;
            
            // Clamp position
            Vector2 tilePos = _selectedTile.position;
            Vector2 emptyPos = puzzleManager.EmptyTilePosition;
            newPos.x = Mathf.Clamp(newPos.x, Mathf.Min(tilePos.x, emptyPos.x), Mathf.Max(tilePos.x, emptyPos.x));
            newPos.y = Mathf.Clamp(newPos.y, Mathf.Min(tilePos.y, emptyPos.y), Mathf.Max(tilePos.y, emptyPos.y));

            puzzleManager.MainBoard.SetTilePosition(_selectedTile, newPos);
        }
    }
}
