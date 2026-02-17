using UnityEngine;
using UnityEngine.UIElements;

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
            // 1. Считаем направление от ствола к мышке в мировых координатах
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0; // Нам не нужна глубина в 2D

            Vector2 direction = (mousePos - _spawnPoint.position).normalized;

            // 2. Вычисляем угол для самой пули
            float bulletAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // 3. Создаем пулю и СРАЗУ задаем ей этот правильный угол
            Instantiate(_bullet, _spawnPoint.position, Quaternion.Euler(0, 0, bulletAngle));
        }
    }

}