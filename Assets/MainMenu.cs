using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 
using System.Collections; 

public class MainMenu : MonoBehaviour
{
    [Header("UI Settings")]
    public TextMeshProUGUI longestSurviveText;

    [Header("Audio Delay Settings")]
    public AudioSource sfxTombolPlay;
    public AudioSource sfxTombolQuit;
    public float jedaPindahScene = 0.4f; 

   void Start()
    {
        // =======================================================
        // UPDATE JSON: Ambil data waktu terbaik dari JSON
        // =======================================================
        float bestTimeInSeconds = SaveSystem.LoadBestTime();

        if (bestTimeInSeconds <= 0f)
        {
            if (longestSurviveText != null) longestSurviveText.text = "00:00"; 
            return;
        }

        int minutes = Mathf.FloorToInt(bestTimeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(bestTimeInSeconds % 60f);

        if (longestSurviveText != null)
        {
            longestSurviveText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void StartGame()
    {
        StartCoroutine(AlurPindahSceneDenganSuara());
    }

    private IEnumerator AlurPindahSceneDenganSuara()
    {
        if (sfxTombolPlay != null) sfxTombolPlay.Play();
        yield return new WaitForSeconds(jedaPindahScene);
        SceneManager.LoadScene("SampleScene"); 
    }

    public void ExitGame()
    {
        StartCoroutine(AlurKeluarGameDenganSuara());
    }

    private IEnumerator AlurKeluarGameDenganSuara()
    {
        if (sfxTombolQuit != null) sfxTombolQuit.Play();
        yield return new WaitForSeconds(jedaPindahScene);
        Application.Quit(); 
    }

    public void ResetRecord()
    {
        // =======================================================
        // UPDATE JSON: Hapus file JSON fisik dari laptop
        // =======================================================
        SaveSystem.ResetData();
        
        if (longestSurviveText != null)
        {
            longestSurviveText.text = "00:00";
        }
    }
}