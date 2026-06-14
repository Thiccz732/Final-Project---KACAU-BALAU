using UnityEngine;

public class EnemyShooterAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;
    public float stoppingDistance = 6f; // Radius jaga jarak (Musuh ngerem di jarak ini)

    [Header("Combat Settings")]
    public GameObject enemyBulletPrefab; 
    public Transform shootPoint;        
    public float fireRate = 2f;          
    public float bulletSpeed = 10f;
    public float bulletLifeTime = 5f;

    [Header("Target Settings")]
    [Tooltip("Tarik objek Player dari Hierarchy ke kolom ini")]
    public Transform player;             // Kita ubah jadi public agar bisa di-drag lewat Inspector

    private Rigidbody2D rb;
    private float fireTimer;

    // Variabel penampung batas panggung (Otomatis dicari oleh script)
    private Transform borderLeft;
    private Transform borderRight;
    private Transform borderTop;
    private Transform borderDown;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogError("PERINGATAN: Objek Player belum dimasukkan ke Inspector musuh!");
            }
        }

        // Border
        GameObject leftObj = GameObject.Find("Left");
        if (leftObj != null) borderLeft = leftObj.transform;

        GameObject rightObj = GameObject.Find("Right");
        if (rightObj != null) borderRight = rightObj.transform;

        GameObject topObj = GameObject.Find("Top");
        if (topObj != null) borderTop = topObj.transform;

        GameObject downObj = GameObject.Find("Down");
        if (downObj != null) borderDown = downObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        // Timer menembak terus berjalan di latar belakang
        fireTimer += Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // 1. Hitung jarak real-time dari posisi musuh ke Player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 2. Putar HANYA objek ShootPoint agar membidik ke arah Player
        RotateShootPointTowardsPlayer();

        // 3. Logika Jaga Jarak (AI Movement) & Menembak
        if (distanceToPlayer > stoppingDistance)
        {
            // Jika Player masih jauh di luar radius, maju mendekat
            MoveTowardsPlayer();
        }
        else
        {
            // Jika sudah masuk radius jaga jarak, berhenti total
            rb.linearVelocity = Vector2.zero;

            // Eksekusi menembak jika timer sudah mencukupi
            if (fireTimer >= fireRate)
            {
                Shoot();
                fireTimer = 0f; // Reset timer setelah menembak
            }
        }

        // Kunci border
        if (borderLeft != null && borderRight != null && borderTop != null && borderDown != null)
        {
            // Mengunci posisi musuh penembak murni di antara koordinat global tembok pembatas
            float clampedX = Mathf.Clamp(transform.position.x, borderLeft.position.x, borderRight.position.x);
            float clampedY = Mathf.Clamp(transform.position.y, borderDown.position.y, borderTop.position.y);
            
            transform.position = new Vector2(clampedX, clampedY);
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    void RotateShootPointTowardsPlayer()
    {
        if (shootPoint != null)
        {
            // Hitung sudut arah dari ShootPoint menuju koordinat Player
            Vector2 lookDir = (Vector2)player.position - (Vector2)shootPoint.position;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;

            // Putar objek ShootPoint secara mandiri di sumbu Z
            shootPoint.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void Shoot()
    {
        if (enemyBulletPrefab != null && shootPoint != null)
        {
            GameObject bulletObj = Instantiate(enemyBulletPrefab, shootPoint.position, shootPoint.rotation);
            Rigidbody2D bulletRb = bulletObj.GetComponent<Rigidbody2D>();
            
            if (bulletRb != null)
            {
                bulletRb.linearVelocity = shootPoint.right * bulletSpeed;
            }

            // UBAH BARIS INI: Ganti 3f menjadi variabel bulletLifeTime
            Destroy(bulletObj, bulletLifeTime); 
        }
    }
}