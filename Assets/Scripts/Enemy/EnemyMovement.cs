using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    private Rigidbody2D rb;
    private Transform targetBase; // ตัวแปรสำหรับเก็บตำแหน่งฐาน
    private float originalSpeed;
    private Coroutine slowCoroutine;

    void Start()
    {
        originalSpeed = moveSpeed;
        moveSpeed = moveSpeed + Random.Range(-0.3f, 0.3f);
        rb = GetComponent<Rigidbody2D>();

        // 1. ให้ศัตรูมองหาวัตถุที่มี Tag ว่า "Base" ตั้งแต่ตอนเกิด
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
            // 1. หาว่าระยะห่างแกน X ห่างกันเท่าไหร่
            float distanceX = targetBase.position.x - transform.position.x;
            float moveDirectionX = 0f;

            // 2. ถ้าห่างกันมากกว่า 0.1 ค่อยเดิน (ป้องกันการสั่น)
            if (Mathf.Abs(distanceX) > 0.1f)
            {
                moveDirectionX = Mathf.Sign(distanceX);
            }

            // 3. เดินไปในทิศทางนั้น (ถ้าตรงกันแล้ว moveDirectionX จะเป็น 0 ทำให้ร่วงลงมาตรงๆ อย่างเดียว)
            rb.linearVelocity = new Vector2(moveDirectionX * moveSpeed, rb.linearVelocity.y);

            // 4. หันหน้า
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

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = new Color(0.5f, 0.8f, 1f); // ตัวฟ้าแข็ง

        yield return new WaitForSeconds(duration);

        moveSpeed = originalSpeed; // คืนความเร็ว
        if (sr != null) sr.color = Color.white;
    }
}
