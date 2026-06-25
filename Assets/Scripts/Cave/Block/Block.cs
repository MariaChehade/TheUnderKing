using UnityEngine;

public class Block : MonoBehaviour
{
    public BlockType blockType;

    public int maxHealth;

    public Sprite fullSprite;
    public Sprite damagedSprite;

    public int state2Threshold;

    public GameObject dropPrefab;

    [Tooltip("Fator de escala do bloco (collider + sprite). Use 1.0 para exato, >1 para fechar frestas.")]
    [Range(0.5f, 2f)]
    public float blockScale = 1.1f;

    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider    = GetComponent<BoxCollider2D>();
        currentHealth  = maxHealth;

        UpdateVisual();
        FitSpriteToCollider();
    }

    /// <summary>
    /// Escala o sprite para ocupar <see cref="blockScale"/> unidades de mundo,
    /// depois ajusta o BoxCollider2D para ter exatamente o mesmo tamanho.
    /// </summary>
    private void FitSpriteToCollider()
    {
        if (spriteRenderer == null || spriteRenderer.sprite == null || boxCollider == null)
            return;

        // Tamanho natural do sprite em unidades de mundo (depende do Pixels Per Unit)
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        if (spriteSize.x <= 0f || spriteSize.y <= 0f)
            return;

        // 1. Escala o transform para que o sprite renderize em blockScale × blockScale unidades
        float scaleX = blockScale / spriteSize.x;
        float scaleY = blockScale / spriteSize.y;
        transform.localScale = new Vector3(scaleX, scaleY, transform.localScale.z);

        // 2. Collider em espaço LOCAL = spriteSize original
        //    Em espaço MUNDO = spriteSize * localScale = blockScale  ← mesmo tamanho do sprite
        boxCollider.size = spriteSize;
    }


    // ── Colisão com o Player ───────────────────────────────────────────────────

    /// <summary>
    /// Rastreia se o player está em contato com o bloco.
    /// O dano só ocorre quando o player pressiona uma seta enquanto encostado.
    /// </summary>
    private bool _playerInContact = false;

    private void Update()
    {
        if (!_playerInContact) return;

        // Cada pressionamento de seta = 1 de dano (GetKeyDown: dispara uma vez por tecla)
        if (Input.GetKeyDown(KeyCode.UpArrow)    ||
            Input.GetKeyDown(KeyCode.DownArrow)  ||
            Input.GetKeyDown(KeyCode.LeftArrow)  ||
            Input.GetKeyDown(KeyCode.RightArrow) ||
            
            Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.A) ||
            Input.GetKeyDown(KeyCode.S) ||
            Input.GetKeyDown(KeyCode.D))
        {
            TakeDamage(1);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        _playerInContact = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        _playerInContact = false;
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