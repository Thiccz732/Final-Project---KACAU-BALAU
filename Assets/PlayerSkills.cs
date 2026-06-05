using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerSkills : MonoBehaviour
{
    [Header("Slot Skill (Scriptable Objects)")]
    public SkillData dataSkillSlot1; 
    public SkillData dataSkillSlot2; 
    public SkillData dataSkillSlot3; 

    [Header("UI Image Components (HUD)")]
    public Image uiImageSlot1;
    public Image uiImageSlot2;
    public Image uiImageSlot3;

    private PlayerMovement movementScript;
    private PlayerControl controls; // Menggunakan Input Action Asset yang sama

    // SAKLAR GLOBAL: Mengunci agar tidak ada 2 skill aktif bersamaan
    private bool isAnySkillActive = false;

    // Status Cooldown masing-masing slot
    private bool isCooldownSlot1 = false;
    private bool isCooldownSlot2 = false;
    private bool isCooldownSlot3 = false;

    void Awake()
    {
        movementScript = GetComponent<PlayerMovement>();

        // Inisialisasi New Input System
        controls = new PlayerControl();

        // MENGIKAT INPUT EVENT DENGAN VALIDASI PENGAMAN GLOBAL
        controls.Player.Skill1.started += ctx => CobaGunakanSkill(dataSkillSlot1, isCooldownSlot1, 1);
        controls.Player.Skill2.started += ctx => CobaGunakanSkill(dataSkillSlot2, isCooldownSlot2, 2);
        controls.Player.Skill3.started += ctx => CobaGunakanSkill(dataSkillSlot3, isCooldownSlot3, 3);
    }

    // WAJIB DIAKTIFKAN AGAR INPUT SYSTEM AKTIF MEMBACA TOMBOL KEYBOARD
    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start()
    {
        UpdateSkillUI();
    }

    private void UpdateSkillUI()
    {
        if (dataSkillSlot1 != null && uiImageSlot1 != null) uiImageSlot1.sprite = dataSkillSlot1.ikonSkill;
        if (dataSkillSlot2 != null && uiImageSlot2 != null) uiImageSlot2.sprite = dataSkillSlot2.ikonSkill;
        if (dataSkillSlot3 != null && uiImageSlot3 != null) uiImageSlot3.sprite = dataSkillSlot3.ikonSkill;
    }

    private void CobaGunakanSkill(SkillData skill, bool isCooldown, int slotNumber)
    {
        // JIKA ADA SKILL LAIN YANG MASIH AKTIF, LANGSUNG BLOKIR INPUT!
        if (skill == null || isCooldown || isAnySkillActive) 
        {
            Debug.Log($"[Skill System] Input Slot {slotNumber} ditolak. Ada skill aktif atau masih cooldown.");
            return;
        }

        StartCoroutine(AktifkanEfekSkill(skill, slotNumber));
    }

    private IEnumerator AktifkanEfekSkill(SkillData skill, int slotNumber)
    {
        // Kunci saklar global & kunci status cooldown slot terpilih
        isAnySkillActive = true;
        SetCooldownStatus(slotNumber, true);
        
        // Ubah warna UI skill yang ditekan menjadi abu-abu transparan (meredup)
        SetUIAlphaAndColor(slotNumber, new Color(0.3f, 0.3f, 0.3f, 0.6f));

        // Putar Sound Cue SFX
        if (skill.sfxSkillAktif != null)
        {
            AudioSource.PlayClipAtPoint(skill.sfxSkillAktif, transform.position);
        }

        // Terapkan Statistik Bonus/Minus ke Player
        movementScript.currentSpeed += skill.bonusSpeed;
        movementScript.currentDamage += skill.bonusDamage;
        movementScript.currentFireRate -= skill.pengurangFireRate;

        if (movementScript.currentFireRate < 0.05f) movementScript.currentFireRate = 0.05f;

        // --- MASA AKTIF SKILL ---
        yield return new WaitForSeconds(skill.durasiSkill);

        // Kembalikan Statistik Player ke Angka Normal
        movementScript.currentSpeed -= skill.bonusSpeed;
        movementScript.currentDamage -= skill.bonusDamage;
        movementScript.currentFireRate += skill.pengurangFireRate;

        // BUKA SAKLAR GLOBAL: Pemain sekarang sudah boleh memicu skill tipe lain
        isAnySkillActive = false;
        Debug.Log($"[Skill System] Efek {skill.namaSkill} selesai. Saklar global terbuka kembali.");

        // --- MASA COOLDOWN SLOT ---
        // Slot ini tetap terkunci abu-abu sampai waktu cooldown-nya habis
        yield return new WaitForSeconds(skill.durasiCooldown);

        SetCooldownStatus(slotNumber, false);
        
        // Kembalikan warna UI asli not balok menjadi terang semula
        SetUIAlphaAndColor(slotNumber, Color.white);
    }

    private void SetCooldownStatus(int slotNumber, bool status)
    {
        if (slotNumber == 1) isCooldownSlot1 = status;
        else if (slotNumber == 2) isCooldownSlot2 = status;
        else if (slotNumber == 3) isCooldownSlot3 = status;
    }

    private void SetUIAlphaAndColor(int slotNumber, Color targetColor)
    {
        if (slotNumber == 1 && uiImageSlot1 != null) uiImageSlot1.color = targetColor;
        else if (slotNumber == 2 && uiImageSlot2 != null) uiImageSlot2.color = targetColor;
        else if (slotNumber == 3 && uiImageSlot3 != null) uiImageSlot3.color = targetColor;
    }
}