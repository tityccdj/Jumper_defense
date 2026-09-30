using UnityEngine;

public class RangerBullet : MonoBehaviour
{
    public float speed = 7f;
    public int damage = 1;

    void Start()
    {
        // ทำลายตัวเองใน 3 วินาที
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        // Vector3.right คือแกน X ของตัวกระสุนเอง 
        // เนื่องจากตอนเกิดมันจะถูกหมุนองศาให้ตรงกับปืน การพุ่งไปแกน X จึงพุ่งตรงไปหา Player พอดีครับ
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. ถ้าชนกับศัตรูด้วยกันเอง (คนยิง หรือเพื่อน) ให้ข้ามคำสั่งด้านล่างไปเลย กระสุนจะได้ไม่พัง
        if (collision.CompareTag("Enemy"))
        {
            return;
        }

        // 2. ถ้าชนกับ Player ให้ทำดาเมจ
        if (collision.CompareTag("Player"))
        {
            IDamageable playerHealth = collision.GetComponent<IDamageable>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }

        // 3. ทำลายกระสุนทิ้ง (ไม่ว่าจะชน Player, ชนกำแพง, หรือชนพื้น)
        Destroy(gameObject);
    }
}