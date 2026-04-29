using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;

    void Start()
    {
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // 1. Cek apakah yang kena adalah musuh (Enemy)
        if (hitInfo.CompareTag("Enemy"))
        {
            Health targetHealth = hitInfo.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage);
            }
            Destroy(gameObject); // Peluru hancur setelah kena musuh
        }

        // 2. Tambahkan ini agar peluru hancur jika kena tembok (opsional)
        // else if (hitInfo.CompareTag("Wall")) { Destroy(gameObject); }
    }
}