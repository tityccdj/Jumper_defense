using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Stomp Settings")]
    public Transform stompCheck;       // จุดที่ใช้เช็คการเหยียบ (ปลายเท้า)
    public float stompRadius = 0.3f;   // รัศมีการเหยียบ
    public LayerMask enemyLayer;       // เลเยอร์ของศัตรู
    public int stompDamage = 10;       // ดาเมจที่ทำได้เมื่อเหยียบ
    public float bounceForce = 15f;    // แรงเด้งหลังจากเหยียบโดน

    private Rigidbody2D rb;
    private PlayerController controller;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = rb.GetComponent<PlayerController>();
    }

    void Update()
    {
        // เงื่อนไข: เราจะทำดาเมจได้ก็ต่อเมื่อ "ตัวละครกำลังตกลงมา" (ความเร็วแกน Y ติดลบ) 
        // ป้องกันบั๊กเวลาเรากระโดดเอาหัวไปโหม่งมอนสเตอร์จากด้านล่าง
        if (controller.isGroundPounding)
        {
            CheckStomp();
        }
    }

    void CheckStomp()
    {
        // สร้างวงกลมเช็คการชนกับเลเยอร์ Enemy
        Collider2D hit = Physics2D.OverlapCircle(stompCheck.position, stompRadius, enemyLayer);

        if (hit != null)
        {
            bool hitSomething = false;

            // 1. เช็คว่าเหยียบมอนสเตอร์ไหม (ทำดาเมจ)
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(stompDamage);
                hitSomething = true;
            }

            // 2. เช็คว่าเหยียบป้อมไหม (แจกบัฟ)
            IBuffable buffable = hit.GetComponent<IBuffable>();
            if (buffable != null)
            {
                buffable.ApplyBuff();
                hitSomething = true;
            }

            // ถ้าเหยียบโดนอะไรสักอย่าง (มอนหรือป้อม) ให้เด้ง
            if (hitSomething)
            {
                Bounce();
                controller.isGroundPounding = false;
            }
        }
    }

    void Bounce()
    {
        // รีเซ็ตความเร็วแนวดิ่งเป็น 0 ก่อนเด้ง เพื่อให้การเด้งได้ความสูงเท่ากันทุกครั้ง
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);
    }

    // วาดวงกลมสีเหลืองเพื่อให้เรากะระยะปลายเท้าได้ง่ายในหน้า Scene
    private void OnDrawGizmosSelected()
    {
        if (stompCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(stompCheck.position, stompRadius);
        }
    }
}