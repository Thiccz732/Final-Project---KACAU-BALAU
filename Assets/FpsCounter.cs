using UnityEngine;
using TMPro; // Wajib dimasukkan untuk mengontrol TextMeshPro

public class FPSCounter : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI fpsText; // Tarik objek teks FPS ke sini

    [Header("Settings")]
    public float updateInterval = 0.5f; // Jeda waktu update angka (tiap 0.5 detik)

    private float accum = 0f;
    private int frames = 0;
    private float timeleft;
    
    // Status apakah FPS Counter sedang aktif atau disembunyikan
    private bool isCounterActive = false;

    void Start()
    {
        timeleft = updateInterval;

        // Saat game mulai, pastikan teks FPS dalam keadaan tidak aktif/sembunyi
        if (fpsText != null)
        {
            fpsText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // 1. Logika Deteksi Tombol F1
        if (Input.GetKeyDown(KeyCode.F1))
        {
            // Membalikkan status (jika aktif jadi mati, jika mati jadi aktif)
            isCounterActive = !isCounterActive;

            // Aktifkan atau matikan game object teks di layar
            if (fpsText != null)
            {
                fpsText.gameObject.SetActive(isCounterActive);
            }
        }

        // Jika statusnya sedang tidak aktif, hentikan perhitungan FPS di bawahnya (menghemat CPU)
        if (!isCounterActive) return;

        // 2. Logika Perhitungan FPS
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        frames++;

        // Jika interval waktu sudah terpenuhi (0.5 detik), update teks di layar
        if (timeleft <= 0.0f)
        {
            float fps = accum / frames;
            
            if (fpsText != null)
            {
                // Menampilkan angka FPS tanpa desimal (bulat)
                fpsText.text = "FPS: " + Mathf.RoundToInt(fps).ToString();
            }

            // Reset hitungan untuk interval berikutnya
            timeleft = updateInterval;
            accum = 0.0f;
            frames = 0;
        }
    }
}