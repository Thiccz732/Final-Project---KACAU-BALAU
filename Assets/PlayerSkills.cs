using UnityEngine;
using System.Collections;
using TMPro;

public class PlayerSkills : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI skill1dan2Text;     // Tarik teks UI Skill 1 & 2 ke sini
    public TextMeshProUGUI skill3CooldownText; // Tarik teks UI Skill 3 ke sini

    // =======================================================================
    // TAMBAHAN: Slot Audio Sound Cue untuk Masing-Masing Skill
    // =======================================================================
    [Header("Skill Sound Cues")]
    [Tooltip("Tarik file audio untuk Skill 1 ke sini")]
    public AudioClip soundSkill1;
    [Tooltip("Tarik file audio untuk Skill 2 ke sini")]
    public AudioClip soundSkill2;
    [Tooltip("Tarik file audio untuk Skill 3 ke sini")]
    public AudioClip soundSkill3;

    // Penanda status internal agar antar skill tidak saling tabrakan
    private bool isSkillActive = false;
    private bool isCooldownSkill12 = false;
    private bool isCooldownSkill3 = false;

    // Komponen jembatan untuk mengubah status movement
    private PlayerMovement movement;
    private PlayerControl controls;

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        controls = new PlayerControl();

        // Menghubungkan Input System langsung ke fungsi Coroutine masing-masing skill
        // Ditambahkan parameter AudioClip di paling belakang agar fungsi tahu suara mana yang harus diputar
        controls.Player.Skill1.performed += ctx => StartCoroutine(HandleSkill1Dan2(3, 0.5f, "SILVER BULLET", 5f, 5f, soundSkill1));
        controls.Player.Skill2.performed += ctx => StartCoroutine(HandleSkill1Dan2(1, 2.0f, "HYPER DRIVE", 5f, 5f, soundSkill2));
        controls.Player.Skill3.performed += ctx => StartCoroutine(HandleSkill3(4.5f, 5f, 15f));
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    // =======================================================================
    // LOGIKA SKILL 1 & 2 (Manipulasi Damage & Kecepatan Gerak)
    // =======================================================================
    IEnumerator HandleSkill1Dan2(int targetDamage, float speedMultiplier, string skillName, float duration, float cooldown, AudioClip skillSound)
    {
        if (isSkillActive || isCooldownSkill12 || movement == null) yield break;

        isSkillActive = true;

        // Pemicu suara skill 1 atau skill 2 saat aktif
        if (skillSound != null)
        {
            AudioSource.PlayClipAtPoint(skillSound, transform.position);
        }

        movement.currentDamage = targetDamage;
        movement.currentSpeed = movement.baseSpeed * speedMultiplier;

        if (skill1dan2Text != null)
        {
            skill1dan2Text.text = skillName + " ACTIVE!";
            skill1dan2Text.color = Color.cyan;
        }

        yield return new WaitForSeconds(duration);

        // Kembalikan ke normal
        movement.currentDamage = movement.baseDamage;
        movement.currentSpeed = movement.baseSpeed;
        isSkillActive = false;
        isCooldownSkill12 = true;

        float timer = cooldown;
        while (timer > 0)
        {
            if (skill1dan2Text != null)
            {
                skill1dan2Text.text = skillName + " CD: " + timer.ToString("F1") + "s";
                skill1dan2Text.color = Color.red;
            }
            yield return new WaitForSeconds(0.1f);
            timer -= 0.1f;
        }

        isCooldownSkill12 = false;
        if (skill1dan2Text != null)
        {
            skill1dan2Text.text = "READY!";
            skill1dan2Text.color = Color.green;
        }

        yield return new WaitForSeconds(1f);
        if (!isSkillActive && !isCooldownSkill12 && skill1dan2Text != null)
            skill1dan2Text.text = "";
    }

    // =======================================================================
    // LOGIKA KHUSUS SKILL 3 (MACHINE GUN MODE)
    // =======================================================================
    IEnumerator HandleSkill3(float fireRateMultiplier, float duration, float cooldown)
    {
        if (isSkillActive || isCooldownSkill3 || movement == null) yield break;

        isSkillActive = true;
        
        // Pemicu suara khusus Skill 3 saat diaktifkan
        if (soundSkill3 != null)
        {
            AudioSource.PlayClipAtPoint(soundSkill3, transform.position);
        }

        // Ubah damage jadi 1 dan percepat tembakan
        movement.currentDamage = 1;
        movement.currentFireRate = movement.baseFireRate / fireRateMultiplier;

        if (skill3CooldownText != null)
        {
            skill3CooldownText.text = "MACHINE GUN MODE!";
            skill3CooldownText.color = Color.yellow;
        }

        Debug.Log("Skill 3 Aktif: Brutal Machine Gun! (Damage berkurang jadi 1)");

        yield return new WaitForSeconds(duration);

        // Reset kembali status player ke normal semula after 5 detik
        movement.currentFireRate = movement.baseFireRate;
        movement.currentDamage = movement.baseDamage; 
        isSkillActive = false;
        isCooldownSkill3 = true;

        float timer = cooldown;
        while (timer > 0)
        {
            if (skill3CooldownText != null)
            {
                skill3CooldownText.text = "S3 CD: " + timer.ToString("F1") + "s";
                skill3CooldownText.color = Color.red;
            }
            yield return new WaitForSeconds(0.1f);
            timer -= 0.1f;
        }

        isCooldownSkill3 = false;
        if (skill3CooldownText != null)
        {
            skill3CooldownText.text = "S3 READY!";
            skill3CooldownText.color = Color.green;
        }

        yield return new WaitForSeconds(1f);
        if (!isSkillActive && !isCooldownSkill3 && skill3CooldownText != null)
            skill3CooldownText.text = "";
    }
}