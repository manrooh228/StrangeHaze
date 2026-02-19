using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] protected float speed;
    [SerializeField] protected int health;
    protected Transform player;


    [Header("FOV Settings")]
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    public LayerMask playerMask;
    public LayerMask obstacleMask;

    [SerializeField] private bool _canSeePlayer;


    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(FOVRoutine());
    }
    private IEnumerator FOVRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            FieldOfViewCheck();
        }
    }

    protected virtual void Update()
    {
        if (player == null) return;

        FieldOfViewCheck();

        if (_canSeePlayer)
        {
            MoveToPlayer();
        }
    }

    protected virtual void MoveToPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) Die();
    }

    //FOV ->
    private void FieldOfViewCheck()
    {
        Collider2D rangeCheck = Physics2D.OverlapCircle(transform.position, viewRadius, playerMask);

        if (rangeCheck != null)
        {
            Transform target = rangeCheck.transform;
            Vector2 directionToTarget = (target.position - transform.position).normalized;

            // Проверка угла: угол между "вперед" монстра и направлением на игрока
            if (Vector2.Angle(transform.right, directionToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector2.Distance(transform.position, target.position);

                // Raycast: пускаем луч. Если он НЕ ударился в препятствие, значит видим игрока
                if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
                    _canSeePlayer = true;
                }
                else { _canSeePlayer = false; }
            }
            else { _canSeePlayer = false; }
        }
        else if (_canSeePlayer) { _canSeePlayer = false; }
    }

    private void OnDrawGizmos()
    {
        // Рисуем радиус
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        // Рисуем линии угла обзора
        Vector3 viewAngleA = DirectionFromAngle(-viewAngle / 2);
        Vector3 viewAngleB = DirectionFromAngle(viewAngle / 2);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);

        if (_canSeePlayer)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }

    private Vector3 DirectionFromAngle(float angleInDegrees)
    {
        angleInDegrees += transform.eulerAngles.z;
        return new Vector3(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0);
    }


    protected virtual void Die() => Destroy(gameObject);
}
