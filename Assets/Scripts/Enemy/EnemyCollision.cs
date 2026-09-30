using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    public int damageToPlayer = 1;
    public int damageToBase = 1;

    [Header("Damage Cooldown")]
    public float damageCooldown = 1f;
    private float nextDamageTime = 0f;

    // รวบมาใช้ OnTriggerStay2D ทั้งหมด เพราะตอนนี้เราเป็น Hitbox (Is Trigger) แล้ว
    private void OnTriggerStay2D(Collider2D collider)
    {
        // 1. เช็คโดนผู้เล่น
        if (collider.gameObject.CompareTag("Player"))
        {
            PlayerController playerCtrl = collider.gameObject.GetComponent<PlayerController>();

            // ข้ามดาเมจถ้าผู้เล่นกำลังพุ่งลงมาเหยียบ
            bool isSpike = transform.parent.GetComponent<SpikeArmor>() != null;

            // ถ้าไม่ใช่หนาม และผู้เล่นกำลังพุ่งลงมา ให้ข้ามดาเมจไป
            // (แปลว่าถ้าเป็นหนาม มันจะไม่ข้ามดาเมจ ทิ่มผู้เล่นทันที!)
            if (!isSpike && playerCtrl != null && playerCtrl.isGroundPounding) return;

            if (Time.time >= nextDamageTime)
            {
                IDamageable playerHealth = collider.gameObject.GetComponent<IDamageable>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damageToPlayer);
                    Debug.Log("มอนสเตอร์ชนผู้เล่น! เสีย 1 HP");
                    nextDamageTime = Time.time + damageCooldown;
                }
            }
        }

        // 2. เช็คเข้าฐาน (ของเดิม)
        if (collider.gameObject.CompareTag("Base"))
        {
            IDamageable baseHealth = collider.gameObject.GetComponent<IDamageable>();
            if (baseHealth != null)
            {
                baseHealth.TakeDamage(damageToBase);
                Debug.Log("มอนสเตอร์เข้าฐาน! ฐานเสีย 1 HP");
            }

            // สั่งปิดตัวแม่ (ศัตรูทั้งตัว) เพราะตอนนี้สคริปต์อยู่บนตัวลูก(Hitbox) 
            transform.parent.gameObject.SetActive(false);
        }
    }
}