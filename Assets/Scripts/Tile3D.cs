using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile3D : Tile
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Material _defaultMaterial;
    [SerializeField] private Material _transparentMaterial;

    public override void SetSprite(Sprite newSprite)
    {
        base.SetSprite(newSprite);

        if (_meshRenderer != null)
        {
            if (newSprite == null)
            {
                _meshRenderer.material = _transparentMaterial;
            }
            else
            {
                _meshRenderer.material = _defaultMaterial;
            }
        }
    }
}
