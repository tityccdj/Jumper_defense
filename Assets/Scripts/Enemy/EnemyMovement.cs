using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    private Rigidbody2D rb;
    private Transform targetBase; // ตัวแปรสำหรับเก็บตำแหน่งฐาน
    private float originalSpeed;
    private Coroutine slowCoroutine;

    // --- เพิ่ม 2 ตัวแปรนี้ เพื่อจำชิ้นส่วนภาพและสีดั้งเดิม ---
    private SpriteRenderer sr;
    private Color originalColor;

    [Header("Visual Effects")]
    public ParticleSystem slowParticle; // ตัวแปรสำหรับใส่ Particle น้ำแข็ง

    void Start()
    {
        originalSpeed = moveSpeed;
        moveSpeed = moveSpeed + Random.Range(-0.3f, 0.3f);
        rb = GetComponent<Rigidbody2D>();

        // 1. จำสีเดิมตั้งแต่ตอนเริ่มเกม
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            originalColor = sr.color;
        }

        // 2. ให้ศัตรูมองหาวัตถุที่มี Tag ว่า "Base" ตั้งแต่ตอนเกิด
        GameObject baseObj = GameObject.FindGameObjectWithTag("Base");
        if (baseObj != null)
        {
            targetBase = baseObj.transform;
        }
    }

    void FixedUpdate()
    {
        if (targetBase != null)
        {
            float distanceX = targetBase.position.x - transform.position.x;
            float moveDirectionX = 0f;

            if (Mathf.Abs(distanceX) > 0.1f)
            {
                moveDirectionX = Mathf.Sign(distanceX);
            }

            rb.linearVelocity = new Vector2(moveDirectionX * moveSpeed, rb.linearVelocity.y);

            if (moveDirectionX > 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (moveDirectionX < 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
        }
    }

    public void ApplySlow(float multiplier, float duration)
    {
        if (slowCoroutine != null) StopCoroutine(slowCoroutine);
        slowCoroutine = StartCoroutine(SlowRoutine(multiplier, duration));
    }

    private System.Collections.IEnumerator SlowRoutine(float multiplier, float duration)
    {
        moveSpeed = originalSpeed * multiplier; // เดินช้าลง

        if (sr != null) sr.color = new Color(0.5f, 0.8f, 1f); // เปลี่ยนเป็นสีฟ้าแข็ง

        // 1. สั่งเล่น Particle น้ำแข็ง
        if (slowParticle != null)
        {
            slowParticle.Play();
        }

        yield return new WaitForSeconds(duration);

        moveSpeed = originalSpeed; // คืนความเร็ว

        // เปลี่ยนจาก Color.white เป็นสีที่จำไว้
        if (sr != null) sr.color = originalColor;

        // 2. สั่งหยุด Particle เมื่อหมดเวลา
        if (slowParticle != null)
        {
            slowParticle.Stop();
        }
    }
}