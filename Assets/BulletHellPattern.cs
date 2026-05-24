using UnityEngine;

public class BulletHellPattern : MonoBehaviour
{
    public enum PatternType { Spiral, WaveSpiral, CrossExpanding }

    [Header("Pilih Pola Serangan")]
    public PatternType polaSaatIni = PatternType.Spiral;

    [Header("Settings Dasar")]
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public float bulletSpeed = 5f;
    public float fireRate = 0.05f; // Sangat cepat untuk efek bullet hell

    [Header("Pengatur Pola (Akan Berubah Otomatis)")]
    private float baseAngle = 0f;
    private float fireTimer;

    void Update()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireRate)
        {
            JalankanPolaTembakan();
            fireTimer = 0f;
        }
    }

    void JalankanPolaTembakan()
    {
        if (bulletPrefab == null || shootPoint == null) return;

        switch (polaSaatIni)
        {
            case PatternType.Spiral:
                SpawnSpiral();
                break;
            case PatternType.WaveSpiral:
                SpawnWaveSpiral();
                break;
            case PatternType.CrossExpanding:
                SpawnCrossExpanding();
                break;
        }
    }

    // ========================================================
    // POLA 1: SPIRAL CLOCKWISE (Semburan Memutar Searah Jarum Jam)
    // ========================================================
    void SpawnSpiral()
    {
        // Menembakkan 2 peluru yang berlawanan arah (0 dan 180 derajat)
        // lalu sudut dasarnya terus ditambah agar berputar
        for (int i = 0; i < 2; i++)
        {
            float finalAngle = baseAngle + (i * 180f);
            CreateBulletAtAngle(finalAngle);
        }

        baseAngle += 10f; // Mengontrol kecepatan putaran spiral
        if (baseAngle >= 360f) baseAngle = 0f;
    }

    // ========================================================
    // POLA 2: WAVE SPIRAL (Spiral yang Berayun Kiri-Kanan Seperti Ombak)
    // ========================================================
    void SpawnWaveSpiral()
    {
        // Menggunakan Mathf.Sin untuk membuat sudut putarannya mengayun bolak-balik
        float waveAngle = baseAngle + Mathf.Sin(Time.time * 3f) * 45f;

        // Menembak 4 arah sekaligus yang berayun
        for (int i = 0; i < 4; i++)
        {
            float finalAngle = waveAngle + (i * 90f);
            CreateBulletAtAngle(finalAngle);
        }

        baseAngle += 5f;
    }

    // ========================================================
    // POLA 3: CROSS EXPANDING (Pola Salib Berondong Menyebar)
    // ========================================================
    void SpawnCrossExpanding()
    {
        // Sekali tembak langsung memancarkan 6 peluru berbentuk lingkaran,
        // tapi sudut awalnya berganti-ganti secara patah-patah
        int bulletCount = 6;
        float angleStep = 360f / bulletCount;

        for (int i = 0; i < bulletCount; i++)
        {
            float finalAngle = (i * angleStep) + baseAngle;
            CreateBulletAtAngle(finalAngle);
        }

        baseAngle += 25f; // Membuat lingkaran berikutnya agak miring dari sebelumnya
    }

    // Fungsi pembantu untuk memproses instansiasi peluru berdasarkan sudut
    void CreateBulletAtAngle(float targetAngle)
    {
        float bulletDirX = Mathf.Cos(targetAngle * Mathf.Deg2Rad);
        float bulletDirY = Mathf.Sin(targetAngle * Mathf.Deg2Rad);
        Vector2 moveDirection = new Vector2(bulletDirX, bulletDirY).normalized;

        float rotationAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        Quaternion bulletRotation = Quaternion.Euler(0, 0, rotationAngle);

        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, bulletRotation);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = moveDirection * bulletSpeed;
        }
    }
}