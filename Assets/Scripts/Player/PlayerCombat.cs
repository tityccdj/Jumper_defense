using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Stomp Settings")]
    public Transform stompCheck;
    public float stompRadius = 0.3f;
    public LayerMask enemyLayer;
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

    void CheckStomp()
    {
        Collider2D hit = Physics2D.OverlapCircle(stompCheck.position, stompRadius, enemyLayer);

        if (hit != null)
        {
            bool hitSomething = false;

            // 1. เช็คหนาม
            SpikeArmor spike = hit.GetComponent<SpikeArmor>();
            if (spike != null)
            {
                if (EnchantManager.Instance != null && EnchantManager.Instance.hasSpikeImmune)
                {
                    Debug.Log("เหยียบหนาม แต่มี Spike Immune! ไม่เสียเลือดแถมเหยียบมันตายได้ด้วย!");
                    IDamageable damageable = hit.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(stompDamage);
                    }
                }
                else
                {
                    IDamageable playerHealth = GetComponent<IDamageable>();
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(spike.recoilDamage);
                        Debug.Log("โอ๊ย! เหยียบโดนตัวหนาม ผู้เล่นเสียเลือด!");
                    }
                }
                hitSomething = true;
            }
            // 2. ถ้าไม่มีหนาม ทำดาเมจปกติ
            else
            {
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

            // --- ถ้าเหยียบโดนอะไรสักอย่างให้ทำสิ่งนี้ ---
            if (hitSomething)
            {
                // สั่งเล่น Particle ตรงตำแหน่งของศัตรู/ป้อม ที่ถูกเหยียบ
                if (ParticleManager.Instance != null)
                {
                    ParticleManager.Instance.PlayStompImpact(stompCheck.position);
                }

                Bounce();
                controller.isGroundPounding = false;
            }
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