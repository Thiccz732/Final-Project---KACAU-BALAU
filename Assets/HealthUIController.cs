using UnityEngine;
using UnityEngine.UI;

public class HealthUIController : MonoBehaviour
{
    [Header("UI Reference")]
    public Image healthBarImage; 
    
    [Header("Masukkan 5 Sprite (0 sampai 4) Di Sini")]
    public Sprite[] healthSprites; // Tetap diisi 5 tahapan gambar bar kamu

    // Tambahkan variabel untuk mencatat darah maksimal Player
    private int maxHealth = 10; 

    void Start()
    {
        // Mencari komponen Health milik Player secara otomatis untuk tahu Max Health-nya
        Health playerHealth = Object.FindFirstObjectByType<Health>();
        if (playerHealth != null)
        {
            // Pastikan di script Health.cs kamu ada variabel public/property bernama maxHealth atau MaxHealth
            // Jika variabel di Health.cs kamu bernama 'maxHealth' (huruf kecil), sesuaikan di bawah:
            // maxHealth = playerHealth.maxHealth; 
        }
    }

    public void UpdateHealthUI(int currentHealth)
    {
        if (healthSprites == null || healthSprites.Length == 0) return;

        // =======================================================================
        // RUMUS MATEMATIKA SKALA (MAPPING 10 DARAH KE 5 GAMBAR)
        // =======================================================================
        // 1. Hitung rasio darah saat ini (antara 0.0 sampai 1.0)
        float healthRatio = (float)currentHealth / maxHealth;

        // 2. Kalikan rasio dengan indeks maksimal gambar yang tersedia (5 gambar = indeks 0 sampai 4)
        int maxSpriteIndex = healthSprites.Length - 1;
        int spriteIndex = Mathf.RoundToInt(healthRatio * maxSpriteIndex);

        // 3. Kunci angkanya agar tidak melompat keluar dari batas array gambar
        spriteIndex = Mathf.Clamp(spriteIndex, 0, maxSpriteIndex);

        // 4. Ganti gambar di layar
        if (healthBarImage != null)
        {
            healthBarImage.sprite = healthSprites[spriteIndex];
        }
    }
}