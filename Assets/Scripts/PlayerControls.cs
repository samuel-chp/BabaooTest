using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] private PuzzleManager puzzleManager;
    [SerializeField] private Camera activeCamera;
    
    private PlayerPuzzleActions _actions;
    private Tile _selectedTile;
    private Vector3 _deltaPos;

    private void Awake()
    {
        _actions = new PlayerPuzzleActions();
    }

    private void OnEnable()
    {
        _actions.Enable();
    }

    private void OnDisable()
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
            // Raycast under cursor for tiles
            Vector2 cursorPos = _actions.Game.Move.ReadValue<Vector2>();
            RaycastHit2D hit = Physics2D.Raycast(activeCamera.ScreenToWorldPoint(cursorPos), Vector2.zero);
            if (hit.collider != null)
            {
                Tile tile = hit.collider.GetComponent<Tile>();
                if (tile != null && Vector2Int.Distance(tile.position, puzzleManager.EmptyTile.position) <= 1.01f)
                {
                    // If the empty tile is a neighbor
                    _selectedTile = tile;
                    _deltaPos = activeCamera.ScreenToWorldPoint(cursorPos) - _selectedTile.transform.position;
                }
            }
        }
        else
        {
            if (_selectedTile != null)
            {
                // Release tile to the nearest position
                Vector2 boardPos =
                    puzzleManager.Board.GetBoardPositionFromWorldPosition(_selectedTile.transform.position);
                if (Vector2.Distance(puzzleManager.EmptyTile.position, boardPos) <
                    Vector2.Distance(_selectedTile.position, boardPos))
                {
                    puzzleManager.SwapTile(_selectedTile);
                }
                else
                {
                    puzzleManager.Board.SetTileLocalPosition(_selectedTile, _selectedTile.position);
                }
            }
            
            _selectedTile = null;
        }
    }
    
    private void MoveTile(InputAction.CallbackContext context)
    {
        if (_selectedTile != null)
        {
            Vector2 cursorPos = _actions.Game.Move.ReadValue<Vector2>(); // Screen coords
            cursorPos = activeCamera.ScreenToWorldPoint(cursorPos); // World coords
            cursorPos = puzzleManager.Board.GetBoardPositionFromWorldPosition((Vector3)cursorPos - _deltaPos); // Board coords

            // Movement in only one direction
            Vector2 normal =  puzzleManager.EmptyTile.position - _selectedTile.position;
            Vector2 newPos = 
                Vector2.Dot(cursorPos - _selectedTile.position, normal) * normal;
            newPos += _selectedTile.position;
            
            // Clamp position
            Vector2 tilePos = _selectedTile.position;
            Vector2 emptyPos = puzzleManager.EmptyTile.position;
            newPos.x = Mathf.Clamp(newPos.x, Mathf.Min(tilePos.x, emptyPos.x), Mathf.Max(tilePos.x, emptyPos.x));
            newPos.y = Mathf.Clamp(newPos.y, Mathf.Min(tilePos.y, emptyPos.y), Mathf.Max(tilePos.y, emptyPos.y));

            puzzleManager.Board.SetTileLocalPosition(_selectedTile, newPos);
        }
    }
}
