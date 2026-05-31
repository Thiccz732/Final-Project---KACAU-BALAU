using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 3f;

    private Transform player;
    private Rigidbody2D rb;

    // Variabel penampung batas panggung (Otomatis dicari oleh script)
    private Transform borderLeft;
    private Transform borderRight;
    private Transform borderTop;
    private Transform borderDown;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // OTOMATIS MENCARI OBJEK BORDER DI HIERARCHY KAMU
        // (Pastikan nama objek di Unity kamu persis: Left, Right, Top, Down)
        GameObject leftObj = GameObject.Find("Left");
        if (leftObj != null) borderLeft = leftObj.transform;

        GameObject rightObj = GameObject.Find("Right");
        if (rightObj != null) borderRight = rightObj.transform;

        GameObject topObj = GameObject.Find("Top");
        if (topObj != null) borderTop = topObj.transform;

        GameObject downObj = GameObject.Find("Down");
        if (downObj != null) borderDown = downObj.transform;
    }

    void FixedUpdate() 
    {
        if (player != null)
        {
            MoveTowardsPlayer();
        }

        // =======================================================================
        // SISTEM PENGUNCI OTOMATIS (ANTI-GAGAL)
        // =======================================================================
        // Jika objek pembatas ditemukan, script akan mengambil posisinya secara akurat
        if (borderLeft != null && borderRight != null && borderTop != null && borderDown != null)
        {
            // Mengunci posisi musuh murni di antara koordinat global tembok pembatas
            float clampedX = Mathf.Clamp(transform.position.x, borderLeft.position.x, borderRight.position.x);
            float clampedY = Mathf.Clamp(transform.position.y, borderDown.position.y, borderTop.position.y);
            
            transform.position = new Vector2(clampedX, clampedY);
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }
}