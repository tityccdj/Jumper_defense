using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Transform target;
    public float speed = 10f;
    public int damage = 2; // ดาเมจตาม GDD สำหรับ Normal Tower

    [Header("Ice Tower Settings")]
    public bool isIceBullet = false;    // ติ๊กถูกถ้าเป็นกระสุนน้ำแข็ง
    public float slowMultiplier = 0.5f; // ความเร็วเดินเหลือ 50%
    public float slowDuration = 2f;     // แช่แข็งนาน 2 วินาที

    void Update()
    {
        // ถ้าศัตรูตายไปแล้ว (เป้าหมายหายไป) ให้ทำลายกระสุนทิ้ง
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            Destroy(gameObject);
            return;
        }

        // วิ่งเข้าหาเป้าหมาย
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // เช็คว่าชนกับเป้าหมายที่ล็อกไว้ใช่ไหม
        if (collision.transform == target)
        {
            // 1. ทำดาเมจปกติ
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }

            // 2. [เพิ่มใหม่] ถ้าเป็นกระสุนน้ำแข็ง ให้สั่งศัตรูเดินช้าลง!
            if (isIceBullet)
            {
                EnemyMovement em = collision.GetComponent<EnemyMovement>();
                if (em != null)
                {
                    em.ApplySlow(slowMultiplier, slowDuration);
                }
            }

            // ชนแล้วทำลายกระสุนตัวเองทิ้ง
            Destroy(gameObject);
        }
    }
}