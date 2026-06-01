using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float baseSpeed = 5f;
    [HideInInspector] public float currentSpeed; 

    [Header("Combat Settings")]
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public int baseDamage = 2;
    [HideInInspector] public int currentDamage;   

    [Header("Fire Rate Settings")]
    public float baseFireRate = 0.4f;
    [HideInInspector] public float currentFireRate; 
    private float nextFireTime;
    private bool isAttackPressed = false;

    [Header("Visual & Animation Settings")]
    [Tooltip("Tarik objek anak 'VisualKarakter' dari Hierarchy ke kolom ini")]
    public Transform visualKarakterTransform; 
    private Animator anim;

    private Rigidbody2D rb;
    private PlayerControl controls;
    private Vector2 moveInput; // Menyimpan live feed arah jalan

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        currentSpeed = baseSpeed;
        currentDamage = baseDamage;
        currentFireRate = baseFireRate;

        controls = new PlayerControl();

        // Setup input menembak dasar
        controls.Player.Attack.started += ctx => isAttackPressed = true;
        controls.Player.Attack.canceled += ctx => isAttackPressed = false;

        // =======================================================================
        // ADAPTASI VIDEO: MENGIKAT INPUT JALAN KE SISTEM ANIMASI (CALLBACK)
        // =======================================================================
        // Saat tombol WASD mulai ditekan atau sedang ditekan
        controls.Player.Move.performed += ctx => ProcessMovementInput(ctx);
        
        // Saat tombol WASD dilepas total (sprung back to center)
        controls.Player.Move.canceled += ctx => CancelMovementInput(ctx);
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Update()
    {
        AimAtCursor();

        if (isAttackPressed && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + currentFireRate;
        }
    }

    void FixedUpdate()
    {
        // Fisika pergerakan konstan membaca live feed moveInput
        rb.linearVelocity = moveInput * currentSpeed;
    }

    // Fungsi pembantu saat mendeteksi ada input jalan aktif
    private void ProcessMovementInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (anim != null)
        {
            // Set parameter boolean IsWalking menjadi true
            anim.SetBool("IsWalking", true);

            // Kirim live feed arah pergerakan ke parameter Blend Tree
            anim.SetFloat("InputX", moveInput.x);
            anim.SetFloat("InputY", moveInput.y);
        }
    }

    // Fungsi pembantu saat tombol dilepas (Sesuai dengan logika context.canceled di video)
    private void CancelMovementInput(InputAction.CallbackContext context)
    {
        if (anim != null)
        {
            // Matikan animasi berjalan
            anim.SetBool("IsWalking", false);

            // Ambil arah input terakhir TEPAT sebelum di-reset menjadi 0, 
            // lalu simpan ke parameter LastInputX & LastInputY milik Blend Tree Idle
            anim.SetFloat("LastInputX", moveInput.x);
            anim.SetFloat("LastInputY", moveInput.y);
        }

        // Reset total kecepatan fisik menjadi nol
        moveInput = Vector2.zero;
    }

    void AimAtCursor()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 10f));

        if (shootPoint != null)
        {
            Vector2 shootDir = (Vector2)mouseWorldPos - (Vector2)shootPoint.position;
            float shootAngle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
            shootPoint.rotation = Quaternion.Euler(0f, 0f, shootAngle);
        }

        // Mekanisme membalik badan (Kiri/Kanan) mengikuti posisi kursor Mouse di layar
        if (visualKarakterTransform != null)
        {
            if (mouseWorldPos.x > transform.position.x)
            {
                visualKarakterTransform.localScale = new Vector3(1f, 1f, 1f);
            }
            else
            {
                visualKarakterTransform.localScale = new Vector3(-1f, 1f, 1f);
            }
        }
    }

    void Shoot()
    {
        if (bulletPrefab != null && shootPoint != null)
        {
            GameObject bulletObj = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
            Bullet bulletScript = bulletObj.GetComponent<Bullet>();

            if (bulletScript != null)
            {
                bulletScript.damage = currentDamage;
            }
        }
    }
}