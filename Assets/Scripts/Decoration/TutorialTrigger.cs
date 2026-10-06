using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class TutorialTrigger : MonoBehaviour
{
    [Header("Tutorial Settings")]
    [Tooltip("ลาก UI Panel หน้าต่างสอนเล่นของจุดนี้มาใส่")]
    public GameObject tutorialUI;

    [Tooltip("ต้องการให้เกมหยุดชั่วคราวตอนป้ายสอนเด้งไหม?")]
    public bool pauseGame = false;

    [Tooltip("ถ้าติ๊กถูก ป้ายนี้จะเด้งแค่ครั้งเดียวต่อการเล่น")]
    public bool showOnlyOnce = true;

    // --- ส่วนที่เพิ่มเข้ามาใหม่ ---
    [Header("Auto Close Settings")]
    [Tooltip("เปิดให้ปิดหน้าต่างอัตโนมัติเมื่อกดเดิน, กระโดด หรือคลิก/แตะจอ")]
    public bool closeOnAnyInput = true;
    // ----------------------

    [Header("Optional Actions")]
    [Tooltip("อยากให้ทำอะไรเพิ่มเติมตอนเดินชนไหม? (ใส่ Event ได้)")]
    public UnityEvent onTutorialTriggered;

    private bool hasTriggered = false;
    private bool isTutorialActive = false;
    private bool canCloseNow = false; // ตัวแปรป้องกันการปิดลั่นตอนเดินชน

    private void Start()
    {
        if (tutorialUI != null)
        {
            tutorialUI.SetActive(false);
        }

        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void Update()
    {
        // ถ้าหน้าสอนเล่นเปิดอยู่ และอนุญาตให้กดเพื่อปิดได้แล้ว
        if (isTutorialActive && closeOnAnyInput && canCloseNow)
        {
            // เช็คว่ามีการกดปุ่มใดๆ หรือไม่ (คีย์บอร์ด, แตะจอ, คลิกเมาส์, หรือกดทิศทางเดิน)
            if (Input.anyKeyDown ||
                Input.GetMouseButtonDown(0) ||
                Input.GetAxisRaw("Horizontal") != 0 ||
                Input.GetAxisRaw("Vertical") != 0)
            {
                CloseTutorial();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && (!hasTriggered || !showOnlyOnce))
        {
            hasTriggered = true;
            isTutorialActive = true;
            canCloseNow = false; // ห้ามปิดทันทีที่โชว์

            if (tutorialUI != null)
            {
                tutorialUI.SetActive(true);
            }

            if (pauseGame)
            {
                Time.timeScale = 0f;
            }

            onTutorialTriggered?.Invoke();

            // เริ่มรัน Coroutine หน่วงเวลาก่อนอนุญาตให้ปิด
            StartCoroutine(EnableCloseDelay());
        }
    }

    // ฟังก์ชันหน่วงเวลา 0.5 วินาที
    private IEnumerator EnableCloseDelay()
    {
        // ใช้ Realtime เผื่อกรณีที่คุณตั้ง pauseGame = true (เวลาเกมเป็น 0) มันจะได้นับเวลาต่อได้
        yield return new WaitForSecondsRealtime(0.5f);
        canCloseNow = true;
    }

    public void CloseTutorial()
    {
        if (!isTutorialActive) return;

        isTutorialActive = false;

        if (tutorialUI != null)
        {
            tutorialUI.SetActive(false);
        }

        if (pauseGame)
        {
            Time.timeScale = 1f; // คืนค่าเวลาให้เกมเดินต่อ
        }
    }
}