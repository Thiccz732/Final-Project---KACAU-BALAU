using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public GameObject bulletPrefab;
    public Transform shootPoint;

    private Rigidbody2D rb;

    // Pakai class yang sesuai dengan nama file Input Action Anda
    private PlayerControl controls;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Inisialisasi sesuai nama file .inputactions Anda
        controls = new PlayerControl();

        // Menghubungkan action "Attack" yang baru dibuat ke fungsi Shoot
        controls.Player.Attack.performed += ctx => Shoot();
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Update()
    {
        AimAtCursor();
    }

    void FixedUpdate()
    {
        // Membaca input gerak dari file PlayerControl
        Vector2 moveInput = controls.Player.Move.ReadValue<Vector2>();
        rb.linearVelocity = moveInput * speed;
    }

    void AimAtCursor()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 10f));

        Vector2 lookDir = (Vector2)mouseWorldPos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;

        rb.rotation = angle;
    }

    void Shoot()
    {
        if (bulletPrefab != null && shootPoint != null)
        {
            Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
        }
    }
}