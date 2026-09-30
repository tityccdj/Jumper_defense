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
    public GameObject waveClearText;

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

            // --- เพิ่มบรรทัดนี้เข้าไป เพื่อเรียกคำสั่งชนะ ---
            if (GameManager.Instance != null) GameManager.Instance.GameWin();

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

        // --- ส่วนที่เพิ่มเข้ามาใหม่: รอจนกว่าศัตรูจะตายหมด ---
        Debug.Log("ปล่อยศัตรูครบแล้ว! รอผู้เล่นเคลียร์มอนสเตอร์...");

        // เช็คว่ายังมีวัตถุที่ติด Tag "Enemy" อยู่ในฉากหรือไม่ 
        // (หน่วงเวลาเช็คทุกๆ 0.5 วิ เกมจะได้ไม่กระตุก)
        while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
        {
            yield return new WaitForSeconds(0.5f);
        }
        // ------------------------------------------

        // --- ส่วนที่เพิ่มเข้ามา: รอ 3 วินาที ค่อยเด้งหน้าจอ ---
        if (waveClearText != null) waveClearText.SetActive(true);
        Debug.Log("ศัตรูตายหมดแล้ว! โชว์ข้อความ Wave Clear 3 วินาที...");

        yield return new WaitForSeconds(3f); // รอ 3 วินาที

        // ปิดข้อความ Wave Clear ก่อนเด้งหน้าต่าง Shard
        if (waveClearText != null) waveClearText.SetActive(false);
        // ------------------------------------------------

        currentWaveIndex++; // เตรียมเข้าสู่เวฟถัดไป

        // แจก Shard และเปิดหน้าจอ
        Debug.Log("เคลียร์เวฟสำเร็จ! แจก Shard และรอผู้เล่นกดยืนยัน...");

        // ให้ Shard เป็นรางวัลจบเวฟ (สามารถเขียนสูตรแจกตาม waveIndex ได้ในอนาคต)
        if (GameManager.Instance != null) GameManager.Instance.AddShard(1);

        // เปิดหน้าจอพักเบรค
        if (UIManager.Instance != null) UIManager.Instance.ToggleIntermission(true);
    }
    public void StartNextWaveFromUI()
    {
        if (UIManager.Instance != null) UIManager.Instance.ToggleIntermission(false); // ปิดหน้าจอพักเบรค
        StartCoroutine(StartNextWave()); // สั่งรันเวฟต่อไป
    }
}