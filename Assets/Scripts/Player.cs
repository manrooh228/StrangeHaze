using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.VFX;

public class Player : MonoBehaviour
{
    [SerializeField] private Animator anim;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    public VisualEffect vfxRenderer;
    public GameObject inventoryCanvas;

    [Header("Cursor Settings")]
    [SerializeField] private Texture2D _crosshairTexture;
    [SerializeField] private Vector2 _hotspot = new Vector2(12.5f, 12.5f); //25*25

    [Header("Physics")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private int health;
    [SerializeField] private int maxHealth = 10;

    [Header("SFX Settings")]
    public AudioSource audioSource;
    public AudioClip[] stepSounds;
    public float stepInterval = 0.5f;
    [Range(0.1f, 0.5f)] public float pitchRange = 0.2f;

    public AudioClip inventoryOpenSound;
    public AudioClip inventoryCloseSound;

    [Header("Post Processing")]
    public Volume volume;
    private Vignette vignette;

    private float stepTimer;

    [Header("Weapon")]
    [SerializeField] private bool haveWeapon = true;
    [SerializeField] private GameObject weapon;
    [SerializeField] private GameObject idle;

    public int Health { get => health; set => health = value; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        haveWeaponCheck();
        UpdateCursor();
    }

    private void Start()
    {
        health = maxHealth;

        if (volume.profile.TryGet<Vignette>(out var tmpVignette))
        {
            vignette = tmpVignette;
            vignette.intensity.value = 0.5f;
        }
    }
    private void haveWeaponCheck()
    {
        if (haveWeapon)
        {
            Instantiate(weapon, gameObject.transform);
        }
        else
        {
            Instantiate(idle, gameObject.transform);
        }
    }

    private void Update()
    {
        if (!haveWeapon)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 directionToMouse = mousePos - transform.position;
            float rotateZ = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rotateZ);
        }

        if (!anim)
        {
            anim = GetComponentInChildren<Animator>();
        }

        HandleInput();
        HandleAnimations();
        HandleMovement();
        toggleInventory();
    }


    private void toggleInventory()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            bool isActive = !inventoryCanvas.activeSelf;
            inventoryCanvas.SetActive(isActive);

            if (isActive)
            {
                Time.timeScale = 0f;
                Cursor.visible = true;
                //audioSource.PlayOneShot(inventoryOpenSound);
                //Cursor.lockState = CursorLockMode.None;

                Assets.Scripts.weapons.InventoryManager.Instance.RefreshInventoryUI();
                
            }
            else
            {
                Time.timeScale = 1f;
                //audioSource.PlayOneShot(inventoryCloseSound);
                Cursor.visible = false;
                //Cursor.lockState = CursorLockMode.Locked;
                
            }

            UpdateCursor();
        }
    }

    private void HandleAnimations()
    {
        bool isMoving;

        if (rb.linearVelocity != Vector2.zero)
            isMoving = true;
        else
            isMoving = false;

        anim.SetBool("isMoving", isMoving);
    }

    private void HandleInput() => moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

    private void HandleMovement()
    {
        rb.linearVelocity = moveInput.normalized * moveSpeed;

        vfxRenderer.SetVector3("ColliderPos", transform.position);

        // Логика звуков шагов
        if (moveInput.sqrMagnitude > 0.1f)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0)
            {
                PlayStepSound();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0; // Сбрасываем таймер при остановке
        }
    }

    private void PlayStepSound()
    {
        if (stepSounds.Length == 0 || audioSource == null) return;

        AudioClip clip = stepSounds[Random.Range(0, stepSounds.Length)];

        audioSource.pitch = 1.0f + Random.Range(-pitchRange, pitchRange);

        audioSource.PlayOneShot(clip);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        float healthPercent = (float)health / (float)maxHealth;

        if (vignette != null)
        {
            // Цвет плавно переходит от черного к красному
            // Когда healthPercent = 1 (полное ХП), цвет будет black
            // Когда healthPercent = 0 (нет ХП), цвет будет red
            Color safeColor = Color.black;
            Color dangerColor = Color.red;

            vignette.color.value = Color.Lerp(dangerColor, safeColor, healthPercent);

            // Дополнительно: можно чуть-чуть увеличивать интенсивность 
            // от 0.2 (здоровье) до 0.5 (при смерти)
            vignette.intensity.value = Mathf.Lerp(0.7f, 0.5f, healthPercent);
        }

        if (health <= 0) Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    public void UnlockWeapon ()
    {
        Destroy(GetComponentInChildren<Animator>().gameObject);
        haveWeapon = true;
        haveWeaponCheck();
        UpdateCursor();
    }

    private void UpdateCursor()
    {
        if (inventoryCanvas.activeSelf)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            Cursor.visible = true;
        }

        else if (haveWeapon)
        {
            Cursor.SetCursor(_crosshairTexture, _hotspot, CursorMode.Auto);
            Cursor.visible = true;
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            Cursor.visible = false;
        }
    }
}

