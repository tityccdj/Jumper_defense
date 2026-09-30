using UnityEngine;

public class RangerShooter : MonoBehaviour
{
    [Header("Combat")]
    public GameObject bulletPrefab;
    public Transform gunPivot;
    public Transform firePoint;
    public float attackRange = 6f;
    public float fireRate = 2f;

    // --- เพิ่มช่องปรับความเร็วการหมุนตรงนี้ครับ ---
    [Header("Aim Settings")]
    public float turnSpeed = 150f;  // ความเร็วในการหมุนปืน (องศาต่อวินาที) ค่าน้อย = หมุนช้า, ค่ามาก = หมุนเร็ว

    private Transform player;
    private float nextFireTime;
    private MonoBehaviour moveScript;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        moveScript = GetComponent("EnemyMovement") as MonoBehaviour;
    }

    void Update()
    {
        if (player == null || !player.gameObject.activeInHierarchy)
        {
            if (moveScript != null) moveScript.enabled = true;
            return;
        }

        // 1. ค่อยๆ หมุนปืนเล็งตามผู้เล่นตลอดเวลา
        AimAtPlayer();

        // 2. วัดระยะห่าง
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            if (moveScript != null) moveScript.enabled = false; // หยุดเดิน

            // ยิง
            if (Time.time >= nextFireTime)
            {
                Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                nextFireTime = Time.time + fireRate;
            }
        }
        else
        {
            if (moveScript != null) moveScript.enabled = true; // เดินต่อ
        }
    }

    // แก้ไขฟังก์ชันหมุนปืน ให้หมุนแบบมีดีเลย์
    private void AimAtPlayer()
    {
        if (gunPivot != null && player != null)
        {
            // 1. หาเวกเตอร์ทิศทาง
            Vector2 direction = player.position - gunPivot.position;

            // 2. คำนวณหาองศาเป้าหมายที่ควรจะหันไป
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

            // 3. สั่งให้ค่อยๆ หมุนจาก "องศาปัจจุบัน" ไปหา "องศาเป้าหมาย" ด้วยความเร็ว turnSpeed
            gunPivot.rotation = Quaternion.RotateTowards(gunPivot.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}