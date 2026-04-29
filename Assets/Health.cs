using UnityEngine;
using TMPro; // Dibutuhkan untuk menampilkan UI darah

public class Health : MonoBehaviour
{
    [Header("Statistik Darah")]
    public int maxHealth = 4;
    private int currentHealth;

    [Header("Settings")]
    public bool isPlayer; // Centang di Inspector jika ini adalah Player
    public TextMeshProUGUI healthText; // Taruk objek Text UI ke sini (hanya untuk Player)

    void Start()
    {
        currentHealth = maxHealth;
        if (isPlayer)
        {
            UpdateUI();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (isPlayer)
        {
            UpdateUI();
        }

        Debug.Log(gameObject.name + " terkena damage! Sisa darah: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateUI()
    {
        if (healthText != null)
        {
            healthText.text = "HP: " + currentHealth;
        }
    }

    // Menggunakan OnCollisionEnter2D (Jika Collider TIDAK dicentang Is Trigger)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Player hanya kurang darah jika bersentuhan dengan musuh
        if (isPlayer && collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }

    // Menggunakan OnTriggerEnter2D (Jika Collider DICENTANG Is Trigger)
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Player hanya kurang darah jika bersentuhan dengan musuh
        if (isPlayer && other.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }

    void Die()
    {
        if (isPlayer)
        {
            Debug.Log("Player mati! Game Over.");
            // Kamu bisa tambahkan SceneManager.LoadScene di sini nanti
        }
        else
        {
            Debug.Log("Musuh hancur!");
        }

        Destroy(gameObject);
    }
}