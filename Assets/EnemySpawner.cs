using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Masukkan Prefab Enemy ke sini
    public float spawnRate = 2f;   // Jeda waktu spawn (detik)
    private float nextSpawn = 0f;

    void Update()
    {
        if (Time.time > nextSpawn)
        {
            nextSpawn = Time.time + spawnRate;

            // Tentukan posisi acak di sekitar spawner (contoh radius 5)
            Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * 5f;

            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        }
    }
}