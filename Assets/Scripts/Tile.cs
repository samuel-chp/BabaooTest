using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Material _defaultMaterial;
    [SerializeField] private Material _transparentMaterial;

    [HideInInspector] public int index;
    [HideInInspector] public Vector2Int position;

    private Sprite _sprite;

    public Sprite GetSprite()
    {
        return _sprite;
    }
    
    public void SetSprite(Sprite newSprite)
    {
        if (_spriteRenderer != null)
            _spriteRenderer.sprite = newSprite;

        if (_meshRenderer != null)
        {
            if (newSprite == null)
            {
                _meshRenderer.material = _transparentMaterial;
            }
            else
            {
                Material newMat = new Material(_defaultMaterial);
                newMat.SetTexture("_MainTex", newSprite.texture);
                _meshRenderer.material = newMat;
            }
        }

        _sprite = newSprite;
    }
}
