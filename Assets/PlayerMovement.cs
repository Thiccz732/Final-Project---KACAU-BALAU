using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using TMPro; // Wajib ditambahkan agar bisa mengontrol teks UI

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float baseSpeed = 5f;
    private float currentSpeed;

    [Header("Combat Settings")]
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public int baseDamage = 2;
    private int currentDamage;

    [Header("Skill Status")]
    private bool isSkillActive = false;
    private bool isCooldown = false;

    [Header("UI Settings")]
    public TextMeshProUGUI cooldownText; // Tarik objek Text (TMP) dari Canvas ke kolom ini di Inspector

    private Rigidbody2D rb;
    private PlayerControl controls;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Inisialisasi nilai awal
        currentSpeed = baseSpeed;
        currentDamage = baseDamage;

        controls = new PlayerControl();

        // Input Setup
        controls.Player.Attack.performed += ctx => Shoot();

        // Skill 1 (Dmg 3, Slow) dan Skill 2 (Dmg 1, Fast)
        controls.Player.Skill1.performed += ctx => StartCoroutine(HandleSkill(3, 0.5f));
        controls.Player.Skill2.performed += ctx => StartCoroutine(HandleSkill(1, 2.0f));
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Update()
    {
        // Rotasi visual dilakukan di Update agar mulus mengikuti mouse
        AimAtCursor();
    }

    void FixedUpdate()
    {
        // Pergerakan fisik menggunakan Velocity agar tidak overlapping dengan musuh
        Vector2 moveInput = controls.Player.Move.ReadValue<Vector2>();
        rb.linearVelocity = moveInput * currentSpeed;
    }

    void AimAtCursor()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        // Z set ke 10 agar berada di jangkauan kamera
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 10f));

        Vector2 lookDir = (Vector2)mouseWorldPos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;

        // Menggunakan MoveRotation agar sinkron dengan sistem fisika
        rb.MoveRotation(angle);
    }

    IEnumerator HandleSkill(int targetDamage, float speedMultiplier)
    {
        // Cegah penggunaan jika masih aktif atau sedang cooldown
        if (isSkillActive || isCooldown) yield break;

        // --- AKTIFKAN SKILL (Durasi 5 detik) ---
        isSkillActive = true;
        currentDamage = targetDamage;
        currentSpeed = baseSpeed * speedMultiplier;

        if (cooldownText != null)
        {
            cooldownText.text = "SKILL ACTIVE!";
            cooldownText.color = Color.cyan; // Warna saat skill aktif
        }

        Debug.Log("Skill Aktif!");

        yield return new WaitForSeconds(5f);

        // --- RESET KE NORMAL & MULAI COOLDOWN ---
        currentDamage = baseDamage;
        currentSpeed = baseSpeed;
        isSkillActive = false;
        isCooldown = true;

        Debug.Log("Cooldown Dimulai...");

        // Logika Hitung Mundur Cooldown (5 detik)
        float timer = 5f;
        while (timer > 0)
        {
            if (cooldownText != null)
            {
                cooldownText.text = "COOLDOWN: " + timer.ToString("F1") + "s";
                cooldownText.color = Color.red; // Warna saat cooldown
            }

            yield return new WaitForSeconds(0.1f);
            timer -= 0.1f;
        }

        // --- SELESAI COOLDOWN ---
        isCooldown = false;
        if (cooldownText != null)
        {
            cooldownText.text = "READY!";
            cooldownText.color = Color.green; // Warna saat siap digunakan
        }

        Debug.Log("Skill Ready!");

        // Opsional: Hilangkan teks setelah 1 detik Ready
        yield return new WaitForSeconds(1f);
        if (!isSkillActive && !isCooldown && cooldownText != null)
            cooldownText.text = "";
    }

    void Shoot()
    {
        if (bulletPrefab != null && shootPoint != null)
        {
            // Ambil posisi dan rotasi tepat di moncong senjata (shootPoint)
            GameObject bulletObj = Instantiate(bulletPrefab, shootPoint.position, transform.rotation);
            Bullet bulletScript = bulletObj.GetComponent<Bullet>();

            if (bulletScript != null)
            {
                bulletScript.damage = currentDamage;
            }
        }
    }
}