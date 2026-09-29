using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    public int damageToPlayer = 1;
    public int damageToBase = 1;

    // เช็คการเดินชนกับผู้เล่น (ฟิสิกส์ทั่วไป)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // ดึงระบบเลือดของผู้เล่นมาลด HP
            // ดึงสคริปต์ PlayerController ของผู้เล่นมาเช็คสถานะ
            PlayerController playerCtrl = collision.gameObject.GetComponent<PlayerController>();

            // ถ้าดึงมาได้ และผู้เล่นกำลังใช้ท่า Ground Pound อยู่
            if (playerCtrl != null && playerCtrl.isGroundPounding)
            {
                return; // ข้ามการทำดาเมจใส่ผู้เล่นไปเลย ปล่อยให้ PlayerCombat จัดการเหยียบ
            }

            // ถ้าไม่ได้ใช้ท่า Ground Pound ก็โดนดาเมจตามปกติ
            IDamageable playerHealth = collision.gameObject.GetComponent<IDamageable>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageToPlayer);
                Debug.Log("มอนสเตอร์ชนผู้เล่น! เสีย 1 HP");
            }
        }
    }

    // เช็คเมื่อศัตรูเดินเข้าฐาน (เพราะฐานเราตั้งเป็น Trigger ไว้)
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