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

    // Sesuai permintaan: Normal damage adalah 2
    public int baseDamage = 2;
    private int currentDamage;

    private Rigidbody2D rb;
    private PlayerControl controls; // Menggunakan class dari PlayerControl.cs

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Inisialisasi nilai awal
        currentSpeed = baseSpeed;
        currentDamage = baseDamage;

        controls = new PlayerControl();

        // Mendaftarkan fungsi input
        controls.Player.Attack.performed += ctx => Shoot();

        // Skill 1 (Tombol 1) dan Skill 2 (Tombol 2)
        controls.Player.Skill1.performed += ctx => ActivateSkill1();
        controls.Player.Skill2.performed += ctx => ActivateSkill2();
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Update()
    {
        AimAtCursor();
    }

    void FixedUpdate()
    {
        // Pergerakan berdasarkan currentSpeed yang dinamis
        Vector2 moveInput = controls.Player.Move.ReadValue<Vector2>();
        rb.linearVelocity = moveInput * currentSpeed;
    }

    // SKILL 1: Damage naik jadi 3, Speed turun (Slow Down)
    void ActivateSkill1()
    {
        currentDamage = 3;
        currentSpeed = baseSpeed * 0.5f; // Kecepatan jadi setengah
        Debug.Log("Skill 1 Aktif: Damage 3, Speed Lambat");
    }

    // SKILL 2: Damage turun jadi 1, Speed naik (Speed Up)
    void ActivateSkill2()
    {
        currentDamage = 1;
        currentSpeed = baseSpeed * 2f; // Kecepatan jadi dua kali lipat
        Debug.Log("Skill 2 Aktif: Damage 1, Speed Cepat");
    }

    // Rotasi player menghadap kursor mouse
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
                // Mengirimkan nilai currentDamage ke peluru
                bulletScript.damage = currentDamage;
            }
        }
    }
}