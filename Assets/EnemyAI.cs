using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 3f;
    public float detectionRange = 10f; // Jarak musuh bisa melihat
    public LayerMask obstacleLayer;    // Pilih layer "Wall" di Inspector

    private Transform player;
    private bool canSeePlayer = false;

    void Start()
    {
        // Mencari Player di Scene
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        // 1. Hitung jarak ke Player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            // 2. Cek apakah ada tembok di antara Musuh dan Player (Raycasting)
            CheckLineOfSight();
        }
        else
        {
            canSeePlayer = false;
        }

        // 3. Jika melihat player, baru bergerak mengejar
        if (canSeePlayer)
        {
            MoveTowardsPlayer();
        }
    }

    void CheckLineOfSight()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, player.position);

        // Menembakkan sinar dari musuh ke arah player
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, obstacleLayer);

        if (hit.collider == null)
        {
            // Jika sinar tidak menabrak apa pun (tembok), berarti musuh melihat player
            canSeePlayer = true;
        }
        else
        {
            // Jika sinar menabrak sesuatu, berarti itu tembok
            canSeePlayer = false;
        }
    }

    void MoveTowardsPlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }

    // Untuk visualisasi jarak pandang di Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}