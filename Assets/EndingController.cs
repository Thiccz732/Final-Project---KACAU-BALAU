using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // Wajib untuk memuat ulang Main Menu

public class EndingController : MonoBehaviour
{
    [Header("Waktu Mundur")]
    public float jedaWaktu = 7f; // Durasi sebelum pindah ke Main Menu (7 detik)

    void Start()
    {
        // Jalankan hitung mundur otomatis saat Scene ini terbuka
        StartCoroutine(HitungMundurKeMainMenu());
    }

    private IEnumerator HitungMundurKeMainMenu()
    {
        // Menunggu selama durasi yang ditentukan (7 detik)
        yield return new WaitForSeconds(jedaWaktu);

        Debug.Log("7 Detik selesai. Kembali ke Main Menu...");
        
        // Pindah kembali ke Scene Menu Utama
        // Pastikan teks "MainMenu" di bawah sesuai dengan nama asli Scene Menu milikmu
        SceneManager.LoadScene("MainMenu"); 
    }
}