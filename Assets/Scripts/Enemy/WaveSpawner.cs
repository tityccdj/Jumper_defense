using System.Collections;
using UnityEngine;
using TMPro;

// 1. สร้าง Class ใหม่สำหรับจัดกลุ่มศัตรู
[System.Serializable]
public class EnemyGroup
{
    public GameObject enemyPrefab;    // มอนสเตอร์ชนิดที่ต้องการ
    public int enemyCount;            // จำนวนที่จะปล่อย
    public float spawnInterval = 1f;  // ปล่อยห่างกันตัวละกี่วินาที
}

// 2. แก้ไข Class Wave ให้เก็บข้อมูลเป็น Array ของ EnemyGroup แทน
[System.Serializable]
public class Wave
{
    public string waveName;
    public EnemyGroup[] enemyGroups;  // ใส่ศัตรูได้หลายชนิดใน 1 เวฟ!
}

public class WaveSpawner : MonoBehaviour
{
    [Header("Wave Settings")]
    public Wave[] waves;
    public Transform[] spawnPoints;
    public float timeBetweenWaves = 5f;

    [Header("UI (Optional)")]
    public TextMeshProUGUI waveText;
    public GameObject waveClearText;

    private int currentWaveIndex = 0;
    private bool isSpawning = false;

    void Start()
    {
        StartCoroutine(StartNextWave());
    }

    private IEnumerator StartNextWave()
    {
        if (currentWaveIndex >= waves.Length)
        {
            Debug.Log("ยินดีด้วย! คุณเคลียร์ครบทุกเวฟแล้ว!");
            if (waveText != null) waveText.text = "You Win!";
            if (GameManager.Instance != null) GameManager.Instance.GameWin();
            yield break;
        }

        isSpawning = true;
        Wave currentWave = waves[currentWaveIndex];

        if (waveText != null) waveText.text = "Wave: " + (currentWaveIndex + 1);
        Debug.Log("กำลังเริ่มเวฟ: " + currentWave.waveName);

        // --- ส่วนที่เปลี่ยนไป: วนลูป 2 ชั้น ---
        // ชั้นที่ 1: วนลูปตามกลุ่มศัตรู (เช่น BaseEnemy, ตามด้วย RangerEnemy)
        foreach (EnemyGroup group in currentWave.enemyGroups)
        {
            // ชั้นที่ 2: วนลูปปล่อยศัตรูตามจำนวนในกลุ่มนั้น
            for (int i = 0; i < group.enemyCount; i++)
            {
                int randomIndex = Random.Range(0, spawnPoints.Length);
                Transform chosenSpawnPoint = spawnPoints[randomIndex];

                // แอบใส่การสุ่มจุดเกิดนิดหน่อย กันศัตรูสิงร่างกัน
                Vector3 randomOffset = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), 0);
                Vector3 finalPosition = chosenSpawnPoint.position + randomOffset;

                GameObject newEnemy = Instantiate(group.enemyPrefab, finalPosition, chosenSpawnPoint.rotation);

                EnemyScaling scaling = newEnemy.GetComponent<EnemyScaling>();
                if (scaling != null)
                {
                    scaling.ApplyScaling(currentWaveIndex);
                }

                // รอเวลาก่อนปล่อยตัวถัดไปในกลุ่ม
                yield return new WaitForSeconds(group.spawnInterval);
            }
        }
        // ------------------------------------

        isSpawning = false;
        Debug.Log("ปล่อยศัตรูครบแล้ว! รอผู้เล่นเคลียร์มอนสเตอร์...");

        while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
        {
            yield return new WaitForSeconds(0.5f);
        }

        if (waveClearText != null) waveClearText.SetActive(true);
        Debug.Log("ศัตรูตายหมดแล้ว! โชว์ข้อความ Wave Clear 3 วินาที...");

        yield return new WaitForSeconds(3f);

        if (waveClearText != null) waveClearText.SetActive(false);

        currentWaveIndex++;
        Debug.Log("เคลียร์เวฟสำเร็จ! แจก Shard และรอผู้เล่นกดยืนยัน...");

        if (GameManager.Instance != null) GameManager.Instance.AddShard(1);
        if (UIManager.Instance != null) UIManager.Instance.ToggleIntermission(true);
    }

    public void StartNextWaveFromUI()
    {
        if (UIManager.Instance != null) UIManager.Instance.ToggleIntermission(false);
        StartCoroutine(StartNextWave());
    }
}