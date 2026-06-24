using UnityEngine;

public class Block : MonoBehaviour
{
    public BlockType blockType;

    public int maxHealth;

    public Sprite fullSprite;
    public Sprite damagedSprite;

    public int state2Threshold;

    public GameObject dropPrefab;

    private int currentHealth;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;

        UpdateVisual();
    }

    [ContextMenu("Tomar 1 de Dano")]
    private void TestTakeDamage()
    {
        TakeDamage(1);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log($"{blockType} HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            BreakBlock();
            return;
        }

        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (currentHealth == maxHealth)
        {
            spriteRenderer.sprite = fullSprite;
        }
        else
        {
            spriteRenderer.sprite = damagedSprite;
        }
    }

    private void BreakBlock()
    {
        Debug.Log($"{blockType} QUEBROU");

        if (dropPrefab != null)
        {
            Instantiate(
                dropPrefab,
                transform.position,
                Quaternion.identity
            );
        }

        Destroy(gameObject);
    }
}