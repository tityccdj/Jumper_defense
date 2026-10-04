using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Stomp Settings")]
    public Transform stompCheck;
    public float stompRadius = 0.4f; // ขยายรัศมีอีกนิดให้โดนง่ายขึ้น

    [Tooltip("ใส่ Layer ของศัตรูและป้อม (ติ๊กถูกทั้งคู่)")]
    public LayerMask targetLayer; // <--- เปลี่ยนชื่อจาก enemyLayer เป็น targetLayer

    public int stompDamage = 10;
    public float bounceForce = 15f;

    private Rigidbody2D rb;
    private PlayerController controller;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = rb.GetComponent<PlayerController>();
    }

    void Update()
    {
        if (controller.isGroundPounding)
        {
            CheckStomp();
        }
    }

    // เปลี่ยนเป็น public เพื่อให้ Controller เรียกใช้ฉุกเฉินได้
    public void CheckStomp()
    {
        // ใช้ OverlapCircleAll เพื่อกวาดหา "ทุกอย่าง" ในรัศมี (แก้ปัญหาศัตรูซ้อนกับป้อม)
        Collider2D[] hits = Physics2D.OverlapCircleAll(stompCheck.position, stompRadius, targetLayer);

        bool hitSomething = false;

        foreach (Collider2D hit in hits)
        {
            // 1. เช็คหนาม
            SpikeArmor spike = hit.GetComponent<SpikeArmor>();
            if (spike != null)
            {
                if (EnchantManager.Instance != null && EnchantManager.Instance.hasSpikeImmune)
                {
                    IDamageable damageable = hit.GetComponent<IDamageable>();
                    if (damageable != null) damageable.TakeDamage(stompDamage);
                }
                else
                {
                    IDamageable playerHealth = GetComponent<IDamageable>();
                    if (playerHealth != null) playerHealth.TakeDamage(spike.recoilDamage);
                }
                hitSomething = true;
            }
            else
            {
                // 2. ถ้าไม่มีหนาม ทำดาเมจปกติ
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(stompDamage);
                    hitSomething = true;
                }
            }

            // 3. เช็คเหยียบป้อม
            IBuffable buffable = hit.GetComponent<IBuffable>();
            if (buffable != null)
            {
                buffable.ApplyBuff();
                hitSomething = true;
            }
        }

        // --- ถ้าเหยียบโดนอะไรสักอย่างให้ทำสิ่งนี้ ---
        if (hitSomething)
        {
            if (ParticleManager.Instance != null)
            {
                ParticleManager.Instance.PlayStompImpact(stompCheck.position);
            }

            Bounce();
            controller.isGroundPounding = false;
        }
    }

    void Bounce()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);
    }

    private void OnDrawGizmosSelected()
    {
        if (stompCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(stompCheck.position, stompRadius);
        }
    }
}