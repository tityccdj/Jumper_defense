using UnityEngine;

public class PlatformMoverByWave : MonoBehaviour
{
    [Header("Movement Settings")]
    public int triggerWave = 3;      // เวฟที่ต้องการให้เริ่มขยับ
    public float targetY = 30f;      // จุดหมายของแกน Y
    public float moveSpeed = 5f;     // ความเร็วในการเลื่อน

    [Header("References")]
    public WaveSpawner waveSpawner;  // ลากกล่องที่มีสคริปต์ WaveSpawner มาใส่ช่องนี้

    void Update()
    {
        if (waveSpawner == null) return;

        // currentWaveIndex ของเราเริ่มจาก 0 (0 = Wave 1) เลยต้องบวก 1 เพื่อให้ตรงกับเลขเวฟจริงๆ
        int currentWaveNumber = waveSpawner.currentWaveIndex + 1;

        // ถ้าเวฟปัจจุบัน มากกว่าหรือเท่ากับเวฟที่กำหนดไว้
        if (currentWaveNumber >= triggerWave)
        {
            // สร้างตำแหน่งเป้าหมาย โดยคงค่า X และ Z เดิมไว้ เปลี่ยนแค่ Y
            Vector3 targetPosition = new Vector3(transform.position.x, targetY, transform.position.z);

            // ค่อยๆ เลื่อนตำแหน่งไปหาเป้าหมายอย่างนุ่มนวล
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }
}