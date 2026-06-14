using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private float colliderRadius = 0.05f; // raio aproximado do collider da flecha

    private Vector2 velocity;
    private readonly Vector2 gravity = Vector2.down * 9.81f;
    private bool hasHit;
    private float spawnTime;
    private float hitTime;

    public void Launch(Vector2 initialVelocity)
    {
        velocity = initialVelocity;
        hasHit = false;
        spawnTime = Time.time;
    }

    void Update()
    {
        if (hasHit)
        {
            if (Time.time - hitTime > lifetime)
                Destroy(gameObject);
            return;
        }

        if (Time.time - spawnTime > lifetime)
        {
            Destroy(gameObject);
            return;
        }

        velocity += gravity * Time.deltaTime;

        Vector2 origin = transform.position;
        Vector2 movement = velocity * Time.deltaTime;
        float distance = movement.magnitude;
        Vector2 direction = movement.normalized;

        // ── Raycast cobre TODO o trajeto do frame: sem tunneling ──────────────
        RaycastHit2D groundHit = Physics2D.CircleCast(origin, colliderRadius, direction, distance, groundLayers);
        if (groundHit.collider != null)
        {
            StickToSurface(groundHit);
            return;
        }

        // Verifica inimigos no mesmo trajeto
        RaycastHit2D enemyHit = Physics2D.CircleCast(origin, colliderRadius, direction, distance, enemyLayers);
        if (enemyHit.collider != null)
        {
            var enemy = enemyHit.collider.GetComponentInParent<BaseEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }
        }

        // Move normalmente se não houve colisão
        transform.position += (Vector3)movement;

        // Rotaciona na direção do voo
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void StickToSurface(RaycastHit2D hit)
    {
        hasHit = true;
        hitTime = Time.time;
        velocity = Vector2.zero;

        // Posiciona a flecha exatamente no ponto de contato
        transform.position = hit.centroid;

        // Parenteia ao chão para seguir plataformas móveis
        transform.SetParent(hit.collider.transform);

        // Desativa o collider da flecha para não re-triggar nada
        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }

    // OnTriggerEnter2D removido — detecção agora é 100% por Raycast no Update

    void OnDrawGizmos()
    {
        if (!Application.isPlaying || hasHit) return;

        Gizmos.color = Color.yellow;
        var pos = (Vector2)transform.position;
        var vel = velocity;

        for (int i = 0; i < 20; i++)
        {
            vel += Vector2.down * 9.81f * 0.05f;
            var next = pos + vel * 0.05f;
            Gizmos.DrawLine(pos, next);
            pos = next;
        }
    }
}