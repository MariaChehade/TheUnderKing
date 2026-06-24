using UnityEngine;

[CreateAssetMenu(menuName = "Blocks/Block Data")]
public class BlockData : ScriptableObject
{
    public BlockType blockType;

    public int maxHealth = 3;

    public Sprite fullSprite;
    public Sprite damagedSprite;

    public GameObject dropPrefab;
}