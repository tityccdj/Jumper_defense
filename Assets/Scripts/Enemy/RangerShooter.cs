using UnityEngine;

public class RangerShooter : MonoBehaviour
{
    [Header("Combat")]
    public GameObject bulletPrefab;
    public Transform gunPivot;
    public Transform firePoint;
    public float attackRange = 6f;
    public float fireRate = 2f;

    [Header("Aim Settings")]
    public float turnSpeed = 150f;

    private Transform player;
    private float nextFireTime;
    private MonoBehaviour moveScript;
    private Rigidbody2D rb;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        moveScript = GetComponent("EnemyMovement") as MonoBehaviour;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null || !player.gameObject.activeInHierarchy)
        {
            if (moveScript != null) moveScript.enabled = true;
            return;
        }

        AimAtPlayer();

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            // ถ้าระยะถึง และกำลังเดินอยู่ ให้สั่งปิดสคริปต์เดิน + เหยียบเบรก
            if (moveScript != null && moveScript.enabled == true)
            {
                moveScript.enabled = false;
                if (rb != null) rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }

            // ยิง
            if (Time.time >= nextFireTime)
            {
                // ---------------------------------------------------------
                // --- วิธีแก้บั๊กกระสุนยิงกลับหลัง ---
                // 1. หาเวกเตอร์ทิศทางจาก 'โคนปืน' พุ่งไปหา 'ปลายกระบอกปืน'
                Vector2 actualShootDir = firePoint.position - gunPivot.position;

                // 2. แปลงเวกเตอร์นั้นเป็นองศาที่ถูกต้อง 100% ตามภาพที่เห็น
                float shootAngle = Mathf.Atan2(actualShootDir.y, actualShootDir.x) * Mathf.Rad2Deg;
                Quaternion correctRotation = Quaternion.Euler(0, 0, shootAngle);

                // 3. สั่งยิงโดยใช้องศาที่คำนวณใหม่แทน firePoint.rotation เดิม
                Instantiate(bulletPrefab, firePoint.position, correctRotation);
                // ---------------------------------------------------------

                nextFireTime = Time.time + fireRate;
            }
        }
        else
        {
            // ถ้านอกระยะ ให้เปิดสคริปต์กลับมาเดินต่อ
            if (moveScript != null && moveScript.enabled == false)
            {
                moveScript.enabled = true;
            }
        }
    }

    private void AimAtPlayer()
    {
        if (gunPivot != null && player != null)
        {
            Vector2 direction = player.position - gunPivot.position;

            // ไม้ตายแก้บั๊กกระจกสะท้อน (เพื่อให้สไปรต์ปืนไม่กลับหัว)
            if (transform.localScale.x < 0)
            {
                direction.x = -direction.x;
            }

            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

            gunPivot.localRotation = Quaternion.RotateTowards(gunPivot.localRotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}