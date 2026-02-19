using UnityEngine;
using UnityEngine.UIElements;

public class Weapon : MonoBehaviour
{
    [SerializeField] private GameObject _bullet;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _deadzoneRadius = 3f;
    [SerializeField] private int _damage;

    public int Damage { get => _damage; set => _damage = value; }

    private void Awake()
    {
        _playerTransform = GetComponentInParent<Player>().transform;
    }

    private void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        float distanceToPlayer = Vector3.Distance(_playerTransform.position, mousePos);

        if (distanceToPlayer < _deadzoneRadius)
        {
            FireInput();
            return;
        }

        Vector3 directionToMouse = mousePos - transform.position;
        float rotateZ = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, rotateZ);

        Vector3 localScale = transform.localScale;
        if (directionToMouse.x < 0)
        {
            transform.Rotate(180, 180, 0);
        }
        else
        {
            transform.Rotate(0, 0, 0);
        }
        transform.localScale = localScale;

        FireInput();
    }

    private void FireInput()
    {

        bool _canShoot = GetComponentInParent<Hand>().getCanShoot();

        if (Input.GetMouseButtonDown(0) && _canShoot)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            Vector2 direction = (mousePos - _spawnPoint.position).normalized;

            float bulletAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Instantiate(_bullet, _spawnPoint.position, Quaternion.Euler(0, 0, bulletAngle));
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_playerTransform.position, _deadzoneRadius);
    }
}