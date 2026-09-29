using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Transform target;
    public float speed = 10f;
    public int damage = 2; // ดาเมจตาม GDD สำหรับ Normal Tower

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
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
            // ชนแล้วทำลายกระสุนตัวเองทิ้ง
            Destroy(gameObject);
        }
    }
}