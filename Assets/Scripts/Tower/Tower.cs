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

    // --- ส่วนที่เพิ่มเข้ามาใหม่สำหรับระบบอัปเกรด ---
    [Header("Upgrade Settings (ใช้ Shard)")]
    public GameObject upgradedTowerPrefab; // ใส่ Prefab ป้อมร่าง 2 ที่จะเอามาแทนที่
    public bool isMaxLevel = false;        // ถ้าเป็นร่าง 2 แล้วให้ติ๊กถูกอันนี้
    // ----------------------------------------

    private bool isBuffed = false;
    private float buffTimer = 0f;
    private Transform currentTarget;
    private float nextFireTime;

    // ตัวแปรสำหรับจำสีเดิม
    private SpriteRenderer[] allSprites;
    private Color[] originalColors;

    [HideInInspector]
    public TowerNode myNode; // เอาไว้จำว่าป้อมนี้วางอยู่บน Node ไหน

    private void OnMouseDown()
    {
        // ถ้าคลิกโดนตัวป้อมโดยตรง ให้ส่งเรื่องไปเปิดหน้าต่างอัปเกรดเลย!
        if(ShopManager.Instance != null && myNode != null && !isMaxLevel)
        {
            ShopManager.Instance.OpenUpgradeShop(myNode);
        }
    }
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
                // เมื่อหมดเวลา Coroutine จะหยุดทำงาน และคืนสีให้เอง
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

        if (!isBuffed)
        {
            isBuffed = true;
            StartCoroutine(BuffFlashRoutine());
        }

        Debug.Log("ป้อมได้รับบัฟ! ยิงรัวขึ้น 2 เท่า!");
    }

    private IEnumerator BuffFlashRoutine()
    {
        while (isBuffed)
        {
            for (int i = 0; i < allSprites.Length; i++)
            {
                if (allSprites[i] != null) allSprites[i].color = buffColor;
            }

            yield return new WaitForSeconds(blinkSpeed);

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

    // --- ส่วนที่เพิ่มเข้ามาใหม่ ฟังก์ชันสั่งอัปเกรดป้อม ---

    // ---------------------------------------------

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}