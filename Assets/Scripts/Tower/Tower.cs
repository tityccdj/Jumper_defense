using System.Collections;
using UnityEngine;

public class Tower : MonoBehaviour, IBuffable
{
    [Header("Tower Settings")]
    public float attackRange = 5f;
    public float fireRate = 1f;
    public LayerMask enemyLayer;

    [Header("Rotation Setup")]
    public Transform partToRotate;
    public float rotationSpeed = 15f;

    [Header("Buff Settings & Visuals")]
    public float buffDuration = 2.5f;
    public Color buffColor = Color.yellow; // สีตอนที่ติดบัฟ (ค่าเริ่มต้นสีเหลือง)
    public float blinkSpeed = 0.15f;       // ความเร็วกระพริบ

    [Header("References")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    private bool isBuffed = false;
    private float buffTimer = 0f;
    private Transform currentTarget;
    private float nextFireTime;

    // ตัวแปรสำหรับจำสีเดิม
    private SpriteRenderer[] allSprites;
    private Color[] originalColors;

    void Start()
    {
        // 1. หาชิ้นส่วน Sprite ทั้งหมดและจำสีเดิมไว้ตั้งแต่เริ่มเกม
        allSprites = GetComponentsInChildren<SpriteRenderer>();
        originalColors = new Color[allSprites.Length];

        for (int i = 0; i < allSprites.Length; i++)
        {
            originalColors[i] = allSprites[i].color;
        }
    }

    void Update()
    {
        // จัดการเวลานับถอยหลังของบัฟ
        if (isBuffed)
        {
            buffTimer -= Time.deltaTime;
            if (buffTimer <= 0)
            {
                isBuffed = false;
                // เมื่อหมดเวลา Coroutine ด้านล่างจะหยุดทำงาน และคืนสีให้เอง
            }
        }

        UpdateTarget();

        if (currentTarget != null)
        {
            RotateTowardsTarget();

            float currentFireRate = isBuffed ? fireRate * 2f : fireRate;

            if (Time.time >= nextFireTime)
            {
                Shoot(currentTarget);
                nextFireTime = Time.time + (1f / currentFireRate);
            }
        }
    }

    public void ApplyBuff()
    {
        buffTimer = buffDuration; // รีเซ็ตเวลาบัฟเป็น 2.5 วินาที

        // ถ้าป้อมยังไม่ได้ติดบัฟอยู่ ให้เริ่มเล่นเอฟเฟกต์กระพริบ
        // (เช็คเพื่อป้องกันเวลากระโดดเหยียบซ้ำๆ แล้วแสงกระพริบรวน)
        if (!isBuffed)
        {
            isBuffed = true;
            StartCoroutine(BuffFlashRoutine());
        }

        Debug.Log("ป้อมได้รับบัฟ! ยิงรัวขึ้น 2 เท่า!");
    }

    private IEnumerator BuffFlashRoutine()
    {
        // ทำงานวนไปเรื่อยๆ ตราบใดที่ isBuffed ยังเป็น true
        while (isBuffed)
        {
            // เปลี่ยนเป็นสีบัฟ
            for (int i = 0; i < allSprites.Length; i++)
            {
                if (allSprites[i] != null) allSprites[i].color = buffColor;
            }

            yield return new WaitForSeconds(blinkSpeed);

            // คืนเป็นสีดั้งเดิม
            for (int i = 0; i < allSprites.Length; i++)
            {
                if (allSprites[i] != null) allSprites[i].color = originalColors[i];
            }

            yield return new WaitForSeconds(blinkSpeed);
        }

        // ชัวร์อีกรอบ! เมื่อบัฟหมดเวลา บังคับให้สีกลับมาเป็นปกติ 100%
        for (int i = 0; i < allSprites.Length; i++)
        {
            if (allSprites[i] != null) allSprites[i].color = originalColors[i];
        }
    }

    // --- โค้ดส่วนล่างเหมือนเดิมทั้งหมด ---
    void UpdateTarget()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        Transform closestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Collider2D enemy in enemies)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                closestEnemy = enemy.transform;
            }
        }
        currentTarget = closestEnemy;
    }

    void RotateTowardsTarget()
    {
        Vector2 direction = currentTarget.position - partToRotate.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        partToRotate.rotation = Quaternion.Lerp(partToRotate.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    void Shoot(Transform target)
    {
        GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bulletScript = bulletGO.GetComponent<Bullet>();
        if (bulletScript != null) bulletScript.target = target;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}