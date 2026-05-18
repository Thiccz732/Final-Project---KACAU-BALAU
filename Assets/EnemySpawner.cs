using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // Musuh pertama (Robot Ungu)
    public GameObject enemyPrefab;

    // TAMBAHAN: Musuh kedua (Hanya muncul setelah 5 menit)
    [Header("Musuh Kedua Settings")]
    public GameObject enemyPrefab2;

    [Header("Settings Agresif")]
    public float spawnRate = 0.5f;
    private float nextSpawnTime;

    [Header("Jarak Spawn")]
    public float minSpawnDistance = 12f;
    public float maxSpawnDistance = 18f;

    private Transform player;

    // TAMBAHAN: Referensi ke script pencatat waktu
    private GameTimer gameTimer;

    void Start()
    {
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null) player = playerObj.transform;

        // TAMBAHAN: Cari komponen GameTimer yang ada di Scene
        gameTimer = Object.FindFirstObjectByType<GameTimer>();
    }

    void Update()
    {
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

        // Ubah angka 5f ini untuk testing (misal 5f = 5 detik, kalau asli = 300f)
        float waktuSyaratMuncul = 5f;

        if (gameTimer != null && gameTimer.GetTotalTime() >= waktuSyaratMuncul)
        {
            // Jika sudah lewat waktunya, acak antara musuh 1 atau musuh 2 (Peluang 50:50)
            int randomChance = Random.Range(0, 2);

            if (randomChance == 0)
            {
                Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            }
            else
            {
                Instantiate(enemyPrefab2, spawnPos, Quaternion.identity);
                Debug.Log("Musuh kedua (Penembak) berhasil muncul!");
            }
        }
        else
        {
            // Jika belum memenuhi syarat waktu, HANYA memunculkan musuh pertama (Robot Ungu)
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        }
    }

    bool IsPosInPlayerView(Vector2 worldPos)
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(worldPos);
        return viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1 && viewPos.z > 0;
    }
}