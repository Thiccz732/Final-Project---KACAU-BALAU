using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float baseSpeed = 5f;
    private float currentSpeed;

    [Header("Combat Settings")]
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public int baseDamage = 1;
    private int bonusDamage = 0;

    private Rigidbody2D rb;
    private PlayerControl controls; // Menggunakan PlayerControl sesuai file kamu[cite: 4]

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = baseSpeed;
        controls = new PlayerControl();

        // Register Actions[cite: 4]
        controls.Player.Attack.performed += ctx => Shoot();

        // Register Skill Actions
        controls.Player.Skill1.performed += ctx => ActivateDamageBuff();
        controls.Player.Skill2.performed += ctx => ActivateSpeedBuff();
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Update()
    {
        AimAtCursor();
    }

    void FixedUpdate()
    {
        Vector2 moveInput = controls.Player.Move.ReadValue<Vector2>();
        rb.linearVelocity = moveInput * currentSpeed;
    }

    // SKILL 1: Buff Damage (+2), Nerf Speed (Setengahnya)
    void ActivateDamageBuff()
    {
        bonusDamage = 2;
        currentSpeed = baseSpeed * 0.5f;
        Debug.Log("Damage Buff Aktif: Damage naik, Speed turun!");
    }

    // SKILL 2: Buff Speed (Dua kali lipat), Nerf Damage (Kembali ke base)
    void ActivateSpeedBuff()
    {
        bonusDamage = 0;
        currentSpeed = baseSpeed * 2f;
        Debug.Log("Speed Buff Aktif: Speed naik, Damage normal!");
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
            GameObject bulletObj = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
            Bullet bulletScript = bulletObj.GetComponent<Bullet>();

            if (bulletScript != null)
            {
                // Mengirimkan total damage ke script peluru
                bulletScript.damage = baseDamage + bonusDamage;
            }
        }
    }
}