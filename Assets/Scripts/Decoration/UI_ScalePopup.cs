using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UI_ScalePopup : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("ขนาดใหญ่สุดที่ต้องการให้ขยายไปถึง (ปกติคือ 1,1,1)")]
    public Vector3 targetScale = Vector3.one;

    [Tooltip("ระยะเวลาที่ใช้ในการขยายตัว (วินาที)")]
    public float duration = 0.5f;

    [Tooltip("ปรับจังหวะการเด้ง (แนะนำให้ตั้งจุดเริ่มที่ 0 และจบที่ 1)")]
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private RectTransform rectTransform;
    private Coroutine scaleCoroutine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // ฟังก์ชันนี้จะทำงานอัตโนมัติทุกครั้งที่ Object ถูกเปิด (SetActive = true)
    private void OnEnable()
    {
        // รีเซ็ตขนาดให้เป็น 0 ก่อนเริ่มอนิเมชัน
        rectTransform.localScale = Vector3.zero;

        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }
        scaleCoroutine = StartCoroutine(ScaleUpRoutine());
    }

    private IEnumerator ScaleUpRoutine()
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime; // ใช้ unscaledDeltaTime เผื่อตอนนั้นคุณหยุดเกม (Time.timeScale = 0) อยู่

            // คำนวณเปอร์เซ็นต์เวลา (0 ถึง 1)
            float percent = timer / duration;

            // ดึงค่าการเด้งจากเส้น Curve
            float curveValue = scaleCurve.Evaluate(percent);

            // ขยายขนาดตามเส้น Curve (ใช้ LerpUnclamped เพื่อให้ขยายทะลุเป้าหมายแล้วหดกลับได้)
            rectTransform.localScale = Vector3.LerpUnclamped(Vector3.zero, targetScale, curveValue);

            yield return null;
        }

        // จบอนิเมชัน บังคับขนาดให้เป๊ะตาม targetScale
        rectTransform.localScale = targetScale;
    }
}