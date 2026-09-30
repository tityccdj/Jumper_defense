using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    public int damageToPlayer = 1;
    public int damageToBase = 1;

    [Header("Damage Cooldown")]
    public float damageCooldown = 1f; // ระยะเวลาอมตะหลังจากโดนชน (1 วินาที)
    private float nextDamageTime = 0f; // ตัวจำเวลาว่ารอบต่อไปจะทำดาเมจได้ตอนไหน

    // เปลี่ยนจาก OnCollisionEnter2D เป็น OnCollisionStay2D 
    // เพื่อให้มันเช็คตลอดเวลาที่ผู้เล่นยังไถหน้าอกอยู่
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // ดึงสคริปต์ PlayerController ของผู้เล่นมาเช็คสถานะ
            PlayerController playerCtrl = collision.gameObject.GetComponent<PlayerController>();

            // ถ้าดึงมาได้ และผู้เล่นกำลังใช้ท่า Ground Pound อยู่
            if (playerCtrl != null && playerCtrl.isGroundPounding)
            {
                return; // ข้ามการทำดาเมจใส่ผู้เล่นไปเลย ปล่อยให้ PlayerCombat จัดการเหยียบ
            }

            // เช็คว่าเวลาปัจจุบัน เลยเวลาคูลดาวน์ที่ตั้งไว้หรือยัง (กันเลือดลดรัวๆ)
            if (Time.time >= nextDamageTime)
            {
                IDamageable playerHealth = collision.gameObject.GetComponent<IDamageable>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damageToPlayer);
                    Debug.Log("มอนสเตอร์ชนผู้เล่น! เสีย 1 HP");

                    // ทำดาเมจเสร็จ ตั้งเวลาคูลดาวน์รอบต่อไปทันที
                    nextDamageTime = Time.time + damageCooldown;
                }
            }
        }
    }

    // เช็คเมื่อศัตรูเดินเข้าฐาน (อันนี้ใช้ Enter ถูกแล้วครับ เพราะเข้าปุ๊บหายตัวปั๊บ)
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Base"))
        {
            IDamageable baseHealth = collider.gameObject.GetComponent<IDamageable>();
            if (baseHealth != null)
            {
                baseHealth.TakeDamage(damageToBase);
                Debug.Log("มอนสเตอร์เข้าฐาน! ฐานเสีย 1 HP");
            }

            // เมื่อเข้าฐานแล้ว ศัตรูต้องทำลายตัวเอง (หรือซ่อนตัว)
            gameObject.SetActive(false);
        }
    }
}