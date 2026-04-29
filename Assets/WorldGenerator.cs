using UnityEngine;
using System.Collections.Generic;

public class WorldGenerator : MonoBehaviour
{
    public GameObject wallPrefab;
    public Transform player;

    public float chunkSize = 20f; // Ukuran satu petak area
    public int viewDistance = 2;   // Berapa banyak petak di sekitar player yang di-load
    public int wallsPerChunk = 5;  // Kerapatan tembok per petak

    // Menyimpan data chunk yang sudah pernah dibuat agar tidak berubah-ubah posisinya
    private Dictionary<Vector2Int, GameObject> activeChunks = new Dictionary<Vector2Int, GameObject>();

    void Update()
    {
        if (player == null) return;

        // Tentukan koordinat petak tempat player berada sekarang
        int currentX = Mathf.RoundToInt(player.position.x / chunkSize);
        int currentY = Mathf.RoundToInt(player.position.y / chunkSize);

        // Cek area di sekitar player
        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int y = -viewDistance; y <= viewDistance; y++)
            {
                Vector2Int chunkCoord = new Vector2Int(currentX + x, currentY + y);

                if (!activeChunks.ContainsKey(chunkCoord))
                {
                    CreateChunk(chunkCoord);
                }
            }
        }

        // Opsional: Hapus chunk yang terlalu jauh untuk optimasi performa
        CleanupRemoteChunks(currentX, currentY);
    }

    void CreateChunk(Vector2Int coord)
    {
        // Buat objek penampung untuk satu petak
        GameObject chunkParent = new GameObject("Chunk_" + coord.x + "_" + coord.y);
        chunkParent.transform.parent = this.transform;

        // Gunakan koordinat sebagai seed agar saat balik ke sini, posisi tembok tetap sama
        Random.InitState(coord.x * 31 + coord.y * 7);

        for (int i = 0; i < wallsPerChunk; i++)
        {
            float xPos = (coord.x * chunkSize) + Random.Range(-chunkSize / 2, chunkSize / 2);
            float yPos = (coord.y * chunkSize) + Random.Range(-chunkSize / 2, chunkSize / 2);

            Vector3 spawnPos = new Vector3(xPos, yPos, 0);
            Instantiate(wallPrefab, spawnPos, Quaternion.identity, chunkParent.transform);
        }

        activeChunks.Add(coord, chunkParent);
    }

    void CleanupRemoteChunks(int pX, int pY)
    {
        List<Vector2Int> toRemove = new List<Vector2Int>();

        foreach (var chunk in activeChunks)
        {
            if (Mathf.Abs(chunk.Key.x - pX) > viewDistance + 1 || Mathf.Abs(chunk.Key.y - pY) > viewDistance + 1)
            {
                toRemove.Add(chunk.Key);
            }
        }

        foreach (Vector2Int coord in toRemove)
        {
            Destroy(activeChunks[coord]);
            activeChunks.Remove(coord);
        }
    }
}