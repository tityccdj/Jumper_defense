using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour, IDamageable
{
    public int maxHealth = 12;
    public int currentHealth;
    [Header("Death Settings")]
    public bool destroyOnDie = true;

    [Header("Invincibility (I-Frames)")]
    public bool hasInvincibility = false;
    public float invincibilityTime = 1f;

    [Header("Events")]
    public UnityEvent onTakeDamage;
    public UnityEvent onDie;
    // เปลี่ยนจากตัวเดียว เป็น Array (ใส่วงเล็บเหลี่ยม [])
    // และเปลี่ยนเป็น private เพราะเราจะให้โค้ดหาเองอัตโนมัติ ไม่ต้องลากใส่ใน Inspector
    private SpriteRenderer[] allSprites;

    private bool isInvincible = false;
    private void Awake()
    {
        currentHealth = maxHealth;

        // ให้โค้ดค้นหา SpriteRenderer "ทั้งหมด" ในตัวมันและลูกๆ ทันทีที่เริ่มเกม
        allSprites = GetComponentsInChildren<SpriteRenderer>();
    }
    void Start()
    {
        
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        Debug.Log(gameObject.name + " โดนดาเมจ! เลือดเหลือ: " + currentHealth);
        onTakeDamage?.Invoke();
        if (currentHealth <= 0)
        {
            Die();
        }
        else if (hasInvincibility && gameObject.activeInHierarchy)
        {
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

        float elapsedTime = 0f;
        float blinkInterval = 0.1f;

        while (elapsedTime < invincibilityTime)
        {
            // ใช้ foreach เพื่อสั่งกระพริบ Sprite ทุกตัวที่หาเจอพร้อมๆ กัน
            foreach (SpriteRenderer sr in allSprites)
            {
                if (sr != null)
                {
                    sr.enabled = !sr.enabled;
                }
            }

            yield return new WaitForSeconds(blinkInterval);
            elapsedTime += blinkInterval;
        }

        // เมื่อหมดเวลาอมตะ บังคับเปิด Sprite ทุกตัวให้กลับมามองเห็นตามปกติ
        foreach (SpriteRenderer sr in allSprites)
        {
            if (sr != null)
            {
                sr.enabled = true;
            }
        }

        isInvincible = false;
    }
    // ฟังก์ชันนี้สำหรับเรียกใช้ผ่าน Unity Event ใน Inspector
    public void SpawnEnemyDeathParticle()
    {
        if (ParticleManager.Instance != null)
        {
            ParticleManager.Instance.PlayEnemyDeath(transform.position);
        }
    }
    private void Die()
    {
        onDie?.Invoke(); // เรียก Event แจ้งเตือนว่าตายแล้ว
        
        if (destroyOnDie)
        {
            Destroy(gameObject); // ลบทิ้งเฉพาะตัวที่ตั้งให้ลบทิ้ง
        }
    }
    // ฟังก์ชันสำหรับส่งค่าเลือดปัจจุบันออกไปให้ UI อ่าน
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    public void Revive()
    {
        currentHealth = maxHealth; // เลือดกลับมาเต็ม
        if (UIManager.Instance != null) UIManager.Instance.UpdatePlayerHealthUI(); // อัปเดตหลอดเลือด
    }
}