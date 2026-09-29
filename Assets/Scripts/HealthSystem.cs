using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour, IDamageable
{
    public int maxHealth = 12;
    private int currentHealth;

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

    void Start()
    {
        currentHealth = maxHealth;

        // ให้โค้ดค้นหา SpriteRenderer "ทั้งหมด" ในตัวมันและลูกๆ ทันทีที่เริ่มเกม
        allSprites = GetComponentsInChildren<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        Debug.Log(gameObject.name + " โดนดาเมจ! เลือดเหลือ: " + currentHealth);
        onTakeDamage?.Invoke();
        if (currentHealth <= 0)
        {
            onDie.Invoke();
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

    private void Die()
    {
        gameObject.SetActive(false);
    }
}