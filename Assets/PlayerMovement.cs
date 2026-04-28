using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public GameObject bulletPrefab; // Taruh prefab peluru di sini via Inspector
    public Transform shootPoint;  // Titik keluar peluru (ujung senjata/tengah player)

    private Rigidbody2D rb;
    private InputSystem_Actions controls;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controls = new InputSystem_Actions();

        // Mendaftarkan fungsi OnAttack saat tombol ditekan
        controls.Player.Attack.performed += ctx => Shoot();
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void FixedUpdate()
    {
        Vector2 moveInput = controls.Player.Move.ReadValue<Vector2>();
        rb.linearVelocity = moveInput * speed;
    }

    void Shoot()
    {
        if (bulletPrefab != null && shootPoint != null)
        {
            Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
        }
    }
}