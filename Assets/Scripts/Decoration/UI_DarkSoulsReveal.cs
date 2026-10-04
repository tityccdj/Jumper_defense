using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class UI_DarkSoulsReveal : MonoBehaviour
{
    [Header("Dramatic Settings")]
    [Tooltip("เวลาหน่วงก่อนที่ตัวหนังสือจะเริ่มปรากฏ (ยิ่งหน่วงยิ่งลุ้น)")]
    public float delayBeforeStart = 0.5f;

    [Tooltip("ระยะเวลาที่ใช้ในการกางออกและชัดขึ้น (Dark Souls จะใช้เวลาประมาณ 2-3 วินาที)")]
    public float revealDuration = 2.5f;

    // --- ส่วนที่เพิ่มเข้ามา ---
    [Header("Post-Reveal Actions")]
    [Tooltip("ลาก Panel ปุ่ม (เช่น ปุ่ม Restart, Main Menu) มาใส่ตรงนี้ มันจะเปิดขึ้นเมื่อตัวหนังสือโชว์เสร็จ")]
    public GameObject panelToEnableAfterReveal;
    // ----------------------

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Coroutine revealCoroutine;

    private Vector3 originalScale;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        originalScale = rectTransform.localScale;
    }

    private void OnEnable()
    {
        canvasGroup.alpha = 0f;
        rectTransform.localScale = new Vector3(originalScale.x, 0f, originalScale.z);

        // --- ซ่อน Panel ปุ่มไว้ก่อนตอนเริ่ม ---
        if (panelToEnableAfterReveal != null)
        {
            panelToEnableAfterReveal.SetActive(false);
        }
        // ---------------------------------

        if (revealCoroutine != null)
        {
            StopCoroutine(revealCoroutine);
        }
        revealCoroutine = StartCoroutine(RevealRoutine());
    }

    private IEnumerator RevealRoutine()
    {
        if (delayBeforeStart > 0)
        {
            yield return new WaitForSecondsRealtime(delayBeforeStart);
        }

        float timer = 0f;

        while (timer < revealDuration)
        {
            timer += Time.unscaledDeltaTime;

            float percent = timer / revealDuration;

            canvasGroup.alpha = Mathf.Lerp(0f, 1f, percent);

            float currentY = Mathf.Lerp(0f, originalScale.y, percent);
            rectTransform.localScale = new Vector3(originalScale.x, currentY, originalScale.z);

            yield return null;
        }

        canvasGroup.alpha = 1f;
        rectTransform.localScale = originalScale;

        // --- เพิ่มคำสั่งเปิด Panel ปุ่มเมื่อโชว์ตัวหนังสือจบ ---
        if (panelToEnableAfterReveal != null)
        {
            panelToEnableAfterReveal.SetActive(true);
        }
        // --------------------------------------------
    }
}