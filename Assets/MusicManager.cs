using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Daftar Lagu BGM")]
    public AudioClip bgmTahap1; 
    public AudioClip bgmTahap2;
    public AudioClip bgmBoss;   

    private AudioClip currentTrack;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Setelan dasar AudioSource agar musik terus berputar
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        // Jalankan musik tahap 1 di awal game
        if (bgmTahap1 != null)
        {
            currentTrack = bgmTahap1;
            audioSource.clip = bgmTahap1;
            audioSource.Play();
        }
    }

    // Fungsi utama yang akan dipanggil oleh script Spawner / Musuh
    public void GantiMusikDinamis(string tipeTahap)
    {
        AudioClip laguBaru = null;

        if (tipeTahap == "Tahap2") laguBaru = bgmTahap2;
        else if (tipeTahap == "Boss") laguBaru = bgmBoss;
        else if (tipeTahap == "Tahap1") laguBaru = bgmTahap1;

        // Jika lagu yang diminta berbeda dengan yang sedang berputar, lakukan transisi halus
        if (laguBaru != null && laguBaru != currentTrack)
        {
            currentTrack = laguBaru;
            StopAllCoroutines();
            StartCoroutine(TransisiLaguHalus(laguBaru));
        }
    }

    // Coroutine untuk efek Fade Out (mengecil) lalu Fade In (membesar)
    private IEnumerator TransisiLaguHalus(AudioClip clipBaru)
    {
        float durasiTransisi = 1.5f; // Jeda waktu perpindahan lagu (1.5 detik)
        float timer = 0f;
        float volumeAwal = audioSource.volume;

        // 1. FADE OUT: Mengecilkan volume musik lama sampai senyap
        while (timer < durasiTransisi)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(volumeAwal, 0f, timer / durasiTransisi);
            yield return null;
        }

        // 2. GANTI KLIP: Tukar lagu di latar belakang saat senyap
        audioSource.Stop();
        audioSource.clip = clipBaru;
        audioSource.Play();

        // 3. FADE IN: Membesarkan kembali volume musik baru ke normal
        timer = 0f;
        while (timer < durasiTransisi)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, 1f, timer / durasiTransisi);
            yield return null;
        }
    }
}