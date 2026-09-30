using UnityEngine;

public class RangerBullet : MonoBehaviour
{
    public float speed = 10f; // อาจจะต้องปรับเลขนี้เพิ่มขึ้นจากเดิม เพราะตอนนี้มันคือ "แรงดีด" แล้ว
    public int damage = 1;

    private Rigidbody2D rb;

    void Start()
    {
        // 1. ดึง Rigidbody2D มาใช้งาน
        rb = GetComponent<Rigidbody2D>();

        // 2. ออกแรงดีดกระสุนไปด้านหน้า (ตามองศาที่ปืนเล็งไว้) แค่ครั้งเดียว
        if (rb != null)
        {
            rb.linearVelocity = transform.right * speed;
        }

        // ทำลายตัวเองใน 3 วินาที (กันขยะเต็มฉาก)
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        // 3. (แถมความเนียน) สั่งให้หัวกระสุนเชิดขึ้น-ปักลง ตามทิศทางที่มันกำลังพุ่งหรือย้อยลงมา
        if (rb != null && rb.linearVelocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            IDamageable playerHealth = collision.GetComponent<IDamageable>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}