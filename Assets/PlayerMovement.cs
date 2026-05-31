using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float baseSpeed = 5f;
    [HideInInspector] public float currentSpeed; // Bisa diakses & diubah oleh script Skill

    [Header("Combat Settings")]
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public int baseDamage = 2;
    [HideInInspector] public int currentDamage;   // Bisa diakses & diubah oleh script Skill

    [Header("Fire Rate Settings")]
    public float baseFireRate = 0.4f;
    [HideInInspector] public float currentFireRate; // Bisa diakses & diubah oleh script Skill
    private float nextFireTime;
    private bool isAttackPressed = false;

    private Rigidbody2D rb;
    private PlayerControl controls;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Inisialisasi nilai awal standar
        currentSpeed = baseSpeed;
        currentDamage = baseDamage;
        currentFireRate = baseFireRate;

        controls = new PlayerControl();

        // Setup input menembak dasar
        controls.Player.Attack.started += ctx => isAttackPressed = true;
        controls.Player.Attack.canceled += ctx => isAttackPressed = false;
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Update()
    {
        AimAtCursor();

        // Mengatur jeda tembak otomatis secara real-time
        if (isAttackPressed && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + currentFireRate;
        }
    }

    void FixedUpdate()
    {
        Vector2 moveInput = controls.Player.Move.ReadValue<Vector2>();
        rb.linearVelocity = moveInput * currentSpeed;
    }

    void AimAtCursor()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 10f));

        Vector2 lookDir = (Vector2)mouseWorldPos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;

        rb.MoveRotation(angle);
    }

    void Shoot()
    {
        if (bulletPrefab != null && shootPoint != null)
        {
            GameObject bulletObj = Instantiate(bulletPrefab, shootPoint.position, transform.rotation);
            Bullet bulletScript = bulletObj.GetComponent<Bullet>();

            if (bulletScript != null)
            {
                bulletScript.damage = currentDamage;
            }
        }
    }
}