using UnityEngine;
using UnityEngine.UIElements;

public class Hand : MonoBehaviour
{
    [SerializeField] private GameObject _weapon;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _deadzoneRadius = 0.5f;

    private void Awake()
    {
        Instantiate(_weapon, _spawnPoint);
    }

    private void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector3 difference = mousePos - transform.position;

        if (difference.magnitude < _deadzoneRadius) return;

        float rotateZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        FlipPlayer(difference.x);

        transform.rotation = Quaternion.Euler(0f, 0f, rotateZ);

        Vector3 localScale = transform.localScale;
        if (difference.x < 0)
        {
            transform.Rotate(180, 180, 0);
        }
        else
        {
            transform.Rotate(0, 0, 0);
        }
        transform.localScale = localScale;
    }

    private void FlipPlayer(float horizontalDirection)
    {
        Vector3 scaler = _playerTransform.localScale;

        if (horizontalDirection > 0)
        {
            scaler.x = Mathf.Abs(scaler.x);
            
        }

        else if (horizontalDirection < 0)
        {
            scaler.x = -Mathf.Abs(scaler.x);
        }

        _playerTransform.localScale = scaler;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _deadzoneRadius);
    }
}
