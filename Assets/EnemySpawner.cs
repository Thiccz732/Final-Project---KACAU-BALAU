using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefab Musuh")]
    public GameObject enemyPrefab;      // Musuh pertama (Robot Ungu)
    public GameObject enemyPrefab2;     // Musuh kedua (Penembak)
    public GameObject bossPrefab;       // TAMBAHAN: Prefab Musuh Bos

    [Header("Settings Agresif")]
    public float spawnRate = 0.5f;
    private float nextSpawnTime;

    [Header("Jarak Spawn")]
    public float minSpawnDistance = 12f;
    public float maxSpawnDistance = 18f;

    private Transform player;
    private GameTimer gameTimer;

    // TAMBAHAN: Penanda agar bos hanya muncul 1 kali saja
    private bool bossSpawned = false;

    void Start()
    {
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null) player = playerObj.transform;

        gameTimer = Object.FindFirstObjectByType<GameTimer>();
    }

    void Update()
    {
        if (bossSpawned) return;

        // Mengecek syarat waktu kemunculan Bos (Misal: Menit ke-10 atau detik ke-600)
        // Untuk testing, kamu bisa ubah angka 600f ini menjadi 10f (10 detik)
        if (gameTimer != null && gameTimer.GetTotalTime() >= 10f && !bossSpawned)
        {
            SpawnBossAndClearEnemies();
            return;
        }

        if (Time.time >= nextSpawnTime && player != null)
        {
            SpawnEnemyOutsideView();
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    void SpawnEnemyOutsideView()
    {
        Vector2 spawnPos = Vector2.zero;
        bool found = false;

        while (!found)
        {
            float randomDist = Random.Range(minSpawnDistance, maxSpawnDistance);
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            spawnPos = (Vector2)player.position + (randomDir * randomDist);

            if (!IsPosInPlayerView(spawnPos)) found = true;
        }

        // Logika acak kemunculan musuh biasa setelah menit ke-5 (300 detik)
        if (gameTimer != null && gameTimer.GetTotalTime() >= 5f)
        {
            int randomChance = Random.Range(0, 2);
            if (randomChance == 0) Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            else Instantiate(enemyPrefab2, spawnPos, Quaternion.identity);
        }
        else
        {
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        }
    }

    void SpawnBossAndClearEnemies()
    {
        bossSpawned = true; 
        Debug.Log("PERINGATAN: Bos Besar Datang! Menyapu bersih semua kroco...");

        GameObject[] activeEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in activeEnemies)
        {
            Destroy(enemy);
        }

        // 3. Tentukan posisi munculnya bos (Bisa di luar layar atau pas di depan player)
        if (player != null)
        {
            // Contoh: Bos muncul 10 kotak di sebelah kanan posisi Player saat ini
            Vector2 bossSpawnPos = (Vector2)player.position + new Vector2(10f, 0f);

            // 4. Instansiasi Bos Besar ke dalam arena game
            Instantiate(bossPrefab, bossSpawnPos, Quaternion.identity);
        }
    }

    bool IsPosInPlayerView(Vector2 worldPos)
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(worldPos);
        return viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1 && viewPos.z > 0;
    }
}