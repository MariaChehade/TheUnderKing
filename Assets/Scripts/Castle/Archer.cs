using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : MonoBehaviour
{
    [SerializeField]
    private GameObject arrowPrefab;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private string attackStateName = "Atack";

    [SerializeField]
    private string idleStateName = "Idle";

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

    [SerializeField]
    [Tooltip("Momento normalizado da animação de ataque em que a flecha é disparada (0 = início, 1 = fim)")]
    private float shootNormalizedTime = 0.5f;

    private float lastShootTime;
    private List<BaseEnemy> enemies = new List<BaseEnemy>();
    private Vector2 lastCalculatedVelocity;
    private BaseEnemy currentTarget;
    private Coroutine attackAnimationRoutine;
    private bool isPlayingAttackAnimation;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Start()
    {
        // Garante que a animação idle é tocada ao iniciar, não a de ataque
        if (animator != null && !string.IsNullOrEmpty(idleStateName))
        {
            animator.Play(idleStateName, 0, 0f);
        }
    }

    void Update()
    {
        UpdateEnemyList();
        UpdateTargetAndAim();

        if (!isPlayingAttackAnimation && Time.time - lastShootTime >= shootCooldown && currentTarget != null)
        {
            Shoot();
        }
    }

    private void UpdateEnemyList()
    {
        enemies.Clear();
        var allEnemies = FindObjectsByType<BaseEnemy>(FindObjectsSortMode.None);
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

        lastShootTime = Time.time;

        if (attackAnimationRoutine != null)
        {
            StopCoroutine(attackAnimationRoutine);
        }

        attackAnimationRoutine = StartCoroutine(AttackAnimationRoutine());
    }

    private void FireArrow()
    {
        if (currentTarget == null || arrowPrefab == null)
        {
            return;
        }

        var velocity = lastCalculatedVelocity;
        if (velocity == Vector2.zero)
        {
            velocity = CalculateTrajectory(currentTarget.transform.position);
        }

        var spawnPos = new Vector3(transform.position.x, transform.position.y, transform.position.z + arrowZOffset);
        var arrowInstance = Instantiate(arrowPrefab, spawnPos, Quaternion.identity);
        var arrow = arrowInstance.GetComponent<Arrow>();

        if (arrow == null)
        {
            Destroy(arrowInstance);
            return;
        }

        arrow.Launch(velocity);
    }


    private IEnumerator AttackAnimationRoutine()
    {
        isPlayingAttackAnimation = true;

        if (animator != null && !string.IsNullOrEmpty(attackStateName))
        {
            animator.Play(attackStateName, 0, 0f);
        }

        float animationDuration = GetAnimationClipLength(attackStateName);
        if (animationDuration <= 0f)
        {
            animationDuration = shootCooldown;
        }

        // Aguarda até o momento configurado na animação para disparar a flecha
        float shootDelay = animationDuration * Mathf.Clamp01(shootNormalizedTime);
        yield return new WaitForSeconds(shootDelay);

        FireArrow();

        // Aguarda o restante da animação terminar
        float remainingTime = animationDuration - shootDelay;
        if (remainingTime > 0f)
        {
            yield return new WaitForSeconds(remainingTime);
        }

        if (animator != null && !string.IsNullOrEmpty(idleStateName))
        {
            animator.Play(idleStateName, 0, 0f);
        }

        isPlayingAttackAnimation = false;
        attackAnimationRoutine = null;
    }

    private float GetAnimationClipLength(string clipName)
    {
        if (animator == null || animator.runtimeAnimatorController == null || string.IsNullOrEmpty(clipName))
        {
            return 0f;
        }

        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip != null && clip.name == clipName)
            {
                return clip.length / Mathf.Max(0.01f, animator.speed);
            }
        }

        return 0f;
    }

    private Vector2 CalculateTrajectory(Vector2 targetPosition)
    {
        var shooterPos = (Vector2)transform.position;
        var dx = targetPosition.x - shooterPos.x;
        var dy = targetPosition.y - shooterPos.y;

        if (Mathf.Abs(dx) < 0.1f)
            return new Vector2(0, dy > 0 ? baseForce : -baseForce);

        var g = 9.81f;
        var force = baseForce * (1f + Random.Range(-forceVariation, forceVariation));
        var accuracy = 1f - Random.Range(0f, accuracyVariation);

        // Tempo estimado de voo baseado na distância horizontal
        // vx = force * cos(θ) ≈ force para ângulos baixos; usamos dx/force como estimativa inicial
        var tEstimate = Mathf.Abs(dx) / force;

        // Compensação de queda: quanto a gravidade vai puxar a flecha durante o voo
        // dy_needed = dy_real + 0.5 * g * t²   (subimos a mira para compensar a queda)
        var dyCompensated = dy + 0.5f * g * tEstimate * tEstimate;

        // Direção final com compensação de queda embutida
        var direction = new Vector2(dx, dyCompensated).normalized;

        var velocityX = direction.x * force * accuracy;
        var velocityY = direction.y * force * accuracy;

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
