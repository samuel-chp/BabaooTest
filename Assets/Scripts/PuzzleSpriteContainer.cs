using UnityEngine;

[CreateAssetMenu(fileName = "PuzzleSprites", menuName = "ScriptableObjects/PuzzleSpritesContainer", order = 1)]
public class PuzzleSpriteContainer : ScriptableObject
{
    public string name;
    public Sprite[] sprites;
}
