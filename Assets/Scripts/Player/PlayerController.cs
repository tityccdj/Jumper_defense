using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public float groundPoundForce = 20f; // ความแรงตอนพุ่งลงมาเหยียบ

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Jump Feel")]
    public float fallMultiplier = 2.5f;

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isGrounded;
    [HideInInspector] public bool isGroundPounding = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 1. เช็คว่าเท้าติดพื้นไหม (สร้างวงกลมเล็กๆ ที่ตำแหน่ง groundCheck)
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded && rb.linearVelocity.y >= -0.1f)
        {
            isGroundPounding = false;
        }
        // 2. รับค่าปุ่มเดินซ้ายขวา (A/D หรือ ลูกศร)
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // 3. กระโดด (กด Spacebar และต้องยืนอยู่บนพื้น)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // 4. ระบบ Ground Pound: พุ่งลงมาเหยียบ (อยู่กลางอากาศ + กดล่าง + กดกระโดด)
        if (!isGrounded && !isGroundPounding && Input.GetAxisRaw("Vertical") < 0 && Input.GetButtonDown("Jump") && rb.linearVelocity.y < 2f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -groundPoundForce);
            isGroundPounding = true;
        }
        //if (rb.linearVelocity.y < 0)
        //{
        //    rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        //}
    }

    void FixedUpdate()
    {
        // ใส่แรงเดินใน FixedUpdate เพื่อให้ฟิสิกส์เสถียร
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    // วาดวงกลมสีแดงใน Scene View เพื่อให้เราเห็นระยะของ Ground Check
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}