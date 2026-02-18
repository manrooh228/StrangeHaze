using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _timeToDestroy;

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
}
