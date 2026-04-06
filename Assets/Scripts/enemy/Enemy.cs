using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // ───────────────────────────────────────────────
    //  Состояния врага
    // ───────────────────────────────────────────────
    private enum State { Idle, Chase, Search }
    private State _currentState = State.Idle;

    // ───────────────────────────────────────────────
    //  Компоненты
    // ───────────────────────────────────────────────
    private Animator   _anim;
    private Rigidbody2D _rb;
    protected Transform  player;

    // ───────────────────────────────────────────────
    //  Физика / скорость
    // ───────────────────────────────────────────────
    [Header("Physics")]
    [SerializeField] protected float speed   = 3f;
    [SerializeField] protected int   health  = 5;

    // ───────────────────────────────────────────────
    //  Поле зрения (FOV)
    // ───────────────────────────────────────────────
    [Header("FOV Settings")]
    public float   viewRadius = 8f;
    [Range(0, 360)]
    public float   viewAngle  = 90f;
    public LayerMask playerMask;
    public LayerMask obstacleMask;

    // Кулдаун рейкаста (оптимизация — не каждый кадр)
    [SerializeField] private float _fovCheckInterval = 0.1f;
    private float _nextFovCheckTime;

    private bool _canSeePlayer;

    // ───────────────────────────────────────────────
    //  "Zomboid" механика — запомненная позиция
    // ───────────────────────────────────────────────
    private Vector2 _lastSeenPosition;
    // Флаг: дошли ли до lastSeenPosition
    private bool    _reachedLastSeen;

    // ───────────────────────────────────────────────
    //  Search state — таймер осмотра
    // ───────────────────────────────────────────────
    [Header("Search Settings")]
    [SerializeField] private float _searchDuration = 2.5f; // секунд "осмотра"
    private float _searchTimer;

    // ───────────────────────────────────────────────
    //  Атака
    // ───────────────────────────────────────────────
    [Header("Attack Settings")]
    [SerializeField] private float _attackRange    = 1.5f;
    [SerializeField] private int   _attackDamage   = 1;
    [SerializeField] private float _attackCooldown = 1f;
    private float _lastAttackTime;

    // ───────────────────────────────────────────────
    //  Gizmos
    // ───────────────────────────────────────────────
    [Header("Debug")]
    [SerializeField] private bool _showGizmos = true;

    // ═══════════════════════════════════════════════
    //  Unity lifecycle
    // ═══════════════════════════════════════════════

    private void Awake()
    {
        _anim = GetComponentInChildren<Animator>();
        _rb   = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    protected virtual void Update()
    {
        if (player == null) return;

        // ── Обновляем FOV с кулдауном (оптимизация) ──
        if (Time.time >= _nextFovCheckTime)
        {
            _nextFovCheckTime = Time.time + _fovCheckInterval;
            CheckFieldOfView();
        }

        // ── Машина состояний ──
        switch (_currentState)
        {
            case State.Idle:   HandleIdle();   break;
            case State.Chase:  HandleChase();  break;
            case State.Search: HandleSearch(); break;
        }
    }

    // FixedUpdate — движение через Rigidbody2D
    private void FixedUpdate()
    {
        // Скорость уже задаётся в Handle-методах через _rb.linearVelocity
    }

    // ═══════════════════════════════════════════════
    //  Обработчики состояний
    // ═══════════════════════════════════════════════

    // ── IDLE ──────────────────────────────────────
    private void HandleIdle()
    {
        SetVelocity(Vector2.zero);
        SetMovingAnimation(false);

        // Как только увидели игрока — переходим в Chase
        if (_canSeePlayer)
            TransitionTo(State.Chase);
    }

    // ── CHASE ─────────────────────────────────────
    private void HandleChase()
    {
        if (_canSeePlayer)
        {
            // Постоянно запоминаем последнюю видимую позицию
            _lastSeenPosition = player.position;
            _reachedLastSeen  = false;

            float dist = Vector2.Distance(transform.position, player.position);

            if (dist <= _attackRange)
            {
                // Достаточно близко — атакуем
                SetVelocity(Vector2.zero);
                SetMovingAnimation(false);
                RotateTowards(player.position);
                TryAttack();
            }
            else
            {
                // Бежим прямо к игроку
                MoveTowards(player.position);
            }
        }
        else
        {
            // Игрок скрылся — идём к lastSeenPosition (Zomboid-механика)
            float distToLast = Vector2.Distance(transform.position, _lastSeenPosition);

            if (distToLast > 0.3f)
            {
                MoveTowards(_lastSeenPosition);
            }
            else
            {
                // Дошли до точки — переходим в Search
                _reachedLastSeen = true;
                TransitionTo(State.Search);
            }
        }
    }

    // ── SEARCH ────────────────────────────────────
    private void HandleSearch()
    {
        SetVelocity(Vector2.zero);
        SetMovingAnimation(false);

        // Если снова увидели игрока — возобновляем Chase
        if (_canSeePlayer)
        {
            TransitionTo(State.Chase);
            return;
        }

        _searchTimer -= Time.deltaTime;
        if (_searchTimer <= 0f)
        {
            // Осмотрелись — никого нет, возвращаемся в Idle
            TransitionTo(State.Idle);
        }
    }

    // ═══════════════════════════════════════════════
    //  Переход между состояниями
    // ═══════════════════════════════════════════════

    private void TransitionTo(State newState)
    {
        if (_currentState == newState) return;

        _currentState = newState;

        if (newState == State.Search)
        {
            // Запускаем таймер осмотра с небольшим случайным разбросом
            _searchTimer = _searchDuration + Random.Range(-0.5f, 0.5f);
        }
    }

    // ═══════════════════════════════════════════════
    //  FOV — проверка видимости
    // ═══════════════════════════════════════════════

    private void CheckFieldOfView()
    {
        _canSeePlayer = false;

        // 1. Быстрая проверка по радиусу
        Collider2D rangeHit = Physics2D.OverlapCircle(transform.position, viewRadius, playerMask);
        if (rangeHit == null) return;

        Transform target = rangeHit.transform;
        Vector2 dirToTarget = (target.position - transform.position).normalized;

        // 2. Проверяем угол обзора
        float angleToTarget = Mathf.Atan2(dirToTarget.y, dirToTarget.x) * Mathf.Rad2Deg;
        float deltaAngle    = Mathf.DeltaAngle(transform.eulerAngles.z, angleToTarget);

        if (Mathf.Abs(deltaAngle) > viewAngle * 0.5f) return;

        // 3. Raycast для проверки препятствий (Line of Sight)
        float dist = Vector2.Distance(transform.position, target.position);
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            dirToTarget,
            dist,
            obstacleMask | playerMask);

        if (hit.collider == null) return;

        // Попали именно в игрока (не в стену)?
        bool hitPlayer = ((1 << hit.collider.gameObject.layer) & playerMask) != 0;
        if (hitPlayer)
        {
            _canSeePlayer = true;

            // Если только что обнаружили — переходим из Idle/Search в Chase
            if (_currentState != State.Chase)
                TransitionTo(State.Chase);
        }
    }

    // ═══════════════════════════════════════════════
    //  Движение через Rigidbody2D
    // ═══════════════════════════════════════════════

    private void MoveTowards(Vector2 target)
    {
        RotateTowards(target);
        SetMovingAnimation(true);

        Vector2 direction = ((Vector2)target - (Vector2)transform.position).normalized;
        SetVelocity(direction * speed);
    }

    private void SetVelocity(Vector2 vel)
    {
        _rb.linearVelocity = vel;
    }

    // ═══════════════════════════════════════════════
    //  Поворот к цели
    // ═══════════════════════════════════════════════

    private void RotateTowards(Vector2 targetPosition)
    {
        Vector2 dir   = (targetPosition - (Vector2)transform.position).normalized;
        float   angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion targetRot = Quaternion.Euler(0f, 0f, angle);
        transform.rotation   = Quaternion.Lerp(transform.rotation, targetRot, speed * 2f * Time.deltaTime);
    }

    // ═══════════════════════════════════════════════
    //  Атака
    // ═══════════════════════════════════════════════

    private void TryAttack()
    {
        if (Time.time < _lastAttackTime + _attackCooldown) return;

        _anim.SetTrigger("Attack");
        _lastAttackTime = Time.time;

        var playerScript = player.GetComponent<Player>();
        if (playerScript != null)
            playerScript.TakeDamage(_attackDamage);
    }

    // ═══════════════════════════════════════════════
    //  Здоровье
    // ═══════════════════════════════════════════════

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) Die();
    }

    protected virtual void Die()
    {
        _rb.linearVelocity = Vector2.zero;
        Destroy(gameObject);
    }

    // ═══════════════════════════════════════════════
    //  Анимации
    // ═══════════════════════════════════════════════

    private void SetMovingAnimation(bool isMoving)
    {
        if (_anim != null)
            _anim.SetBool("isMoving", isMoving);
    }

    // ═══════════════════════════════════════════════
    //  Gizmos — визуализация FOV в редакторе
    // ═══════════════════════════════════════════════

    private void OnDrawGizmos()
    {
        if (!_showGizmos) return;

        // Радиус обзора
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        // Линии конуса FOV
        Vector3 leftBound  = DirectionFromAngle(-viewAngle / 2f);
        Vector3 rightBound = DirectionFromAngle( viewAngle / 2f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + leftBound  * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightBound * viewRadius);

        // Дуга FOV (опционально: несколько сегментов)
        int   segments  = 20;
        float stepAngle = viewAngle / segments;
        for (int i = 0; i < segments; i++)
        {
            Vector3 from = DirectionFromAngle(-viewAngle / 2f + stepAngle * i);
            Vector3 to   = DirectionFromAngle(-viewAngle / 2f + stepAngle * (i + 1));
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
            Gizmos.DrawLine(
                transform.position + from * viewRadius,
                transform.position + to   * viewRadius);
        }

        // Радиус атаки
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);

        // Луч до игрока (зелёный = видит, красный = нет)
        if (player != null)
        {
            Gizmos.color = _canSeePlayer ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, player.position);
        }

        // Последняя запомненная позиция игрока
        if (_currentState == State.Chase || _currentState == State.Search)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(_lastSeenPosition, 0.3f);
            Gizmos.DrawLine(transform.position, _lastSeenPosition);
        }

        // Текущее состояние — цветная рамка
#if UNITY_EDITOR
        UnityEditor.Handles.color = _currentState switch
        {
            State.Idle   => Color.gray,
            State.Chase  => Color.red,
            State.Search => Color.cyan,
            _            => Color.white
        };
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 1.2f,
            _currentState.ToString());
#endif
    }

    private Vector3 DirectionFromAngle(float angleInDegrees)
    {
        // Учитываем текущий поворот объекта
        float totalAngle = angleInDegrees + transform.eulerAngles.z;
        return new Vector3(
            Mathf.Cos(totalAngle * Mathf.Deg2Rad),
            Mathf.Sin(totalAngle * Mathf.Deg2Rad),
            0f);
    }
}
