using System.Collections;
using UnityEngine;
using TMPro; // เผื่อเอาไว้โชว์เลข Wave บนหน้าจอ

// คลาสนี้เอาไว้เก็บข้อมูลของแต่ละ Wave (ใส่ Serializable เพื่อให้มันไปโผล่ใน Inspector)
[System.Serializable]
public class Wave
{
    public string waveName;           // ชื่อเวฟ (เช่น Wave 1)
    public GameObject enemyPrefab;    // ตัวมอนสเตอร์ที่จะปล่อย
    public int enemyCount;            // จำนวนมอนสเตอร์ในเวฟนี้
    public float spawnInterval = 1f;  // ปล่อยห่างกันตัวละกี่วินาที (เช่น 1.5 วินาที)
}

public class WaveSpawner : MonoBehaviour
{
    [Header("Wave Settings")]
    public Wave[] waves;              // รายการ Wave ทั้งหมดในเกม
    public Transform[] spawnPoints;      // จุดที่จะให้มอนสเตอร์เกิด
    public float timeBetweenWaves = 5f; // เวลาพักเบรคระหว่างแต่ละเวฟ

    [Header("UI (Optional)")]
    public TextMeshProUGUI waveText;  // เอาไว้แสดงผลบนจอว่าถึงเวฟไหนแล้ว

    private int currentWaveIndex = 0;
    private bool isSpawning = false;

    void Start()
    {
        StartCoroutine(StartNextWave());
    }

    private IEnumerator StartNextWave()
    {
        // ถ้าเล่นครบทุกเวฟแล้ว ก็จบเกม (ชนะ)
        if (currentWaveIndex >= waves.Length)
        {
            Debug.Log("ยินดีด้วย! คุณเคลียร์ครบทุกเวฟแล้ว!");
            if (waveText != null) waveText.text = "You Win!";
            yield break;
        }

        isSpawning = true;
        Wave currentWave = waves[currentWaveIndex]; // ดึงข้อมูลเวฟปัจจุบันมา

        // อัปเดต UI บนหน้าจอ
        if (waveText != null) waveText.text = "Wave: " + (currentWaveIndex + 1);
        Debug.Log("กำลังเริ่มเวฟ: " + currentWave.waveName);

        // วนลูปปล่อยมอนสเตอร์ทีละตัว จนครบจำนวน
        for (int i = 0; i < currentWave.enemyCount; i++)
        {
            // --- ส่วนที่เพิ่มเข้ามาใหม่ ---
            // สุ่มเลือกจุดเกิด 1 จุด จากรายชื่อจุดเกิดทั้งหมดที่เรามี
            int randomIndex = Random.Range(0, spawnPoints.Length);
            Transform chosenSpawnPoint = spawnPoints[randomIndex];

            // สร้างมอนสเตอร์ที่จุดที่สุ่มได้
            Instantiate(currentWave.enemyPrefab, chosenSpawnPoint.position, chosenSpawnPoint.rotation);
            // ------------------------

            yield return new WaitForSeconds(currentWave.spawnInterval);
        }

        isSpawning = false;
        currentWaveIndex++; // เตรียมเข้าสู่เวฟถัดไป

        // รอเวลาพักเบรคก่อนเริ่มเวฟต่อไป
        Debug.Log("จบเวฟ! พักเบรค " + timeBetweenWaves + " วินาที");
        yield return new WaitForSeconds(timeBetweenWaves);

        // วนลูปกลับไปเริ่มเวฟใหม่
        StartCoroutine(StartNextWave());
    }
}