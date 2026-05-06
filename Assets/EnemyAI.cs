using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 3f;

    private Transform player;
    private Rigidbody2D rb;

    void Start()
    {
        // Mengambil komponen Rigidbody untuk pergerakan fisik
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void FixedUpdate() // Gunakan FixedUpdate untuk segala hal terkait Fisika/Rigidbody
    {
        if (player != null)
        {
            MoveTowardsPlayer();
        }
    }

    void MoveTowardsPlayer()
    {
        // 1. Hitung arah ke player
        Vector2 direction = (player.position - transform.position).normalized;

        // 2. Gunakan velocity agar musuh saling bertabrakan dan tidak menumpuk
        rb.linearVelocity = direction * speed;
    }
}