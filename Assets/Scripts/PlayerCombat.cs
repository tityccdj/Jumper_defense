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
            // **จุดสำคัญ:** พระเอกของเรา (Interface) มาแล้ว!
            // เราไม่ต้องเช็คว่ามันคือ BaseEnemy หรือบอส แค่ถามว่ามันมี IDamageable ไหม
            IDamageable damageable = hit.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(stompDamage); // สั่งทำดาเมจ 10
                Bounce(); // สั่งให้ Player เด้งขึ้น
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