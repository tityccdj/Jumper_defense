using UnityEngine;

public class ShowOnWave : MonoBehaviour
{
    [Header("References")]
    [Tooltip("ลาก Object ที่ต้องการให้โผล่ (เช่น Minimap) มาใส่ช่องนี้")]
    public GameObject objectToShow;

    [Tooltip("ลาก Object ที่มีสคริปต์ WaveSpawner มาใส่ช่องนี้ (ถ้าว่าง โค้ดจะหาให้อัตโนมัติ)")]
    public WaveSpawner waveSpawner;

    [Header("Wave Settings")]
    [Tooltip("ระบุ Wave ที่ต้องการให้ Object นี้โผล่ขึ้นมา")]
    public int showAtWave = 2;

    void Start()
    {
        // 1. ถ้าลืมลาก WaveSpawner มาใส่ ให้โค้ดพยายามค้นหาให้เองอัตโนมัติ
        if (waveSpawner == null)
        {
            waveSpawner = FindFirstObjectByType<WaveSpawner>();
        }

        // 2. ปิด Object เป้าหมาย (Minimap) ไว้ก่อนตอนเริ่มเกม
        if (objectToShow != null)
        {
            objectToShow.SetActive(false);
        }
    }

    void Update()
    {
        // ป้องกัน Error ถ้าหาใครไม่เจอ
        if (waveSpawner == null || objectToShow == null) return;

        // ดึงค่า Wave ปัจจุบัน (+1 เพราะ Index เริ่มจาก 0)
        int currentWave = waveSpawner.currentWaveIndex + 1;

        // ถ้า Wave ถึงกำหนด และเป้าหมายยังถูกปิดอยู่
        if (currentWave >= showAtWave && !objectToShow.activeSelf)
        {
            objectToShow.SetActive(true); // สั่งเปิด Minimap!
            Debug.Log(objectToShow.name + " ปลดล็อกการแสดงผลแล้วใน Wave " + currentWave);

            // ปิดสคริปต์นี้ทิ้งได้เลยเพื่อประหยัดทรัพยากร เพราะงานสำเร็จแล้ว
            this.enabled = false;
        }
    }
}