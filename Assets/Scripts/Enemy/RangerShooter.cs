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
    private Rigidbody2D rb; // --- เพิ่มตัวแปรเบรก ---

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        moveScript = GetComponent("EnemyMovement") as MonoBehaviour;
        rb = GetComponent<Rigidbody2D>(); // --- ดึง Rigidbody มาใช้เบรก ---
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
            // --- ถ้าระยะถึง และกำลังเดินอยู่ ให้สั่งปิดสคริปต์เดิน + เหยียบเบรก! ---
            if (moveScript != null && moveScript.enabled == true)
            {
                moveScript.enabled = false;

                // สั่งเบรกแกน X ให้เป็น 0 (ส่วนแกน Y ปล่อยให้ตกลงตามแรงโน้มถ่วงปกติ)
                if (rb != null) rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }

            // ยิง
            if (Time.time >= nextFireTime)
            {
                Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                nextFireTime = Time.time + fireRate;
            }
        }
        else
        {
            // --- ถ้านอกระยะ ให้เปิดสคริปต์กลับมาเดินต่อ ---
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
            // 1. หาเวกเตอร์ทิศทางปกติ (เอาปลายทาง ลบ ต้นทาง)
            Vector2 direction = player.position - gunPivot.position;

            // 2. *** ไม้ตายแก้บั๊กกระจกสะท้อน ***
            // ถ้าตัวแม่หันไปทางซ้าย (Scale X ติดลบ) โลกของมันจะกลับด้าน
            // เราจึงต้องคูณทิศทางด้วย -1 เพื่อให้ผลลัพธ์การหมุนกลับมาถูกต้องเป๊ะ!
            if (transform.localScale.x < 0)
            {
                direction = -direction;
            }

            // 3. คำนวณองศาจาก direction ที่ปรับแก้แล้ว
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

            // 4. สั่งหมุนแบบ localRotation (หมุนเทียบกับตัวแม่)
            gunPivot.localRotation = Quaternion.RotateTowards(gunPivot.localRotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}