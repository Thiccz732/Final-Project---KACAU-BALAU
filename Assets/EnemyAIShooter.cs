using UnityEngine;

public class EnemyShooterAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;
    public float stoppingDistance = 6f; // Radius jaga jarak (Musuh ngerem di jarak ini)

    [Header("Combat Settings")]
    public GameObject enemyBulletPrefab; // Tarik prefab peluru musuh ke sini
    public Transform shootPoint;         // Tarik objek moncong senjata ke sini
    public float fireRate = 2f;          // Menembak setiap 2 detik sekali

    private Transform player;
    private Rigidbody2D rb;
    private float fireTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Cari Player di arena game
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // 1. Hitung jarak real-time ke Player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 2. Putar tubuh musuh agar selalu menghadap Player
        RotateTowardsPlayer();

        // 3. Logika Jaga Jarak
        if (distanceToPlayer > stoppingDistance)
        {
            // Jika masih jauh, maju mendekat
            MoveTowardsPlayer();
        }
        else
        {
            // Jika sudah masuk radius tertentu, BERHENTI (kecepatan = 0) dan tembak!
            rb.linearVelocity = Vector2.zero;
            HandleShooting();
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    void RotateTowardsPlayer()
    {
        Vector2 lookDir = (Vector2)player.position - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        rb.MoveRotation(angle);
    }

    void HandleShooting()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireRate)
        {
            Shoot();
            fireTimer = 0f;
        }
    }

    void Shoot()
    {
        if (enemyBulletPrefab != null && shootPoint != null)
        {
            // Munculkan peluru tepat di koordinat shootPoint
            Instantiate(enemyBulletPrefab, shootPoint.position, transform.rotation);
            Debug.Log("Musuh kedua menembak!");
        }
    }
}