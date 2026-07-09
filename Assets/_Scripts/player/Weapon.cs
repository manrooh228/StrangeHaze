using UnityEngine;
using UnityEngine.UIElements;
using Assets.Scripts.Service;

public class Weapon : MonoBehaviour
{
    [Header("Revolver Logic")]
    [SerializeField] private RevolverController _revolverController;
    [SerializeField] private GameObject _bullet;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Player player;
    private Transform _playerTransform;
    private InputHandler _inputHandler;
    [SerializeField] private int _damage;

    [Header("SFX Settings")]
    public AudioSource audioSource;
    public AudioClip revolverShot;
    public AudioClip revolverShotFail;

    public int Damage { get => _damage; set => _damage = value; }


    private void Awake()
    {
        _revolverController = GetComponentInParent<RevolverController>();
        player = FindAnyObjectByType<Player>();
        if (_playerTransform == null)
        {
            _playerTransform = player.transform;
        }
        _inputHandler = GetComponentInParent<InputHandler>();
    }

    private void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 directionToMouse = mousePos - _playerTransform.position;
        float rotateZ = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
        _playerTransform.rotation = Quaternion.Euler(0f, 0f, rotateZ);

        FireInput();
    }

    private void FireInput()
    {
        if (player.inventoryCanvas.activeSelf) return;

        if (_inputHandler.GetShootPressed())
        {
            if (_revolverController.RequestShot())
            {
                Instantiate(_bullet, _spawnPoint.position, _spawnPoint.rotation);
                audioSource.PlayOneShot(revolverShot);
            }
            else
            {
                audioSource.PlayOneShot(revolverShotFail);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        //if(_playerTransform != null)
        //{
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawWireSphere(_playerTransform.position, _deadzoneRadius);
        //}
    }
}