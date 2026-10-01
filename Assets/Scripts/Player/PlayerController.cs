using UnityEngine;
using System.Collections; // เพิ่มเพื่อให้ใช้ Coroutine ได้

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public float groundPoundForce = 20f;

    [Header("Dash Settings (ใหม่)")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

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

    // ตัวแปรสำหรับระบบใหม่
    private bool canDoubleJump = false;
    private bool isDashing = false;
    private float dashCooldownTimer = 0f;
    private float originalGravity;
    private float lastFacingDirection = 1f; // จำว่าหันหน้าไปทางไหน (1=ขวา, -1=ซ้าย)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale; // จำค่า Gravity เดิมไว้ใช้หลัง Dash เสร็จ
    }

    void Update()
    {
        // ถ้ากำลัง Dash อยู่ จะไม่รับคำสั่งอื่นเลย
        if (isDashing) return;

        dashCooldownTimer -= Time.deltaTime;

        // 1. เช็คพื้น
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded && rb.linearVelocity.y >= -0.1f)
        {
            isGroundPounding = false;
            canDoubleJump = true; // รีเซ็ตให้กลับมากระโดด 2 ชั้นได้อีกเมื่อเหยียบพื้น
        }

        // 2. รับค่าเดิน และจำทิศทาง (ใช้เวลา Dash)
        horizontalInput = Input.GetAxisRaw("Horizontal");
        if (horizontalInput != 0)
        {
            lastFacingDirection = Mathf.Sign(horizontalInput);
        }

        // 3. กระโดด และ Double Jump
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                // กรณียืนอยู่บนพื้น -> กระโดดครั้งแรกตามปกติ
                rb.gravityScale = originalGravity; // <--- เพิ่มบรรทัดนี้เพื่อปลดล็อกก่อนกระโดด
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
            else
            {
                // กรณีอยู่กลางอากาศ -> ให้ความสำคัญกับ Ground Pound (Stomp) ก่อน!
                if (Input.GetAxisRaw("Vertical") < 0 && !isGroundPounding)
                {
                    if (rb.linearVelocity.y < 2f)
                    {
                        rb.gravityScale = originalGravity; // <--- (เผื่อไว้) ปลดล็อกก่อนพุ่งเหยียบ
                        rb.linearVelocity = new Vector2(rb.linearVelocity.x, -groundPoundForce);
                        isGroundPounding = true;
                    }
                }
                else if (canDoubleJump && EnchantManager.Instance != null && EnchantManager.Instance.hasDoubleJump)
                {
                    rb.gravityScale = originalGravity; // <--- ปลดล็อกก่อน Double Jump
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                    canDoubleJump = false;
                    Debug.Log("ใช้งาน Double Jump!");
                }
            }
        }

        // 5. Dash (กดปุ่ม Left Shift เพื่อพุ่งตัว)
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0f)
        {
            if (EnchantManager.Instance != null && EnchantManager.Instance.hasDash)
            {
                StartCoroutine(DashRoutine());
            }
        }
    }

    void FixedUpdate()
    {
        if (isDashing) return; // ถ้า Dash อยู่ไม่ต้องคำนวณแรงเดินปกติ

        // --- จุดที่แก้: เพิ่ม && rb.linearVelocity.y <= 0f ---
        // แปลว่า: ถ้าแตะพื้น + ไม่ได้กดเดิน + ไม่ได้กำลังลอยขึ้น
        if (isGrounded && horizontalInput == 0 && rb.linearVelocity.y <= 0f)
        {
            rb.gravityScale = 0f; // ปิดแรงโน้มถ่วงชั่วคราว
            rb.linearVelocity = Vector2.zero; // บังคับหยุดนิ่งสนิท
        }
        else
        {
            rb.gravityScale = originalGravity; // คืนค่าแรงโน้มถ่วง
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        }
    }

    // Coroutine สำหรับระบบ Dash
    private IEnumerator DashRoutine()
    {
        isDashing = true;
        isGroundPounding = false; // ยกเลิกสถานะพุ่งลงพื้นเผื่อกดผิด
        dashCooldownTimer = dashCooldown;

        // ปิดแรงโน้มถ่วงชั่วคราว ให้พุ่งเป็นเส้นตรง
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(lastFacingDirection * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);

        // คืนค่าแรงโน้มถ่วง และหยุดพุ่ง
        rb.gravityScale = originalGravity;
        isDashing = false;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        Debug.Log("Dash เสร็จสิ้น!");
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}