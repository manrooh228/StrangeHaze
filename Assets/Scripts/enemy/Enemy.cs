using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Animator anim;

    [Header("Physics")]
    [SerializeField] protected float speed;
    [SerializeField] protected int health;
    protected Transform player;
    [SerializeField] private bool onoffGismos = true;

    [Header("FOV Settings")]
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    public LayerMask playerMask;
    public LayerMask obstacleMask;

    [SerializeField] private bool _canSeePlayer;

    [Header("Attack Settings")]
    [SerializeField] private float _attackRange = 1.5f;
    [SerializeField] private int _attackDamage = 1;
    [SerializeField] private float _attackCooldown = 1f;
    private float _lastAttackTime;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    protected virtual void Start()
    {
        // Лучше использовать FindWithTag, но с проверкой на null
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        // УБРАЛИ КОРУТИНУ FOVRoutine - она конфликтовала с Update
    }

    protected virtual void Update()
    {
        if (player == null) return;

        // 1. Сначала проверяем, видим ли мы игрока
        FieldOfViewCheck();

        // 2. Единая логика поведения
        if (_canSeePlayer)
        {
            PerformCombatLogic();
        }
        else
        {
            // Если игрока не видим - стоим (или тут можно добавить патруль)
            anim.SetBool("isMoving", false);
        }
    }

    private void PerformCombatLogic()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= _attackRange)
        {
            // Мы близко -> Стоим и Атакуем
            anim.SetBool("isMoving", false);

            // Поворачиваемся к игроку даже во время атаки (опционально, чтобы не бил спиной)
            RotateTowards(player.position);

            Attack();
        }
        else
        {
            // Мы далеко -> Идем к игроку
            MoveToPlayer();
        }
    }

    private void Attack()
    {
        // Проверка таймера
        if (Time.time >= _lastAttackTime + _attackCooldown)
        {
            anim.SetTrigger("Attack");
            if (player != null)
            {
                // Тут лучше использовать интерфейс IDamageable, но оставим как есть
                var playerScript = player.GetComponent<Player>();
                if (playerScript != null)
                    playerScript.TakeDamage(_attackDamage);

                _lastAttackTime = Time.time;
            }
        }
    }

    protected virtual void MoveToPlayer()
    {
        RotateTowards(player.position);

        anim.SetBool("isMoving", true);

        // Важно: MoveTowards более плавный для позиционирования, чем просто Translate с вектором
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }

    // Вынес поворот в отдельный метод, чтобы использовать его и при атаке
    private void RotateTowards(Vector3 targetPosition)
    {
        Vector2 direction = (targetPosition - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // Используем Lerp для плавного поворота, чтобы не было резких рывков (дерганий)
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, speed * 2 * Time.deltaTime);
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) Die();
    }

    private void FieldOfViewCheck()
    {
        // Сбрасываем флаг в начале проверки
        _canSeePlayer = false;

        Collider2D rangeCheck = Physics2D.OverlapCircle(transform.position, viewRadius, playerMask);

        if (rangeCheck != null)
        {
            Transform target = rangeCheck.transform;
            Vector2 directionToTarget = (target.position - transform.position).normalized;
            float distanceToTarget = Vector2.Distance(transform.position, target.position);

            // Угол к цели
            float angleToTarget = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;

            // Разница углов
            float deltaAngle = Mathf.DeltaAngle(transform.eulerAngles.z, angleToTarget);

            if (Mathf.Abs(deltaAngle) < viewAngle * 0.5f)
            {
                // Важно: Луч пускаем чуть дальше от центра врага, чтобы не попасть в свой коллайдер
                // Или убедись, что слой врага не входит в obstacleMask
                RaycastHit2D hit = Physics2D.Raycast(
                    transform.position,
                    directionToTarget,
                    distanceToTarget,
                    obstacleMask | playerMask);

                if (hit.collider != null)
                {
                    // Проверяем по слою (битовые операции)
                    if (((1 << hit.collider.gameObject.layer) & playerMask) != 0)
                    {
                        _canSeePlayer = true;
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (onoffGismos)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, viewRadius);

            Vector3 viewAngleA = DirectionFromAngle(-viewAngle / 2);
            Vector3 viewAngleB = DirectionFromAngle(viewAngle / 2);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
            Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackRange);

            if (_canSeePlayer && player != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, player.position);
            }
        }
    }

    private Vector3 DirectionFromAngle(float angleInDegrees)
    {
        angleInDegrees += transform.eulerAngles.z;
        return new Vector3(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0);
    }

    protected virtual void Die() => Destroy(gameObject);
}