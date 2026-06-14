using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefab Musuh")]
    public GameObject enemyPrefab;      // Musuh pertama (Robot Ungu)
    public GameObject enemyPrefab2;     // Musuh kedua (Penembak)
    public GameObject bossPrefab;       // TAMBAHAN: Prefab Musuh Bos

    [Header("Waktu Kemunculan (Detik)")]
    [Tooltip("Waktu kapan musuh penembak mulai ikut muncul")]
    public float waktuSpawnEnemy2 = 10f; 
    [Tooltip("Waktu kapan bos besar muncul dan menyapu kroco")]
    public float waktuSpawnBoss = 50f;   

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

    // TAMBAHAN: Penanda agar musik tahap 2 hanya dipicu satu kali saja di menit ke-5
    private bool musicStage2Triggered = false;

    void Start()
    {
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null) player = playerObj.transform;

        gameTimer = Object.FindFirstObjectByType<GameTimer>();
    }

    void Update()
    {
        if (bossSpawned) return;

        // Boss spawn
        // MENGGUNAKAN VARIABEL PUBLIC waktuSpawnBoss
        if (gameTimer != null && gameTimer.GetTotalTime() >= waktuSpawnBoss && !bossSpawned)
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

        // Spawn enemy 2
        // MENGGUNAKAN VARIABEL PUBLIC waktuSpawnEnemy2
        if (gameTimer != null && gameTimer.GetTotalTime() >= waktuSpawnEnemy2)
        {
            if (!musicStage2Triggered)
            {
                musicStage2Triggered = true;
                MusicManager music = Object.FindFirstObjectByType<MusicManager>();
                if (music != null)
                {
                    music.GantiMusikDinamis("Tahap2");
                }
            }

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

        MusicManager music = Object.FindFirstObjectByType<MusicManager>();
        if (music != null)
        {
            music.GantiMusikDinamis("Boss");
        }

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