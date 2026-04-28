using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 3f;
    private Transform player;

    void Start()
    {
        // Mencari objek dengan nama "Player" di Hierarchy
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player != null)
        {
            // Menghitung arah ke arah player
            Vector2 direction = (player.position - transform.position).normalized;

            // Menggerakkan musuh
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
    }
}