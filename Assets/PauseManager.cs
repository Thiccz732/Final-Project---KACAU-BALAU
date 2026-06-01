using UnityEngine;
using UnityEngine.SceneManagement; // Wajib ditambahkan untuk mengatur pindah Scene dan Restart

public class PauseManager : MonoBehaviour
{
    [Header("UI Reference")]
    [Tooltip("Tarik objek PausePanel dari Canvas ke kolom ini")]
    public GameObject pausePanel; 

    private bool isPaused = false;

    void Start()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        Time.timeScale = 1f; // Pastikan waktu jalan saat masuk scene
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
        Time.timeScale = 0f; // Bekukan game
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        Time.timeScale = 1f; // Jalankan game kembali
    }

    // =======================================================================
    // FUNGSIONALITAS BARU: RESTART GAME
    // =======================================================================
    public void RestartGame()
    {
        Time.timeScale = 1f; // WAJIB: Kembalikan waktu ke normal sebelum reload agar game tidak macet
        
        // Mengambil nama scene aktif saat ini (SampleScene) lalu memuatnya ulang dari awal
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Game Di-reset!");
    }

    // =======================================================================
    // FUNGSIONALITAS BARU: QUIT KE MAIN MENU
    // =======================================================================
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; // WAJIB: Kembalikan waktu ke normal
        
        // Memuat scene menu utama. 
        // Ganti teks "MainMenu" di bawah sesuai dengan nama Scene Menu milikmu yang asli
        SceneManager.LoadScene("MainMenu"); 
        Debug.Log("Keluar ke Main Menu...");
    }
}