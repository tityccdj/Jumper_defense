using UnityEngine;

public class MinimapIcon : MonoBehaviour
{
    [Header("Icon Settings")]
    public Sprite iconSprite; // เพิ่มช่องให้ลากรูปมาใส่
    public Color iconColor = Color.white;

    private SpriteRenderer sr;

    void Start()
    {
        sr = gameObject.AddComponent<SpriteRenderer>();

        // ใช้รูปที่ลากใส่มา หรือแจ้งเตือนถ้าลืมใส่
        if (iconSprite != null)
        {
            sr.sprite = iconSprite;
        }
        else
        {
            Debug.LogWarning("ลืมใส่รูป Icon Sprite หรือเปล่า?");
        }

        sr.color = iconColor;
        sr.sortingOrder = 100;

        transform.localScale = new Vector3(2f, 2f, 1f);
        transform.localRotation = Quaternion.identity;
    }

    void Update()
    {
        transform.localRotation = Quaternion.identity;
    }
}