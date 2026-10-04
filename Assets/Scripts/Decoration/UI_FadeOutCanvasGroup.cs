using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UI_FadeOutCanvasGroup : MonoBehaviour
{
    [Header("Fade Settings")]
    [Tooltip("เวลาหน่วงก่อนที่จะเริ่มจางหาย (วินาที)")]
    public float delayBeforeFade = 1.0f;

    [Tooltip("ระยะเวลาที่ใช้ในการค่อยๆ จางจนเหลือ 0 (วินาที)")]
    public float fadeDuration = 1.0f;

    [Tooltip("ถ้าติ๊กถูก จะสั่ง SetActive(false) ปิด Object นี้ทันทีที่จางเสร็จ")]
    public bool disableOnComplete = true;

    private CanvasGroup canvasGroup;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        // ดึงคอมโพเนนต์ CanvasGroup มาใช้งาน
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // ทำงานอัตโนมัติเมื่อเปิด Object (SetActive = true)
    private void OnEnable()
    {
        // รีเซ็ตความชัดให้กลับมา 100% ทุกครั้งที่เปิดใช้งานใหม่
        canvasGroup.alpha = 1f;

        // เริ่มนับเวลาจางหาย
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        // 1. รอเวลาหน่วง (ใช้ WaitForSecondsRealtime เผื่อกรณีเกมถูก Pause หรือ Time.timeScale = 0)
        if (delayBeforeFade > 0)
        {
            yield return new WaitForSecondsRealtime(delayBeforeFade);
        }

        float timer = 0f;
        float startAlpha = canvasGroup.alpha;

        // 2. ค่อยๆ ลดค่า alpha ลงเรื่อยๆ ตามเวลาที่กำหนด
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime; // ใช้ unscaled เผื่อเกม Pause อยู่

            // คำนวณค่าความโปร่งใส
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, timer / fadeDuration);

            yield return null;
        }

        // 3. จางเสร็จสมบูรณ์ บังคับค่าให้เป็น 0 เป๊ะๆ
        canvasGroup.alpha = 0f;

        // 4. ถ้าตั้งค่าไว้ ให้ปิด Object นี้ทิ้งเพื่อประหยัดทรัพยากร
        if (disableOnComplete)
        {
            gameObject.SetActive(false);
        }
    }
}