using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    public Vector2Int GridPosition
    {
        get;
        set;
    }

    public int GridIndex { get; set; }

    public int OrderInLayer
    {
        get => spriteRenderer.sortingOrder;
        set => spriteRenderer.sortingOrder = value;
    }

    public void SetSprite(Sprite newSprite)
    {
        spriteRenderer.sprite = newSprite;
    }
}
