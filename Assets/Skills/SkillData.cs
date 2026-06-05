using UnityEngine;

// Memunculkan menu klik kanan di Unity untuk membuat file data skill kustom
[CreateAssetMenu(fileName = "NewSkillData", menuName = "Sistem Game/Skill Data")]
public class SkillData : ScriptableObject
{
    [Header("Informasi Dasar")]
    public string namaSkill;
    [TextArea(2, 5)] public string deskripsiSkill;
    public Sprite ikonSkill;

    [Header("Audio Settings")]
    [Tooltip("Tarik file audio (.mp3/.wav) yang akan berputar saat skill ini ditekan")]
    public AudioClip sfxSkillAktif;

    [Header("Statistik Dasar Skill")]
    public float durasiSkill = 5f;       // Berapa lama skill aktif (detik)
    public float durasiCooldown = 10f;   // Jeda sebelum bisa dipakai lagi (detik)

    [Header("Efek Modifikasi Statistik Player")]
    [Tooltip("Gunakan angka positif untuk mempercepat, gunakan angka NEGATIF (misal: -2.5) untuk memperlambat jalan")]
    public float bonusSpeed = 0f;        

    [Tooltip("Gunakan angka positif untuk menambah daya serang, gunakan angka NEGATIF (misal: -1) untuk mengurangi damage")]
    public int bonusDamage = 0;          

    [Tooltip("Mengurangi jeda waktu antar peluru. Isi dengan angka POSITIF (misal: 0.3) agar tembakan menjadi sangat cepat seperti senapan mesin")]
    public float pengurangFireRate = 0f; 
}