using UnityEngine;
using TMPro;
using System.Collections; // Dibutuhkan untuk IEnumerator

public class Health : MonoBehaviour
{
    [Header("Statistik Darah")]
    public int maxHealth = 4;
    private int currentHealth;

    [Header("Settings")]
    public bool isPlayer;
    public TextMeshProUGUI healthText;

    [Header("I-Frames (Hanya Player)")]
    public float invincibilityDuration = 1f; // Durasi kebal setelah kena hit
    private bool isInvincible = false;

    void Start()
    {
        currentHealth = maxHealth;
        if (isPlayer) UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        // Jika sedang masa kebal, jangan kurangi darah
        if (isInvincible) return;

        currentHealth -= damage;

        // Proteksi agar darah tidak minus
        if (currentHealth < 0) currentHealth = 0;

        if (isPlayer)
        {
            UpdateUI();
            StartCoroutine(BecomeInvincible());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Coroutine untuk membuat player kebal sementara
    private IEnumerator BecomeInvincible()
    {
        isInvincible = true;

        // Opsional: Kamu bisa tambahkan efek kedip-kedip sprite di sini
        Debug.Log("Player sedang kebal...");

        yield return new WaitForSeconds(invincibilityDuration);

        isInvincible = false;
        Debug.Log("Masa kebal habis.");
    }

    void UpdateUI()
    {
        if (healthText != null)
        {
            healthText.text = "HP: " + currentHealth;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isPlayer && collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isPlayer && other.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }

    void Die()
    {
        if (isPlayer)
        {
            Debug.Log("Game Over!");
            // Bisa tambahkan efek mati atau pindah scene
        }
        Destroy(gameObject);
    }
}