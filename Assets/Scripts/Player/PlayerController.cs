using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Animation")]
    public Animator anim;
    private SpriteRenderer[] allSprites;

    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public float groundPoundForce = 20f;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Jump Feel")]
    public float fallMultiplier = 2.5f;

    [Header("Attached Particles")]
    public ParticleSystem dashParticle;
    public ParticleSystem doubleJumpParticle;
    public ParticleSystem groundPoundParticle;
    public GameObject playerDeathPrefab;

    [Header("Ground Pound Settings")]
    [Tooltip("เวลาหน่วง (วินาที) ก่อนที่เอฟเฟกต์พุ่งลงพื้นจะแสดง")]
    public float particleDelay = 0.15f;
    [Tooltip("เวลาหน่วง (วินาที) ที่เอฟเฟกต์จะค้างอยู่หลังจากแตะพื้นแล้ว")]
    public float particleStopDelay = 0.2f; // <--- เพิ่มตัวแปรหน่วงเวลาตอนหยุด

    private float groundPoundTimer = 0f;
    private bool hasPlayedGPEffect = false;
    private Coroutine stopParticleCoroutine; // <--- เก็บสถานะการนับเวลาปิด

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isGrounded;

    [HideInInspector] public bool isGroundPounding = false;

    private bool canDoubleJump = false;
    private bool isDashing = false;
    private float dashCooldownTimer = 0f;
    private float originalGravity;
    private float lastFacingDirection = 1f;

    public void SpawnPlayerDeathParticle()
    {
        if (ParticleManager.Instance != null)
        {
            ParticleManager.Instance.PlayPlayerDeath(transform.position);
        }
    }

    void Start()
    {
        if (anim == null) anim = GetComponent<Animator>();
        allSprites = GetComponentsInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale;
    }

    void Update()
    {
        if (isDashing) return;

        dashCooldownTimer -= Time.deltaTime;

        // 1. เช็คพื้น (เฉพาะเลเยอร์ Ground)
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 2. คืนสิทธิ์ Double Jump เมื่อแตะพื้น
        if (isGrounded && rb.linearVelocity.y >= -0.1f)
        {
            canDoubleJump = true;
        }

        // จับเวลาเริ่ม Ground Pound
        if (isGroundPounding)
        {
            groundPoundTimer += Time.deltaTime;
            if (groundPoundTimer >= particleDelay && !hasPlayedGPEffect)
            {
                if (groundPoundParticle != null) groundPoundParticle.Play();
                hasPlayedGPEffect = true;
            }
        }

        // 3. เงื่อนไขหยุด Ground Pound
        if (isGroundPounding && (isGrounded || rb.linearVelocity.y >= -0.1f))
        {
            isGroundPounding = false;

            // แทนที่จะปิดทันที ให้สั่งหน่วงเวลาปิดผ่าน Coroutine
            if (groundPoundParticle != null)
            {
                if (stopParticleCoroutine != null) StopCoroutine(stopParticleCoroutine);
                stopParticleCoroutine = StartCoroutine(StopParticleDelayed());
            }

            if (ParticleManager.Instance != null && isGrounded)
            {
                ParticleManager.Instance.PlayStompImpact(groundCheck.position);
            }
        }

        // รับค่าเดิน
        horizontalInput = Input.GetAxisRaw("Horizontal");
        if (horizontalInput != 0)
        {
            lastFacingDirection = Mathf.Sign(horizontalInput);

            if (horizontalInput > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (horizontalInput < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        // กระโดด และ พุ่งลงพื้น
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                rb.gravityScale = originalGravity;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
            else
            {
                if (Input.GetAxisRaw("Vertical") < 0 && !isGroundPounding)
                {
                    if (rb.linearVelocity.y < 2f)
                    {
                        rb.gravityScale = originalGravity;
                        rb.linearVelocity = new Vector2(rb.linearVelocity.x, -groundPoundForce);
                        isGroundPounding = true;

                        groundPoundTimer = 0f;
                        hasPlayedGPEffect = false;

                        // ถ้าระบบกำลังนับเวลาปิด Particle ของรอบที่แล้วอยู่ ให้ยกเลิกการปิดไปเลย
                        if (stopParticleCoroutine != null)
                        {
                            StopCoroutine(stopParticleCoroutine);
                            stopParticleCoroutine = null;
                        }
                        if (groundPoundParticle != null)
                        {
                            groundPoundParticle.Stop();
                        }
                    }
                }
                else if (canDoubleJump && EnchantManager.Instance != null && EnchantManager.Instance.hasDoubleJump)
                {
                    rb.gravityScale = originalGravity;
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                    canDoubleJump = false;

                    if (doubleJumpParticle != null) doubleJumpParticle.Play();
                }
            }
        }

        // Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0f)
        {
            if (EnchantManager.Instance != null && EnchantManager.Instance.hasDash)
            {
                StartCoroutine(DashRoutine());
            }
        }

        UpdateAnimations();
    }

    private void UpdateAnimations()
    {
        if (anim == null) return;
        anim.SetFloat("Speed", Mathf.Abs(horizontalInput));
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetFloat("VelocityY", rb.linearVelocity.y);
        anim.SetBool("IsGroundPounding", isGroundPounding);
        anim.SetBool("IsDashing", isDashing);
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        if (isGrounded && horizontalInput == 0 && rb.linearVelocity.y <= 0f)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            rb.gravityScale = originalGravity;
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        }
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;

        if (isGroundPounding)
        {
            isGroundPounding = false;
            // ถ้า Dash กลางอากาศ ให้ปิดเอฟเฟกต์ทันที ไม่ต้องรอดีเลย์
            if (groundPoundParticle != null) groundPoundParticle.Stop();
        }

        if (dashParticle != null) dashParticle.Play();

        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(lastFacingDirection * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (dashParticle != null) dashParticle.Stop();
    }

    // --- ฟังก์ชัน Coroutine สำหรับนับเวลาถอยหลังปิดเอฟเฟกต์ ---
    private IEnumerator StopParticleDelayed()
    {
        yield return new WaitForSeconds(particleStopDelay);
        if (groundPoundParticle != null)
        {
            groundPoundParticle.Stop();
        }
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