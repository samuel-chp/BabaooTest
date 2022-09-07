using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenScaler : MonoBehaviour
{
    [SerializeField] private Vector2 referenceResolution = new Vector2(1920f, 1080f);
    [SerializeField] private bool scaleOnUpdate = false;

    private Vector2 _currentResolution;

    private void Start()
    {
        _currentResolution = new Vector2(Screen.width, Screen.height);
        
        Scale();
    }

    private void Update()
    {
        Vector2 resolution = new Vector2(Screen.width, Screen.height);
        if (scaleOnUpdate && (resolution - _currentResolution).magnitude > 0.1f)
        {
            Scale();
        }
    }

    private void Scale()
    {
        float screenRatio = Screen.width / referenceResolution.x;
        transform.localScale = Vector3.one * screenRatio;
        _currentResolution = new Vector2(Screen.width, Screen.height);

        PuzzleManager manager = FindObjectOfType<PuzzleManager>();
        manager.RefreshGrid();
        
        
    }
}
