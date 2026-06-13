using UnityEngine;
using System.Collections;

public class GameplayIntro : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Tarik objek 'ControlOverlay' dari Hierarchy ke sini")]
    public CanvasGroup overlayCanvasGroup;

    [Header("Settings")]
    public float durasiTampil = 3f; // Berapa lama teks petunjuk diam di layar
    public float kecepatanFadeOut = 2f; // Kecepatan menghilang secara halus

    void Start()
    {
        // Jalankan hitung mundur overlay tanpa mengunci komponen Player sama sekali
        StartCoroutine(AlurOverlayKontrol());
    }

    private IEnumerator AlurOverlayKontrol()
    {
        // Pastikan overlay tampil penuh di detik pertama game dimulai
        if (overlayCanvasGroup != null)
        {
            overlayCanvasGroup.alpha = 1f;
            
            // SANGAT PENTING: Ubah blocksRaycasts menjadi false agar klik mouse (Right MB) 
            // milikmu tidak terhalang oleh panel UI dan bisa langsung dipakai menembak!
            overlayCanvasGroup.blocksRaycasts = false; 
        }

        // Game tetap jalan, kamu bisa gerak, overlay diam menemani selama 3 detik
        yield return new WaitForSeconds(durasiTampil);

        // Efek memudar halus (Fade Out)
        if (overlayCanvasGroup != null)
        {
            while (overlayCanvasGroup.alpha > 0f)
            {
                overlayCanvasGroup.alpha -= Time.deltaTime * kecepatanFadeOut;
                yield return null;
            }
        }

        // Matikan objek overlay sepenuhnya setelah menghilang agar hemat memori
        if (overlayCanvasGroup != null) 
        {
            overlayCanvasGroup.gameObject.SetActive(false);
        }
    }
}