using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    public float shakeDuration = 0.3f;   // ระยะเวลาที่สั่น
    public float shakeMagnitude = 0.2f;  // ความแรงในการสั่น

    private Vector3 originalPos;
    private Coroutine shakeCoroutine;

    // เปิด public ไว้เพื่อให้ Unity Event มองเห็นและเรียกใช้ได้
    public void TriggerShake()
    {
        // ถ้ากำลังสั่นอยู่ ให้หยุดอันเก่าก่อน จะได้สั่นใหม่แบบเต็มแรง
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            transform.localPosition = originalPos; // รีเซ็ตตำแหน่งก่อนเริ่มสั่นใหม่
        }

        shakeCoroutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        float elapsed = 0.0f;

        // เก็บตำแหน่งเดิมของกล้องเอาไว้ก่อนสั่น
        originalPos = transform.localPosition;

        while (elapsed < shakeDuration)
        {
            // สุ่มตำแหน่ง X และ Y ขยับไปมา
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            // อัปเดตตำแหน่งกล้อง
            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null; // รอจนกว่าจะเฟรมถัดไป
        }

        // พอสั่นเสร็จ จับกล้องกลับมาวางไว้ที่เดิมเป๊ะๆ
        transform.localPosition = originalPos;
    }
}