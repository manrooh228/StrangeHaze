using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _timeToDestroy;
    [SerializeField] private LayerMask obstacleMask;

    private float _time;
    private void Update()
    {
        _time += Time.deltaTime;
        if (_time > _timeToDestroy)
        {
            Destroy(gameObject);
        }
        
        transform.Translate(Vector2.right * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int damage = FindFirstObjectByType<Weapon>().Damage;

        Enemy enemy = collision.GetComponent<Enemy>();

        if (enemy != null)
        {
            Debug.Log("-1hp");
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }

        // Удалять пулю, если попала в стену (слой Obstacle)
        if (((1 << collision.gameObject.layer) & obstacleMask) != 0)
        {
            Destroy(gameObject);
        }
    }
}
