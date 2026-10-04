using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class UI_BlinkingText : MonoBehaviour
{
    [Header("Blink Settings")]
    [Tooltip("ความเร็วในการกระพริบ (ค่ายิ่งเยอะยิ่งกระพริบถี่)")]
    public float blinkSpeed = 2f;

    [Tooltip("ความโปร่งใสต่ำสุด (0 = จางหายไปเลย, 1 = ชัดเต็มที่)")]
    [Range(0f, 1f)]
    public float minAlpha = 0.2f;

    [Tooltip("ความโปร่งใสสูงสุด (ปกติคือ 1)")]
    [Range(0f, 1f)]
    public float maxAlpha = 1f;

    [Tooltip("ติ๊กถูก = กระพริบแบบนุ่มนวล (Fade), เอาออก = กระพริบตัดฉับ (Arcade)")]
    public bool smoothFade = true;

    private TextMeshProUGUI tmpText;
    private Coroutine blinkCoroutine;

    private void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        // เริ่มการทำงานใหม่ทุกครั้งที่ข้อความนี้ถูกเปิด (SetActive = true)
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }
        blinkCoroutine = StartCoroutine(BlinkRoutine());
    }

    private IEnumerator BlinkRoutine()
    {
        float timer = 0f;

        while (true) // วนลูปกระพริบไปเรื่อยๆ จนกว่า Object นี้จะถูกปิด
        {
            timer += Time.unscaledDeltaTime * blinkSpeed;
            float currentAlpha;

            if (smoothFade)
            {
                // ใช้ PingPong เพื่อให้ค่าวิ่งไป-กลับ ระหว่าง 0 ถึง 1 อย่างสมูท
                float pingPong = Mathf.PingPong(timer, 1f);
                currentAlpha = Mathf.Lerp(minAlpha, maxAlpha, pingPong);
            }
            else
            {
                // กระพริบแบบตัดฉับ (สลับค่าทุกๆ ครึ่งจังหวะ)
                currentAlpha = (timer % 1f < 0.5f) ? maxAlpha : minAlpha;
            }

            // อัปเดตค่าความโปร่งใสให้ TextMeshPro
            Color currentColor = tmpText.color;
            currentColor.a = currentAlpha;
            tmpText.color = currentColor;

            yield return null;
        }
    }
}