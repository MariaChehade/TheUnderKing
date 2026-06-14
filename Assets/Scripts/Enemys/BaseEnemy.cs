using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour
{
    [SerializeField]
    protected float moveSpeed = 2f;

    [SerializeField]
    protected int damage = 1;

    [SerializeField]
    protected float attackCooldown = 1f;

    [SerializeField]
    protected int maxHealth = 3;

    protected int currentHealth;
    protected bool isAttacking;
    protected float lastAttackTime = float.MinValue;
    protected Rigidbody2D rigidbody2d;
    protected Collider2D enemyCollider;
    protected Castle targetCastle;

    protected virtual void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<Collider2D>();

        currentHealth = maxHealth;

        var enemyLayer = LayerMask.NameToLayer("Enemy");
        if (enemyLayer != -1)
        {
            Physics2D.IgnoreLayerCollision(enemyLayer, enemyLayer, true);
        }
    }

    protected virtual void FixedUpdate()
    {
        if (isAttacking)
        {
            StopMovement();
            TryAttackOnCooldown();
            return;
        }

        Move();
    }

    // Override this to implement movement behavior
    protected virtual void Move()
    {
        var movement = Vector2.right * moveSpeed;
        if (rigidbody2d != null)
        {
            rigidbody2d.linearVelocity = movement;
        }
        else
        {
            transform.Translate(movement * Time.fixedDeltaTime);
        }
    }

    protected void TryAttackOnCooldown()
    {
        if (targetCastle == null)
        {
            isAttacking = false;
            return;
        }

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            targetCastle.TakeDamage(damage);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryStartAttack(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryStartAttack(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryStartAttack(collision.collider);
    }

    protected virtual void TryStartAttack(Collider2D targetCollider)
    {
        if (isAttacking)
        {
            return;
        }

        var castle = targetCollider.GetComponentInParent<Castle>();
        if (castle == null)
        {
            return;
        }

        targetCastle = castle;
        isAttacking = true;
        StopAtCastle(targetCollider);
    }

    protected void StopAtCastle(Collider2D castleCollider)
    {
        StopMovement();

        if (enemyCollider == null)
        {
            return;
        }

        var position = transform.position;
        position.x = castleCollider.bounds.min.x - enemyCollider.bounds.extents.x;
        transform.position = position;
    }

    protected void StopMovement()
    {
        if (rigidbody2d != null)
        {
            rigidbody2d.linearVelocity = Vector2.zero;
        }
    }

    public virtual void TakeDamage(int arrowDamage)
    {
        if (arrowDamage <= 0 || currentHealth <= 0) return;

        currentHealth = Mathf.Max(0, currentHealth - arrowDamage);
        Debug.Log($"Enemy took {arrowDamage} damage. Remaining: {currentHealth}");

        if (currentHealth == 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    protected virtual void OnDrawGizmos()
    {
        var collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);

        if (Application.isPlaying)
        {
            float ratio = (maxHealth > 0) ? (float)currentHealth / maxHealth : 0f;
            var top = collider.bounds.max;
            var barSize = new Vector3(collider.bounds.size.x * ratio, 0.1f, 0f);
            var barCenter = new Vector3((collider.bounds.min.x + collider.bounds.max.x) / 2f - (collider.bounds.size.x * (1 - ratio) / 2f), top.y + 0.15f, 0f);

            Gizmos.color = Color.green;
            Gizmos.DrawCube(barCenter, barSize);
        }
    }
}
