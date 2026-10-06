using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Economy")]
    public int coins = 0;
    public int shards = 0;

    [Header("Player Respawn")]
    public Transform respawnPoint;      // จุดเกิด (เอา Base มาใส่)
    public float respawnCooldown = 10f; // เวลาเกิดใหม่ (วินาที)
    public TextMeshProUGUI respawnText; // UI นับถอยหลังบนจอ

    // --- เพิ่มตัวแปรเช็คสถานะเกมจบ ---
    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UIManager.Instance.UpdateCoinUI(coins);
        UIManager.Instance.UpdateShardUI(shards);
    }

    public void AddCoin(int amount)
    {
        coins += amount;
        UIManager.Instance.UpdateCoinUI(coins);
    }

    public void AddShard(int amount)
    {
        shards += amount;
        UIManager.Instance.UpdateShardUI(shards);
        Debug.Log("ได้รับ Shard! ตอนนี้มี: " + shards + " ชิ้น");
    }

    public bool SpendCoin(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            UIManager.Instance.UpdateCoinUI(coins);
            return true;
        }
        return false;
    }

    public bool SpendShard(int amount)
    {
        if (shards >= amount)
        {
            shards -= amount;
            UIManager.Instance.UpdateShardUI(shards);
            return true;
        }
        return false;
    }

    public void GameOver()
    {
        isGameOver = true; // <--- แจ้งเตือนระบบว่าเกมจบแล้ว

        // --- ปิดข้อความเกิดใหม่ทันที ---
        if (respawnText != null)
        {
            respawnText.gameObject.SetActive(false);
        }

        Debug.Log("Game Over! ฐานถูกทำลาย");
        UIManager.Instance.ShowGameOver(); // สั่ง UI โชว์หน้าแพ้
        Time.timeScale = 0f;               // หยุดเวลาในเกมทั้งหมด (มอนสเตอร์หยุดเดิน)
    }

    public void GameWin()
    {
        isGameOver = true; // กันไว้เผื่อชนะตอนกำลังตายพอดี
        if (respawnText != null) respawnText.gameObject.SetActive(false);

        Debug.Log("You Win! กันได้ทุกเวฟ");
        UIManager.Instance.ShowWin();      // สั่ง UI โชว์หน้าชนะ
        Time.timeScale = 0f;               // หยุดเวลาเช่นกัน
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void HandlePlayerDeath(GameObject player)
    {
        // เช็คว่าถ้าเกมยังไม่จบ ถึงจะยอมให้นับถอยหลังเกิดใหม่
        if (!isGameOver)
        {
            StartCoroutine(RespawnRoutine(player));
        }
    }

    private IEnumerator RespawnRoutine(GameObject player)
    {
        Debug.Log("Player ตาย! เริ่มนับถอยหลังเกิดใหม่...");

        // 1. ซ่อนตัวละคร
        player.SetActive(false);

        // 2. แสดง UI นับถอยหลัง (ถ้ามี)
        float timer = respawnCooldown;
        if (respawnText != null) respawnText.gameObject.SetActive(true);

        while (timer > 0)
        {
            // --- เพิ่มเงื่อนไข: ถ้าเกมโอเวอร์ระหว่างที่กำลังตาย ให้หยุด Coroutine นี้ทิ้งไปเลย ---
            if (isGameOver)
            {
                if (respawnText != null) respawnText.gameObject.SetActive(false);
                yield break; // ดีดตัวออกจาก Coroutine ทันที
            }
            // -------------------------------------------------------------

            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
            {
                Debug.Log("ศัตรูตายหมดฉากแล้ว! เกิดใหม่ทันที!");
                break;
            }

            if (respawnText != null) respawnText.text = "Respawn in: " + timer + "s";
            yield return new WaitForSeconds(1f); // รอทีละ 1 วินาที
            timer--;
        }

        // เช็คอีกรอบก่อนโชว์ตัวละคร เผื่อฐานแตกตอนกำลังจะเกิดพอดี
        if (isGameOver) yield break;

        if (respawnText != null) respawnText.gameObject.SetActive(false);

        // 3. ย้ายตำแหน่งกลับไปที่จุดเกิด
        if (respawnPoint != null)
        {
            player.transform.position = respawnPoint.position;
        }

        // 4. รีเซ็ตเลือดให้เต็ม
        HealthSystem playerHealth = player.GetComponent<HealthSystem>();
        if (playerHealth != null)
        {
            playerHealth.Revive();
        }

        // 5. โชว์ตัวละครกลับมา
        player.SetActive(true);
        Debug.Log("Player เกิดใหม่แล้ว!");
    }
}