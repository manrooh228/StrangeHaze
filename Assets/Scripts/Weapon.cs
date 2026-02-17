using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private GameObject _bullet;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _playerTransform;

    private void Awake()
    {
        _playerTransform = GetComponentInParent<Player>().transform;
    }

    private void Update()
    {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        //float rotateZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(0f, 0f, rotateZ);

        
        FireInput();
    }

    private void FireInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(_bullet, _spawnPoint.position, _spawnPoint.rotation);
        }
    }

}