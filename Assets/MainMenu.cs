using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // WAJIB ditambahkan agar bisa mengontrol teks UI TextMeshPro

public class MainMenu : MonoBehaviour
{
    [Header("UI Settings")]
    [Tooltip("Tarik objek Text (TMP) LongestSurviveText dari Canvas ke sini")]
    public TextMeshProUGUI longestSurviveText;

    void Start()
    {
        // 1. Ambil data waktu terbaik dari memori PlayerPrefs (yang disimpan pas Player mati)
        float bestTimeInSeconds = PlayerPrefs.GetFloat("BestTime", 0f);

        // 2. Jika belum ada rekor sama sekali (pemain baru)
        if (bestTimeInSeconds <= 0f)
        {
            if (longestSurviveText != null)
            {
                longestSurviveText.text = "Longest Survive: -- : --";
            }
            return;
        }

        // 3. Konversi data detik menjadi hitungan Menit dan Detik yang rapi
        int minutes = Mathf.FloorToInt(bestTimeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(bestTimeInSeconds % 60f);

        // 4. Tampilkan hasilnya ke teks UI (Contoh format hasil: Longest Survive: 02:15)
        if (longestSurviveText != null)
        {
            longestSurviveText.text = string.Format("Longest Survive: {0:00}:{1:00}", minutes, seconds);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void ExitGame()
    {
        Debug.Log("Game Keluar");
        Application.Quit();
    }

    // OPSIONAL: Fungsi tambahan jika nanti kamu butuh tombol khusus buat menghapus rekor kembali ke nol
    public void ResetRecord()
    {
        PlayerPrefs.DeleteKey("BestTime");
        if (longestSurviveText != null)
        {
            longestSurviveText.text = "Longest Survive: -- : --";
        }
        Debug.Log("Rekor waktu telah di-reset!");
    }
}