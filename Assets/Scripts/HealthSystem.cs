using UnityEngine;

// คลาสนี้สืบทอด MonoBehaviour และต้องทำตามกฎ IDamageable
public class HealthSystem : MonoBehaviour, IDamageable
{
    public int maxHealth = 12;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " โดนดาเมจ! เลือดเหลือ: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // ซ่อน object ไว้ก่อน (เดี๋ยวค่อยมาทำระบบตายแยกตามประเภททีหลัง)
        gameObject.SetActive(false);
    }
}