using UnityEngine;

public class EnemyShooterAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;
    public float stoppingDistance = 6f; // Radius jaga jarak (Musuh ngerem di jarak ini)

    [Header("Combat Settings")]
    public GameObject enemyBulletPrefab; // Tarik prefab peluru musuh ke sini di Inspector
    public Transform shootPoint;         // Tarik objek ShootPoint ke sini di Inspector
    public float fireRate = 2f;          // Jeda waktu menembak (tiap 2 detik sekali)

    private Transform player;
    private Rigidbody2D rb;
    private float fireTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Mencari objek Player di dalam arena game
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // 1. Hitung jarak real-time dari posisi musuh ke Player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 2. Putar HANYA objek ShootPoint agar membidik ke arah Player
        RotateShootPointTowardsPlayer();

        // 3. Logika Jaga Jarak (AI Movement)
        if (distanceToPlayer > stoppingDistance)
        {
            // Jika Player masih jauh di luar radius, maju mendekat
            MoveTowardsPlayer();
        }
        else
        {
            // Jika sudah masuk radius jaga jarak, berhenti total dan mulai menembak
            rb.linearVelocity = Vector2.zero;
            HandleShooting();
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

    void HandleShooting()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireRate)
        {
            Shoot();
            fireTimer = 0f; // Reset waktu hitung mundur tembakan
        }
    }

    void Shoot()
    {
        if (enemyBulletPrefab != null && shootPoint != null)
        {
            // Instansiasi peluru tepat di posisi dan arah rotasi ShootPoint yang sedang membidik
            Instantiate(enemyBulletPrefab, shootPoint.position, shootPoint.rotation);
            Debug.Log("Musuh kedua menembak dengan akurat!");
        }
    }
}