using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Economy")]
    public int coins = 0;
    public int shards = 0;

    [Header("Tower System")]
    public GameObject normalTowerPrefab;
    private TowerNode selectedNode;

    [Header("Player Respawn")]
    public Transform respawnPoint;      // จุดเกิด (เอา Base มาใส่)
    public float respawnCooldown = 10f; // เวลาเกิดใหม่ (วินาที)
    public TextMeshProUGUI respawnText; // UI นับถอยหลังบนจอ

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UIManager.Instance.UpdateCoinUI(coins); // สั่ง UI ให้อัปเดตเงิน
        UIManager.Instance.UpdateShardUI(shards);
    }
    private void Update()
    {
        // เช็คว่าหน้าต่าง Shop เปิดอยู่ไหม (ถ้าเปิดอยู่ selectedNode จะไม่เป็น null)
        if (selectedNode != null)
        {
            // เช็คว่ามีการกดปุ่มใดๆ (Input.anyKeyDown) 
            // และ ป้องกันไม่ให้นับรวมการคลิกเมาส์ ซ้าย(0), ขวา(1), กลาง(2)
            if (Input.anyKeyDown && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
            {
                CloseShop();
            }
        }
    }

    public void AddCoin(int amount)
    {
        coins += amount;
        UIManager.Instance.UpdateCoinUI(coins); // สั่ง UI ให้อัปเดตเงิน
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
            UIManager.Instance.UpdateCoinUI(coins); // สั่ง UI ให้อัปเดตเงิน
            return true;
        }
        return false;
    }

    public void OpenShop(TowerNode node)
    {
        selectedNode = node;
        // ส่งตำแหน่งของ node ไปให้ UIManager ย้ายหน้าต่าง Shop
        UIManager.Instance.OpenShopAtNode(node.transform);
        Time.timeScale = 0f;
    }

    public void CloseShop()
    {
        selectedNode = null;
        // สั่งปิดแบบใหม่
        UIManager.Instance.CloseShop();
        Time.timeScale = 1f;
    }

    public void BuyNormalTower()
    {
        if (SpendCoin(10))
        {
            selectedNode.BuildTower(normalTowerPrefab);
            CloseShop();
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over! ฐานถูกทำลาย");
        UIManager.Instance.ShowGameOver(); // สั่ง UI โชว์หน้าแพ้
        Time.timeScale = 0f;               // หยุดเวลาในเกมทั้งหมด (มอนสเตอร์หยุดเดิน)
    }

    public void GameWin()
    {
        Debug.Log("You Win! กันได้ทุกเวฟ");
        UIManager.Instance.ShowWin();      // สั่ง UI โชว์หน้าชนะ
        Time.timeScale = 0f;               // หยุดเวลาเช่นกัน
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // สำคัญมาก! ต้องคืนค่าเวลาก่อนโหลดฉากใหม่ ไม่งั้นเกมจะค้างถาวร
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // โหลดฉากปัจจุบันซ้ำ
    }
    public void HandlePlayerDeath(GameObject player)
    {
        StartCoroutine(RespawnRoutine(player));
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
            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
            {
                Debug.Log("ศัตรูตายหมดฉากแล้ว! เกิดใหม่ทันที!");
                break; // คำสั่ง break จะเตะเราออกจากลูป while ทันที โดยไม่ต้องรอให้ timer ถึง 0
            }
            // ------------------------

            if (respawnText != null) respawnText.text = "Respawn in: " + timer + "s";
            yield return new WaitForSeconds(1f); // รอทีละ 1 วินาที
            timer--;
        }

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