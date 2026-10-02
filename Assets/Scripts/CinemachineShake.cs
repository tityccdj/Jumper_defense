using System.Collections;
using UnityEngine;
using Unity.Cinemachine; // ใช้ Namespace ของเวอร์ชั่นใหม่

public class CinemachineShake : MonoBehaviour
{
    [Header("Cinemachine Settings")]
    public CinemachineCamera virtualCamera; // เปลี่ยนชื่อเป็น CinemachineCamera
    public float shakeIntensity = 3f;
    public float shakeDuration = 0.3f;

    private CinemachineBasicMultiChannelPerlin noiseProfile;
    private Coroutine shakeCoroutine;

    void Start()
    {
        if (virtualCamera != null)
        {
            // ดึง Component Noise ในระบบใหม่ (ใช้ GetComponent ปกติได้เลย)
            noiseProfile = virtualCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();

            if (noiseProfile != null)
            {
                noiseProfile.AmplitudeGain = 0f; // เปลี่ยนจาก m_AmplitudeGain เป็น AmplitudeGain
            }
        }
    }

    public void TriggerShake()
    {
        if (noiseProfile == null) return;

        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        shakeCoroutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            noiseProfile.AmplitudeGain = Mathf.Lerp(shakeIntensity, 0f, elapsed / shakeDuration);
            yield return null;
        }

        noiseProfile.AmplitudeGain = 0f;
    }
}