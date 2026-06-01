using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("UI Settings")]
    public TextMeshProUGUI timerText;

    private float elapsedTime = 0f;
    private bool isGameRunning = true;

    void Start()
    {
        elapsedTime = 0f;
        isGameRunning = true; // Memastikan timer langsung jalan saat panggung dimulai
    }

    void Update()
    {
        if (isGameRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);

            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void stopTimer()
    {
        isGameRunning = false;
    }

    public float GetTotalTime()
    {
        return elapsedTime;
    }

    // =======================================================================
    // TAMBAHAN OPSIONAL: PEMBERSIH TIMER SAAT RESTART
    // =======================================================================
    public void ResetTimer()
    {
        elapsedTime = 0f;
        isGameRunning = true;
        UpdateTimerDisplay();
    }
}