using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] // บังคับว่าต้องมี Rigidbody2D ถึงจะใช้สคริปต์นี้ได้
public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // บังคับความเร็วแกน X ให้ติดลบ (เดินไปทางซ้าย) ส่วนแกน Y ปล่อยให้ตกตามแรงโน้มถ่วง
        rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
    }
}