using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer _spriteRenderer;

    [HideInInspector] public int index;
    [HideInInspector] public Vector2Int position;

    public Sprite GetSprite()
    {
        return _spriteRenderer.sprite;
    }
    
    public virtual void SetSprite(Sprite newSprite)
    {
        if (_spriteRenderer != null)
            _spriteRenderer.sprite = newSprite;
    }
}
