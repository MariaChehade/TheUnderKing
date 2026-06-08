using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 2f;

    [SerializeField]
    private int damage = 1;

    [SerializeField]
    private float attackCooldown = 1f;

    private bool isAttacking;
    private float lastAttackTime = float.MinValue;
    private Rigidbody2D rigidbody2d;
    private Collider2D enemyCollider;
    private Castle targetCastle;

    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<Collider2D>();

        var enemyLayer = LayerMask.NameToLayer("Enemy");
        if (enemyLayer != -1)
        {
            Physics2D.IgnoreLayerCollision(enemyLayer, enemyLayer, true);
        }
    }

    void FixedUpdate()
    {
        if (isAttacking)
        {
            StopMovement();
            TryAttackOnCooldown();
            return;
        }

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

    private void TryAttackOnCooldown()
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

    private void TryStartAttack(Collider2D targetCollider)
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

    private void StopAtCastle(Collider2D castleCollider)
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

    private void StopMovement()
    {
        if (rigidbody2d != null)
        {
            rigidbody2d.linearVelocity = Vector2.zero;
        }
    }

    public void TakeDamage(int arrowDamage)
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        var collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
    }
}