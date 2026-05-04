using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    [Header("Settings Agresif")]
    // Nilai kecil = spawn sangat cepat (misal: tiap 0.5 detik)
    public float spawnRate = 0.5f;
    private float nextSpawnTime;

    [Header("Jarak Spawn")]
    public float minSpawnDistance = 12f;
    public float maxSpawnDistance = 18f;

    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        // Selama waktu terpenuhi, terus spawn musuh baru
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

        // Mencari posisi sampai dapat yang di luar layar
        while (!found)
        {
            float randomDist = Random.Range(minSpawnDistance, maxSpawnDistance);
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            spawnPos = (Vector2)player.position + (randomDir * randomDist);

            if (!IsPosInPlayerView(spawnPos)) found = true;
        }

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }

    bool IsPosInPlayerView(Vector2 worldPos)
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(worldPos);
        return viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1 && viewPos.z > 0;
    }
}