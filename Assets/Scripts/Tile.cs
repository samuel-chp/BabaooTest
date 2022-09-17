using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    
    [HideInInspector] public int index;
    [HideInInspector] public Vector2Int position;

    public void SetSprite(Sprite newSprite)
    {
        _spriteRenderer.sprite = newSprite;
    }
}
