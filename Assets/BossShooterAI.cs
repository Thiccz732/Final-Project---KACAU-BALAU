using UnityEngine;

public class BossShooterAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 1.5f;           // Kecepatan Bos mengejar Player

    [Header("Combat Settings")]
    public GameObject bossBulletPrefab; // Tarik prefab peluru boss ke sini
    public Transform shootPoint;         // Titik pusat tembakan (bisa di tengah badan boss)
    public float fireRate = 3f;          // Jeda serangan antar pattern (misal tiap 3 detik)

    [Header("Pattern Settings")]
    public int bulletCount = 8;          // Jumlah total peluru dalam 1 kali lingkaran (misal 8, 12, atau 16)
    public float bulletSpeed = 5f;       // Kecepatan peluru boss

    private float fireTimer;
    private Transform player;
    private Rigidbody2D rb;

    // Variabel penampung batas panggung (Otomatis dicari oleh script)
    private Transform borderLeft;
    private Transform borderRight;
    private Transform borderTop;
    private Transform borderDown;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Cari Player di arena permainan
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // Kunci rotasi fisik Bos agar gambarnya selalu tegak lurus secara otomatis
        if (rb != null)
        {
            rb.freezeRotation = true;
        }

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
        // Timer otomatis berjalan tanpa memedulikan posisi player
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireRate)
        {
            ShootRadialPattern();
            fireTimer = 0f; // Reset timer
        }
    }

    void FixedUpdate()
    {
        // Logika bergerak mengejar Player secara perlahan
        if (player != null && rb != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;
        }

        if (borderLeft != null && borderRight != null && borderTop != null && borderDown != null)
        {
            // Mengunci posisi Bos murni di antara koordinat global dari objek border panggung
            float clampedX = Mathf.Clamp(transform.position.x, borderLeft.position.x, borderRight.position.x);
            float clampedY = Mathf.Clamp(transform.position.y, borderDown.position.y, borderTop.position.y);
            
            transform.position = new Vector2(clampedX, clampedY);
        }
    }

    void ShootRadialPattern()
    {
        if (bossBulletPrefab == null || shootPoint == null) return;

        // Menghitung jarak sudut antar peluru (360 derajat dibagi jumlah peluru)
        float angleStep = 360f / bulletCount;
        float angle = 0f;

        for (int i = 0; i < bulletCount; i++)
        {
            // 1. Hitung arah vektor menggunakan Sin dan Cos berdasarkan sudut saat ini
            float bulletDirX = Mathf.Cos(angle * Mathf.Deg2Rad);
            float bulletDirY = Mathf.Sin(angle * Mathf.Deg2Rad);

            Vector2 bulletMoveDirection = new Vector2(bulletDirX, bulletDirY).normalized;

            // 2. Tentukan rotasi peluru agar menghadap ke arah jalannya
            float bulletAngle = Mathf.Atan2(bulletMoveDirection.y, bulletDirX) * Mathf.Rad2Deg;
            Quaternion bulletRotation = Quaternion.Euler(0, 0, bulletAngle);

            // 3. Spawn peluru di posisi Shoot Point
            GameObject bullet = Instantiate(bossBulletPrefab, shootPoint.position, bulletRotation);

            // 4. Berikan kecepatan fisik ke peluru berdasarkan arah lingkaran tadi
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                bulletRb.linearVelocity = bulletMoveDirection * bulletSpeed;
            }

            // Tambahkan sudut untuk peluru berikutnya di dalam loop
            angle += angleStep;
        }

        Debug.Log("Boss mengeluarkan serangan lingkaran " + bulletCount + " arah!");
    }
}