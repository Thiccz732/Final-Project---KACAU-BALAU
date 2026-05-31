using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Header("UI Reference")]
    [Tooltip("Tarik objek PausePanel dari Canvas ke kolom ini")]
    public GameObject pausePanel; // Menampung panel UI Pause

    private bool isPaused = false;

    void Start()
    {
        // Memastikan panel pause tertutup di awal game berjalan
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        
        // Memastikan waktu game berjalan normal di awal
        Time.timeScale = 1f;
    }

    void Update()
    {
        // Mendeteksi jika pemain menekan tombol Escape (Esc)
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
        
        // 1. Munculkan panel UI Pause di layar
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        // 2. BEKUKAN WAKTU GAME TOTAL (Angka 0 berarti berhenti)
        Time.timeScale = 0f;
        
        Debug.Log("Game di-pause.");
    }

    public void ResumeGame()
    {
        isPaused = false;

        // 1. Sembunyikan kembali panel UI Pause
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        // 2. KEMBALIKAN WAKTU GAME KE NORMAL (Angka 1 berarti jalan)
        Time.timeScale = 1f;

        Debug.Log("Game dilanjutkan.");
    }
}