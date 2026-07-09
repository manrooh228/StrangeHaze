using UnityEngine;
using Assets.Scripts.Needs;
using Assets.Scripts.Service;
using StrangeHaze.Bootstrap;

// Состояние "без оружия" — игрок бьёт кулаком.
public class idle : MonoBehaviour
{
    [Header("Melee Punch")]
    [SerializeField] private int minDamage = 1;
    [SerializeField] private int maxDamage = 3;
    [SerializeField] private float attackDistance = 1f; // на каком расстоянии от игрока бьём
    [SerializeField] private float attackRadius = 0.6f;  // радиус зоны поражения удара
    [SerializeField] private float attackCooldown = 0.5f;

    private Player _player;
    private Transform _playerTransform;
    private InputHandler _inputHandler;
    private float _lastAttackTime;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
        _playerTransform = _player.transform;
        _inputHandler = GetComponentInParent<InputHandler>();
    }

    private void Update()
    {
        if (_player.inventoryCanvas.activeSelf) return;

        if (_inputHandler.GetShootPressed())
        {
            TryPunch();
        }
    }

    private void TryPunch()
    {
        if (Time.time < _lastAttackTime + attackCooldown) return;
        _lastAttackTime = Time.time;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = ((Vector2)mousePos - (Vector2)_playerTransform.position).normalized;
        Vector2 origin = (Vector2)_playerTransform.position + dir * attackDistance;

        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, attackRadius);
        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy == null) continue;

            // Урон рандомизирован (minDamage..maxDamage) и зависит от текущей стамины/усталости.
            int rawDamage = Random.Range(minDamage, maxDamage + 1);
            float multiplier = ServiceLocator.Get<IPlayerNeedsService>().GetDamageMultiplier();
            int finalDamage = Mathf.Max(1, Mathf.RoundToInt(rawDamage * multiplier));

            enemy.TakeDamage(finalDamage);
            break; // один удар — один враг
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_playerTransform == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_playerTransform.position, attackDistance + attackRadius);
    }
}
