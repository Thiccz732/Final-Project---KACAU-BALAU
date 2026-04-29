using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public GameObject wallPrefab; // Masukkan prefab tembok/block di sini
    public int numberOfWalls = 10; // Jumlah tembok yang ingin di-spawn
    public Vector2 spawnArea = new Vector2(10, 10); // Rentang area (X, Y)

    void Start()
    {
        GenerateWalls();
    }

    void GenerateWalls()
    {
        for (int i = 0; i < numberOfWalls; i++)
        {
            // Menghitung posisi acak di dalam jangkauan spawnArea
            float randomX = Random.Range(-spawnArea.x / 2, spawnArea.x / 2);
            float randomY = Random.Range(-spawnArea.y / 2, spawnArea.y / 2);
            Vector3 spawnPos = new Vector3(randomX, randomY, 0);

            // Munculkan tembok
            Instantiate(wallPrefab, spawnPos, Quaternion.identity, transform);
        }
    }

    // Menggambar kotak area di Editor untuk membantu visualisasi
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnArea.x, spawnArea.y, 0));
    }
}