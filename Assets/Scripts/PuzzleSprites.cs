using UnityEngine;

/// <summary>
/// ScriptableObject containing the sprites of a puzzle.
/// Each sprite should be 1 unit.
/// </summary>
[CreateAssetMenu(fileName = "PlatformSprites", menuName = "ScriptableObjects/PuzzleSpritesHolder")]
public class PuzzleSprites : ScriptableObject
{
    public Sprite[] sprites;
}
