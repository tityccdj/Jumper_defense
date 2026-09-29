using System.Collections;
using UnityEngine;

public class DamageFeedback : MonoBehaviour
{
    [Header("Flash Settings")]
    public Color flashColor = Color.white; // แนะนำให้ใช้สีขาวตอนกระพริบ จะเห็นชัดที่สุดครับ
    public float flashDuration = 0.1f;

    [Header("Particle Settings")]
    public GameObject damageParticlePrefab;

    private SpriteRenderer[] allSprites;
    private Color[] originalColors; // สร้างกล่องเก็บสีดั้งเดิมของแต่ละชิ้นส่วน

    void Start()
    {
        // หา Sprite ทุกตัว
        allSprites = GetComponentsInChildren<SpriteRenderer>();

        // เตรียมกล่องเก็บสีให้จำนวนเท่ากับ Sprite ที่หาเจอ
        originalColors = new Color[allSprites.Length];

        // วนลูปเพื่อจดจำสีเดิมของทุกชิ้นส่วนเก็บไว้
        for (int i = 0; i < allSprites.Length; i++)
        {
            originalColors[i] = allSprites[i].color;
        }
    }

    public void PlayFeedback()
    {
        if (damageParticlePrefab != null)
        {
            Instantiate(damageParticlePrefab, transform.position, Quaternion.identity);
        }

        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // 1. เปลี่ยนสี Sprite ทุกตัวเป็นสีกระพริบ (เช่น สีขาวสว่าง)
        foreach (SpriteRenderer sr in allSprites)
        {
            if (sr != null) sr.color = flashColor;
        }

        yield return new WaitForSeconds(flashDuration);

        // 2. คืนสีกลับเป็นสีเดิมของใครของมัน ที่เราจดจำไว้ตอนแรก
        for (int i = 0; i < allSprites.Length; i++)
        {
            if (allSprites[i] != null)
            {
                allSprites[i].color = originalColors[i];
            }
        }
    }
}