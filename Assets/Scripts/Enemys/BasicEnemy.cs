using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 2f;

    [SerializeField]
    private int damage = 1;

    private bool isAttacking;
    private Rigidbody2D rigidbody2d;
    private Collider2D enemyCollider;

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryAttackCastle(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryAttackCastle(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryAttackCastle(collision.collider);
    }

    private void TryAttackCastle(Collider2D targetCollider)
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

        isAttacking = true;
        StopAtCastle(targetCollider);
        castle.TakeDamage(damage);
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
}
