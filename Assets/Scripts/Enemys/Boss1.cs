using UnityEngine;

public class Boss1 : BaseEnemy
{
    [SerializeField]
    private float stompDamageInterval = 0.75f;

    [SerializeField]
    private float stompRadius = 2.5f;

    [SerializeField]
    private LayerMask castleLayers;

    private float lastStompTime = float.MinValue;

    protected override void Awake()
    {
        base.Awake();
        moveSpeed *= 0.5f;
        maxHealth *= 10;
        currentHealth = maxHealth;
    }

    protected override void Move()
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

    protected override void TryStartAttack(Collider2D targetCollider)
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

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (targetCastle == null)
        {
            return;
        }

        if (Time.time - lastStompTime < stompDamageInterval)
        {
            return;
        }

        lastStompTime = Time.time;
        targetCastle.TakeDamage(damage * 3);
    }

    protected override void Die()
    {
        Debug.Log("Boss defeated");
        base.Die();
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (!Application.isPlaying)
        {
            return;
        }

        var collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            return;
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(collider.bounds.center, stompRadius);
    }
}
