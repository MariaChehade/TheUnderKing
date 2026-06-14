using UnityEngine;
using System.Collections.Generic;

public class Archer : MonoBehaviour
{
    [SerializeField]
    private GameObject arrowPrefab;

    [SerializeField]
    private float arrowZOffset = -0.1f;

    [SerializeField]
    private float shootCooldown = 2f;

    [SerializeField]
    private float shootRange = 15f;

    [SerializeField]
    private float baseForce = 10f;

    [SerializeField]
    private float forceVariation = 0.2f;

    [SerializeField]
    private float accuracyVariation = 0.15f;

    private float lastShootTime;
    private List<BasicEnemy> enemies = new List<BasicEnemy>();
    private Vector2 lastCalculatedVelocity;
    private BasicEnemy currentTarget;

    void Update()
    {
        UpdateEnemyList();
        UpdateTargetAndAim();

        if (Time.time - lastShootTime >= shootCooldown && currentTarget != null)
        {
            Shoot();
        }
    }

    private void UpdateEnemyList()
    {
        enemies.Clear();
        var allEnemies = FindObjectsByType<BasicEnemy>(FindObjectsSortMode.None);
        foreach (var enemy in allEnemies)
        {
            if (enemy == null)
            {
                continue;
            }

            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance <= shootRange)
            {
                enemies.Add(enemy);
            }
        }
    }

    private void UpdateTargetAndAim()
    {
        if (enemies.Count == 0)
        {
            currentTarget = null;
            return;
        }

        if (currentTarget == null || !enemies.Contains(currentTarget))
        {
            currentTarget = enemies[0];
        }

        if (currentTarget != null)
        {
            lastCalculatedVelocity = CalculateTrajectory(currentTarget.transform.position);
        }
    }

    private void Shoot()
    {
        if (currentTarget == null)
        {
            return;
        }

        var spawnPos = new Vector3(transform.position.x, transform.position.y, transform.position.z + arrowZOffset);
        var arrowInstance = Instantiate(arrowPrefab, spawnPos, Quaternion.identity);
        var arrow = arrowInstance.GetComponent<Arrow>();

        if (arrow == null)
        {
            Destroy(arrowInstance);
            return;
        }

        var velocity = CalculateTrajectory(currentTarget.transform.position);
        arrow.Launch(velocity);

        lastShootTime = Time.time;
    }

    private Vector2 CalculateTrajectory(Vector2 targetPosition)
    {
        var shooterPos = (Vector2)transform.position;
        var dx = targetPosition.x - shooterPos.x;
        var dy = targetPosition.y - shooterPos.y;

        if (Mathf.Abs(dx) < 0.1f)
            return Vector2.up * baseForce;

        var force = baseForce * (1f + Random.Range(-forceVariation, forceVariation));
        var accuracy = 1f - Random.Range(0f, accuracyVariation);
        var g = 9.81f;
        var v2 = force * force;
        var absDx = Mathf.Abs(dx);

        // Equação quadrática: a*tan²θ - absDx*tanθ + (dy + a) = 0
        // onde a = g*dx²/(2v²)
        var a = g * absDx * absDx / (2f * v2);
        var discriminant = absDx * absDx - 4f * a * (dy + a);

        float launchAngle;
        if (discriminant < 0)
        {
            // Alvo fora do alcance — força ângulo de 45°
            launchAngle = 45f * Mathf.Deg2Rad;
        }
        else
        {
            // Duas soluções: escolhe a trajetória mais baixa (mais rápida e precisa)
            var sqrtDisc = Mathf.Sqrt(discriminant);
            var tanFlat = (absDx - sqrtDisc) / (2f * a);
            var tanHigh = (absDx + sqrtDisc) / (2f * a);

            // Prefere ângulo baixo; se negativo, usa o alto
            var tanAngle = tanFlat > 0 ? tanFlat : tanHigh;
            launchAngle = Mathf.Atan(tanAngle);
            launchAngle = Mathf.Clamp(launchAngle, 15f * Mathf.Deg2Rad, 75f * Mathf.Deg2Rad);
        }

        var sign = Mathf.Sign(dx);
        var velocityX = Mathf.Cos(launchAngle) * force * sign * accuracy;
        var velocityY = Mathf.Sin(launchAngle) * force * accuracy;

        return new Vector2(velocityX, velocityY);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || currentTarget == null)
        {
            return;
        }

        var shooterPos = (Vector2)transform.position;
        var targetPos = (Vector2)currentTarget.transform.position;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(shooterPos, targetPos);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(targetPos, 0.5f);
    }
}
