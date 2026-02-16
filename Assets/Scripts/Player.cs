using UnityEngine;
using UnityEngine.VFX;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 moveInput;
    public VisualEffect vfxRenderer;

    [Header("Movement Details")]
    [SerializeField] private float moveSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();    
    }

    private void Update()
    {
        HandleInput();
        HandleMovement();
    }

    private void HandleInput() => moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

    private void HandleMovement()
    {
        rb.linearVelocity = moveInput.normalized * moveSpeed;

        vfxRenderer.SetVector3("ColliderPos", transform.position);
    }

   
}
