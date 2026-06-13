using UnityEngine;
using TMPro;
using System.Collections; 
using Unity.Cinemachine; 
using UnityEngine.SceneManagement; 

public class Health : MonoBehaviour
{
    [Header("Statistik Darah")]
    public int maxHealth = 10; 
    private int currentHealth;

    [Header("Audio Settings")]
    public AudioClip deathSound;

    [Header("Settings")]
    public bool isPlayer;
    public TextMeshProUGUI healthText;

    [Header("I-Frames (Hanya Player)")]
    public float invincibilityDuration = 1f; 
    private bool isInvincible = false;

    [Header("Visual Effects Settings")]
    public GameObject explosionEffectPrefab;

    private HealthUIController uiController;

    void Start()
    {
        currentHealth = maxHealth;
        
        if (isPlayer)
        {
            uiController = Object.FindFirstObjectByType<HealthUIController>();
            UpdateUI();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        if (isPlayer)
        {
            UpdateUI();
            
            Unity.Cinemachine.CinemachineImpulseSource impulse = GetComponent<Unity.Cinemachine.CinemachineImpulseSource>();
            if (impulse != null)
            {
                impulse.GenerateImpulse(); 
            }

            StartCoroutine(BecomeInvincible());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator BecomeInvincible()
    {
        isInvincible = true;
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

        if (uiController != null)
        {
            uiController.UpdateHealthUI(currentHealth);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isPlayer)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                TakeDamage(1);
                Destroy(collision.gameObject); 
            }
            else if (collision.gameObject.CompareTag("Boss"))
            {
                TakeDamage(1); 
            }
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
            Debug.Log("--- CCTV 1: Game Over! Fungsi Die() berhasil dipanggil ---");

            GameTimer timer = Object.FindFirstObjectByType<GameTimer>();
            
            if (timer != null)
            {
                Debug.Log("--- CCTV 2: Script GameTimer BERHASIL ditemukan di arena ---");
                
                // Ambil waktu dulu sebelum dimatikan
                float waktuSekarang = timer.GetTotalTime(); 
                timer.stopTimer(); 

                float rekorLama = SaveSystem.LoadBestTime(); 
                Debug.Log($"--- CCTV 3: Waktu Bermain = {waktuSekarang} detik | Rekor JSON = {rekorLama} detik ---");

                // Cek apakah layak jadi rekor baru
                if (waktuSekarang > rekorLama)
                {
                    SaveSystem.SaveBestTime(waktuSekarang); 
                    Debug.Log("--- CCTV 4: SUKSES! REKOR BARU DISIMPAN KE JSON ---");
                }
                else
                {
                    Debug.Log("--- CCTV 4: GAGAL SAVE. Waktu bermain tidak mengalahkan rekor lama ---");
                }
            }
            else
            {
                Debug.LogError("--- CCTV ERROR: Script GameTimer TIDAK DITEMUKAN! Pantas saja tidak bisa save! ---");
            }

            GameOverManager gameOver = Object.FindFirstObjectByType<GameOverManager>();
            if (gameOver != null)
            {
                gameOver.ShowGameOver();
            }

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                Destroy(enemy);
            }

            EnemySpawner spawner = Object.FindAnyObjectByType<EnemySpawner>();
            if (spawner != null) spawner.enabled = false;

            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.GetComponent<PlayerMovement>().enabled = false;
        }   
        else
        {
            if (gameObject.CompareTag("Boss"))
            {
                Debug.Log("Bos Hancur! Menyimpan rekor akhir dan pindah ke Ending Scene...");

                GameTimer timer = Object.FindFirstObjectByType<GameTimer>();
                if (timer != null)
                {
                    timer.stopTimer();
                    
                    // =======================================================
                    // UPDATE JSON: Simpan Rekor Saat Bos Mati
                    // =======================================================
                    float waktuAkhir = timer.GetTotalTime();
                    float rekorLama = SaveSystem.LoadBestTime(); // Baca dari JSON
                    
                    if (waktuAkhir > rekorLama)
                    {
                        SaveSystem.SaveBestTime(waktuAkhir); // Tulis ke JSON
                    }
                }

                if (explosionEffectPrefab != null) Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
                if (deathSound != null) AudioSource.PlayClipAtPoint(deathSound, transform.position);

                Time.timeScale = 1f;
                SceneManager.LoadScene("EndingScene");
                Destroy(gameObject);
                return; 
            }

            if (explosionEffectPrefab != null) Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            if (deathSound != null) AudioSource.PlayClipAtPoint(deathSound, transform.position);

            Destroy(gameObject); 
        }
    }
}