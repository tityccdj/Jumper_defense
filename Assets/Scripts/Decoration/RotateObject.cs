using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("ความเร็วในการหมุน (ใส่ค่าติดลบเพื่อหมุนกลับด้าน)")]
    public float rotationSpeed = 100f;

    void Update()
    {
        // หมุนรอบแกน Z (แกน X และ Y เป็น 0)
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}