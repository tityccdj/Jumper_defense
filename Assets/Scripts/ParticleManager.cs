using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    // ทำเป็น Singleton เพื่อให้ทุกสคริปต์เรียกใช้ได้ง่ายๆ ผ่าน ParticleManager.Instance
    public static ParticleManager Instance { get; private set; }

    [Header("Player Effects")]
    public GameObject stompImpactPrefab; // ฝุ่นตอนพุ่งกระทืบพื้น
    public GameObject dashEffectPrefab;  // เอฟเฟกต์ตอนพุ่ง Dash
    public GameObject doubleJumpPrefab;  // เมฆเล็กๆ ตอนกระโดด 2 ชั้น

    [Header("Enemy Effects")]
    public GameObject enemyHitPrefab;    // เลือดกระจาย หรือประกายไฟตอนโดนตี
    public GameObject enemyDeathPrefab;  // ระเบิดตอนศัตรูตาย

    [Header("Tower & System Effects")]
    public GameObject towerUpgradePrefab; // แสงวิบวับตอนอัปเกรดป้อม

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- ฟังก์ชันหลักสำหรับสร้างและทำลาย Particle อัตโนมัติ ---
    // destroyDelay คือเวลาที่จะลบ GameObject ทิ้ง ป้องกันขยะล้นฉาก (ค่าเริ่มต้น 2 วินาที)
    private void SpawnParticle(GameObject prefab, Vector3 position, float destroyDelay = 2f)
    {
        if (prefab != null)
        {
            // เปลี่ยนจาก Quaternion.identity เป็น prefab.transform.rotation
            GameObject fx = Instantiate(prefab, position, prefab.transform.rotation);
            Destroy(fx, destroyDelay);
        }
        else
        {
            Debug.LogWarning("ลืมใส่ Prefab ใน ParticleManager หรือเปล่า?");
        }
    }

    // --- ฟังก์ชันย่อย ให้สคริปต์อื่นเรียกใช้ง่ายๆ ---

    public void PlayStompImpact(Vector3 position)
    {
        SpawnParticle(stompImpactPrefab, position);
    }

    public void PlayDashEffect(Vector3 position)
    {
        SpawnParticle(dashEffectPrefab, position, 1f); // Dash แป๊บเดียวหาย ตั้งลบทิ้งเร็วนิดนึง
    }

    public void PlayDoubleJumpEffect(Vector3 position)
    {
        SpawnParticle(doubleJumpPrefab, position, 1f);
    }

    public void PlayEnemyHit(Vector3 position)
    {
        SpawnParticle(enemyHitPrefab, position, 1f);
    }

    public void PlayEnemyDeath(Vector3 position)
    {
        SpawnParticle(enemyDeathPrefab, position, 3f);
    }

    public void PlayTowerUpgrade(Vector3 position)
    {
        SpawnParticle(towerUpgradePrefab, position, 3f);
    }
}