using UnityEngine;

public partial class Health : MonoBehaviour
{
    public int maxHealth = 4;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Tambahkan efek partikel atau suara di sini jika perlu
        Destroy(gameObject);
    }
}