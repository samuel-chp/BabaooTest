using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private PuzzleManager puzzleManager;
    
    private Vector2 _mousePosition;
    private Tile _selectedTile;
    private Vector2 _mouseToTileDelta;

    private InputMappings _touchControls;

    private void Awake()
    {
        _touchControls = new InputMappings();
    }

    private void OnEnable()
    {
        _touchControls.Enable();
    }

    private void OnDisable()
    {
        _touchControls.Disable();
    }

    private void Start()
    {
        _touchControls.Player.Select.started += SelectTile;
        _touchControls.Player.Select.canceled += SelectTile;
        _touchControls.Player.Move.performed += MoveTile;
    }

    public void SelectTile(InputAction.CallbackContext context)
    {
        _mousePosition = _touchControls.Player.Move.ReadValue<Vector2>();
        
        if (context.started)
        {
            RaycastHit2D hit = Physics2D.GetRayIntersection(cam.ScreenPointToRay(_mousePosition));
            if (hit)
            {
                _selectedTile = hit.transform.GetComponent<Tile>();
                if (_selectedTile != null)
                {
                    _mouseToTileDelta = cam.ScreenToWorldPoint(_mousePosition) - _selectedTile.transform.position;
                    _selectedTile.OrderInLayer = 10; // Ensure visibility of the movable tile
                }
            }
        }

        if (context.canceled && _selectedTile != null)
        {
            // Release tile to the closest tile position
            Vector2 freeTilePosition = puzzleManager.GetGridToWorldPosition(puzzleManager.FreeTile);
            Vector2 currentTilePosition = puzzleManager.GetGridToWorldPosition(_selectedTile.GridPosition);

            if (Vector2.Distance(_selectedTile.transform.position, freeTilePosition) <
                Vector2.Distance(_selectedTile.transform.position, currentTilePosition))
            {
                puzzleManager.SwapTile(_selectedTile);
            }
            else
            {
                _selectedTile.transform.position = currentTilePosition;
            }

            _selectedTile.OrderInLayer = 0;
            _selectedTile = null;
        }
    }

    public void MoveTile(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _mousePosition = context.ReadValue<Vector2>();

            if (_selectedTile != null)
            {
                Vector2 moveAxis = puzzleManager.FreeTile - _selectedTile.GridPosition;
                if (moveAxis.magnitude > 1.01f)
                {
                    // Tile and free tile not adjacent
                    return;
                }
                
                // Set tile pos to mouse pos
                Vector2 newTilePos = (Vector2)cam.ScreenToWorldPoint(_mousePosition) - _mouseToTileDelta;
                
                // Clamp x or y axis depending on the movement
                if (Mathf.Abs(moveAxis.x) < 0.01f)
                {
                    newTilePos.x = _selectedTile.transform.position.x;
                }
                if (Mathf.Abs(moveAxis.y) < 0.01f)
                {
                    newTilePos.y = _selectedTile.transform.position.y;
                }
                
                // Limit movement between the tiles
                Vector2 freeTilePosition = puzzleManager.GetGridToWorldPosition(puzzleManager.FreeTile);
                Vector2 currentTilePosition = puzzleManager.GetGridToWorldPosition(_selectedTile.GridPosition);
                newTilePos.x = Mathf.Clamp(
                    newTilePos.x,  
                    Mathf.Min(freeTilePosition.x, currentTilePosition.x), 
                    Mathf.Max(freeTilePosition.x, currentTilePosition.x));
                newTilePos.y = Mathf.Clamp(
                    newTilePos.y,  
                    Mathf.Min(freeTilePosition.y, currentTilePosition.y), 
                    Mathf.Max(freeTilePosition.y, currentTilePosition.y));
                
                _selectedTile.transform.position = new Vector3(newTilePos.x, newTilePos.y, _selectedTile.transform.position.z);
            }
        }
    }
}
