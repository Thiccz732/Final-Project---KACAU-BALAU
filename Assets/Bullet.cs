using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;

    // Menentukan tipe peluru di Inspector
    public enum BulletOwner { Player, Enemy }
    public BulletOwner firedBy;

    void Start()
    {
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        // Peluru akan bergerak maju ke arah "depan/kanan" objek secara lokal
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // JIKA PELURU INI MILIK PLAYER
        if (firedBy == BulletOwner.Player)
        {
            // TAMBAHAN: Menggunakan operator || (atau) untuk mengecek Tag "Enemy" ATAU "Boss"
            if (hitInfo.CompareTag("Enemy") || hitInfo.CompareTag("Boss"))
            {
                Health targetHealth = hitInfo.GetComponent<Health>();
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(damage);
                }
                Destroy(gameObject); // Peluru hancur setelah kena musuh/bos
            }
        }
        // JIKA PELURU INI MILIK MUSUH
        else if (firedBy == BulletOwner.Enemy)
        {
            if (hitInfo.CompareTag("Player"))
            {
                Health targetHealth = hitInfo.GetComponent<Health>();
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(damage);
                }
                Destroy(gameObject); // Peluru hancur setelah kena player
            }
        }

        // Peluru hancur jika menabrak tembok
        if (hitInfo.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}