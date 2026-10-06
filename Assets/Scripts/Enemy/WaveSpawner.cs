using System.Collections;
using UnityEngine;
using TMPro;

[System.Serializable]
public class EnemyGroup
{
    public GameObject enemyPrefab;
    public int enemyCount;
    public float spawnInterval = 1f;
}

[System.Serializable]
public class Wave
{
    public string waveName;

    [Header("--- ข้อมูลคำนวณอัตโนมัติ (ห้ามแก้) ---")]
    [Tooltip("รวมจำนวนมอนสเตอร์ทั้งหมดที่จะเกิดในเวฟนี้")]
    public int totalEnemiesInWave; // จะโชว์ใน Inspector ไว้ดูยอดรวม

    [Space(10)]
    public EnemyGroup[] enemyGroups;
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

    public int currentWaveIndex = 0;
    private bool isSpawning = false;

    // ----------------------------------------------------
    // ฟังก์ชันนี้จะทำงานอัตโนมัติทุกครั้งที่เราขยับเมาส์หรือพิมพ์ตัวเลขในหน้า Inspector
    private void OnValidate()
    {
        if (waves != null)
        {
            foreach (Wave wave in waves)
            {
                int sum = 0;
                if (wave.enemyGroups != null)
                {
                    foreach (EnemyGroup group in wave.enemyGroups)
                    {
                        sum += group.enemyCount;
                    }
                }
                wave.totalEnemiesInWave = sum; // อัปเดตยอดรวมให้เราดูทันที
            }
        }
    }
    // ----------------------------------------------------

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

        // --- เพิ่มเงื่อนไขกำหนดจุดเกิดตามเวฟ ---
        int availableSpawners = 1;

        if (currentWaveIndex == 0)
        {
            availableSpawners = 1;
        }
        else if (currentWaveIndex >= 1 && currentWaveIndex <= 2)
        {
            availableSpawners = 2;
        }
        else if (currentWaveIndex >= 3)
        {
            availableSpawners = 3;
        }

        availableSpawners = Mathf.Min(availableSpawners, spawnPoints.Length);
        // ------------------------------------

        foreach (EnemyGroup group in currentWave.enemyGroups)
        {
            for (int i = 0; i < group.enemyCount; i++)
            {
                int randomIndex = Random.Range(0, availableSpawners);
                Transform chosenSpawnPoint = spawnPoints[randomIndex];

                Vector3 randomOffset = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), 0);
                Vector3 finalPosition = chosenSpawnPoint.position + randomOffset;

                GameObject newEnemy = Instantiate(group.enemyPrefab, finalPosition, chosenSpawnPoint.rotation);

                EnemyScaling scaling = newEnemy.GetComponent<EnemyScaling>();
                if (scaling != null)
                {
                    scaling.ApplyScaling(currentWaveIndex);
                }

                yield return new WaitForSeconds(group.spawnInterval);
            }
        }

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

        if (GameManager.Instance != null) GameManager.Instance.AddShard(2);
        if (UIManager.Instance != null) UIManager.Instance.ToggleIntermission(true);
    }

    public void StartNextWaveFromUI()
    {
        if (UIManager.Instance != null) UIManager.Instance.ToggleIntermission(false);
        StartCoroutine(StartNextWave());
    }
}