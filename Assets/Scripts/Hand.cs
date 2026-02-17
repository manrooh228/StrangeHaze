using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] private GameObject _weapon;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _playerTransform;

    private void Awake()
    {
        Instantiate(_weapon, _spawnPoint);
    }

    private void Update()
    {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotateZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        FlipPlayer(difference.x);

        if (_playerTransform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            //transform.rotation = Quaternion.Euler(180f, 0f, rotateZ);
        }
        else
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            //transform.rotation = Quaternion.Euler(0f, 0f, rotateZ);
        }
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
}
