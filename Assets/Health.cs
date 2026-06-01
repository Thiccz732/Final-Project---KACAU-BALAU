using UnityEngine;
using TMPro;
using System.Collections; 
using Unity.Cinemachine; 
using UnityEngine.SceneManagement; // WAJIB: Ditambahkan agar bisa memanggil fungsi pindah Scene

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
    public float invincibilityDuration = 1f; // Durasi kebal setelah kena hit
    private bool isInvincible = false;

    [Header("Visual Effects Settings")]
    public GameObject explosionEffectPrefab;

    private HealthUIController uiController;

    void Start()
    {
        currentHealth = maxHealth;
        
        if (isPlayer)
        {
            // 2. DI SINI: Mencari otomatis objek pengatur gambar sprite UI di arena
            uiController = Object.FindFirstObjectByType<HealthUIController>();
            
            UpdateUI();
        }
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
            
            // =======================================================================
            // DI SINI: PEMICU KAMERA GETAR SAAT PLAYER TERKENA HIT
            // =======================================================================
            Unity.Cinemachine.CinemachineImpulseSource impulse = GetComponent<Unity.Cinemachine.CinemachineImpulseSource>();
            if (impulse != null)
            {
                impulse.GenerateImpulse(); // Memancarkan gelombang getar ke kamera
            }

            StartCoroutine(BecomeInvincible());
        }

        if (currentHealth <= 0)
        {
            // Jika darah habis, panggil fungsi Die()
            Die();
        }
    }

    // Coroutine untuk membuat player kebal sementara
    private IEnumerator BecomeInvincible()
    {
        isInvincible = true;

        Debug.Log("Player sedang kebal...");

        // Opsional: Kamu bisa tambahkan visual kedip-kedip sprite Player di sini nanti

        yield return new WaitForSeconds(invincibilityDuration);

        isInvincible = false;
        Debug.Log("Masa kebal habis.");
    }

    void UpdateUI()
    {
        // Tetap mempertahankan text bawaan lama kamu agar tidak error
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
        // Cek jika objek ini adalah Player
        if (isPlayer)
        {
            // 1. JIKA YANG MENABRAK ADALAH KROCO BIASA
            if (collision.gameObject.CompareTag("Enemy"))
            {
                TakeDamage(1);
                Destroy(collision.gameObject); // Kroco langsung hancur saat nyentuh Player
                Debug.Log("Kroco menabrak Player dan hancur!");
            }

            // 2. JIKA YANG MENABRAK ADALAH BOS BESAR
            else if (collision.gameObject.CompareTag("Boss"))
            {
                TakeDamage(1); // Player tetap kena damage
                               // KODE DESTROY SENGAJA TIDAK DITULIS AGAR BOS GAK BAKAL ANCUR!
                Debug.Log("Bos menyenggol Player! Player terluka, tapi Bos tetap hidup.");
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
            Debug.Log("Game Over!");

            GameTimer timer = Object.FindFirstObjectByType<GameTimer>();
            if (timer != null)
            {
                timer.stopTimer();

                // =======================================================================
                // LOGIKA MENYIMPAN REKOR WAKTU BERTAHAN TERLAMA (PLAYERPREFS)
                // =======================================================================
                float waktuSekarang = timer.GetTotalTime(); 
                float rekorLama = PlayerPrefs.GetFloat("BestTime", 0f);

                if (waktuSekarang > rekorLama)
                {
                    PlayerPrefs.SetFloat("BestTime", waktuSekarang);
                    PlayerPrefs.Save(); // Mengunci data agar aman di memori laptop
                    Debug.Log("REKOR BARU BERHASIL DISIMPAN: " + waktuSekarang + " detik!");
                }
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
            if (spawner != null)
            {
                spawner.enabled = false;
            }

            Debug.Log("Semua telah dihapus");

            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.GetComponent<PlayerMovement>().enabled = false;
        }   
        else
        {
            // =======================================================================
            // DI SINI: DETEKSI JIKA YANG MATI ADALAH BOS BESAR -> PINDAH SCENE ENDING
            // =======================================================================
            if (gameObject.CompareTag("Boss"))
            {
                Debug.Log("Bos Hancur! Menyimpan rekor akhir dan pindah ke Ending Scene...");

                // Menghentikan dan mengunci rekor waktu permainan saat sukses menang
                GameTimer timer = Object.FindFirstObjectByType<GameTimer>();
                if (timer != null)
                {
                    timer.stopTimer();
                    float waktuAkhir = timer.GetTotalTime();
                    float rekorLama = PlayerPrefs.GetFloat("BestTime", 0f);
                    if (waktuAkhir > rekorLama)
                    {
                        PlayerPrefs.SetFloat("BestTime", waktuAkhir);
                        PlayerPrefs.Save();
                    }
                }

                // Efek ledakan visual bos sebelum menghilang ke Scene lain (Biar terasa mantap hancurnya)
                if (explosionEffectPrefab != null)
                {
                    Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
                }
                if (deathSound != null)
                {
                    AudioSource.PlayClipAtPoint(deathSound, transform.position);
                }

                // Setel skala waktu ke normal, lalu muat scene khusus puitis teks-mu
                Time.timeScale = 1f;
                SceneManager.LoadScene("EndingScene");
                
                Destroy(gameObject);
                return; // Keluar dari fungsi agar tidak mengeksekusi kode hancur kroco biasa di bawah
            }

            // =======================================================================
            // KONDISI NORMAL: JIKA YANG MATI ADALAH KROCO BIASA
            // =======================================================================
            if (explosionEffectPrefab != null)
            {
                // Spawn animasi ledakan di koordinat musuh tanpa mengubah rotasinya (identity)
                Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            }

            if (deathSound != null)
            {
                AudioSource.PlayClipAtPoint(deathSound, transform.position);
            }

            Debug.Log("Musuh Hancur!");
            Destroy(gameObject); // Tubuh musuh hilang, menyisakan efek ledakan yang berputar
        }
    }
}