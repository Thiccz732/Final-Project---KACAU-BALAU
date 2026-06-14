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

    [Header("Audio Settings")]
    // SEKARANG BERUBAH MENJADI ARRAY [] UNTUK MENAMPUNG BANYAK NADA
    public AudioClip[] sfxTembakan;    // Slot untuk file-file audio suara tembakan (bisa diisi 3 nada berbeda)
    public AudioSource audioSource;    // Mulut yang akan membunyikan suaranya

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
        controls.Player.Move.performed += ctx => ProcessMovementInput(ctx);
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
        rb.linearVelocity = moveInput * currentSpeed;
    }

    private void ProcessMovementInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (anim != null)
        {
            anim.SetBool("IsWalking", true);
            anim.SetFloat("InputX", moveInput.x);
            anim.SetFloat("InputY", moveInput.y);
        }
    }

    private void CancelMovementInput(InputAction.CallbackContext context)
    {
        if (anim != null)
        {
            anim.SetBool("IsWalking", false);
            anim.SetFloat("LastInputX", moveInput.x);
            anim.SetFloat("LastInputY", moveInput.y);
        }

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

            // ========================================================
            // LOGIKA BARU: MEMILIH NADA SECARA RANDOM
            // ========================================================
            if (sfxTembakan != null && sfxTembakan.Length > 0 && audioSource != null)
            {
                // Memilih index acak dari isi array sfxTembakan
                int randomIndex = Random.Range(0, sfxTembakan.Length);

                // Mainkan suara terpilih tanpa memutus suara yang sedang berjalan
                audioSource.PlayOneShot(sfxTembakan[randomIndex]);
            }
        }
    }
}   