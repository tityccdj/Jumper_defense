using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Economy UI")]
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI shardText;

    [Header("Health UI")]
    public TextMeshProUGUI playerHealthText;
    public HealthSystem playerHealth; // โยงไปหาเลือดผู้เล่น

    public TextMeshProUGUI baseHealthText;
    public HealthSystem baseHealth;   // โยงไปหาเลือดฐาน
    public Slider baseHealthSlider;

    [Header("Shop UI")]
    public GameObject shopUI;
    public GameObject upgradeShopUI; // <--- เพิ่มบรรทัดนี้

    [Header("Game State UI")]
    public GameObject gameOverPanel; // หน้าจอแพ้
    public GameObject winPanel;      // หน้าจอชนะ
    public GameObject intermissionPanel;

    public void ShowGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void ShowWin()
    {
        if (winPanel != null) winPanel.SetActive(true);
    }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // อัปเดตเลือดครั้งแรกตอนเริ่มเกม
        UpdatePlayerHealthUI();
        UpdateBaseHealthUI();
        if (baseHealthSlider != null && baseHealth != null)
        {
            // สมมติว่าใน HealthSystem มีตัวแปรชื่อ maxHealth 
            // baseHealthSlider.maxValue = baseHealth.maxHealth; 

            // ปรับค่าเริ่มต้นของหลอดให้เต็ม
            baseHealthSlider.maxValue = baseHealth.GetCurrentHealth();
            baseHealthSlider.value = baseHealth.GetCurrentHealth();
        }
    }

    public void UpdateCoinUI(int coins)
    {
        if (coinText != null) coinText.text = "Coins: " + coins;
    }

    // ฟังก์ชันนี้เราจะเอาไปผูกกับ Unity Event ตอนโดนโจมตี
    public void UpdatePlayerHealthUI()
    {
        if (playerHealthText != null && playerHealth != null)
        {
            playerHealthText.text = "Player HP: " + playerHealth.GetCurrentHealth();
        }
    }

    public void UpdateBaseHealthUI()
    {
        if (baseHealthText != null && baseHealth != null)
        {
            baseHealthText.text = "Base HP: " + baseHealth.GetCurrentHealth();
        }
        if (baseHealthSlider != null && baseHealth != null)
        {
            // สมมติว่าใน HealthSystem มีตัวแปรชื่อ maxHealth 
            // baseHealthSlider.maxValue = baseHealth.maxHealth; 

            // ปรับค่าเริ่มต้นของหลอดให้เต็ม
            baseHealthSlider.value = baseHealth.GetCurrentHealth();
        }
    }

    public void OpenShopAtNode(Transform nodeTransform)
    {
        if (shopUI != null)
        {
            shopUI.SetActive(true);

            // แปลงพิกัดจากในเกม (World Space) มาเป็นพิกัดหน้าจอ (Screen Space)
            Vector3 screenPos = Camera.main.WorldToScreenPoint(nodeTransform.position);

            // ขยับ UI ขึ้นไปข้างบนนิดหน่อย (บวกแกน Y) จะได้ไม่บังจุดที่เราคลิก
            screenPos.y += 80f; // ปรับตัวเลขนี้ได้ตามต้องการ (ยิ่งเยอะยิ่งลอยสูง)

            shopUI.transform.position = screenPos;
        }
    }
    public void OpenUpgradeShopAtNode(Transform nodeTransform)
    {
        if (upgradeShopUI != null)
        {
            upgradeShopUI.SetActive(true);

            // แปลงพิกัดจากในเกมมาเป็นพิกัดหน้าจอ
            Vector3 screenPos = Camera.main.WorldToScreenPoint(nodeTransform.position);
            screenPos.y += 80f;

            upgradeShopUI.transform.position = screenPos;
        }
    }
    public void CloseShop()
    {
        if (shopUI != null) shopUI.SetActive(false);
        if (upgradeShopUI != null) upgradeShopUI.SetActive(false); // <--- เพิ่มบรรทัดนี้
    }
    public void UpdateShardUI(int shards)
    {
        if (shardText != null) shardText.text = "Shards: " + shards;
    }

    public void ToggleIntermission(bool isOpen)
    {
        if (intermissionPanel != null) intermissionPanel.SetActive(isOpen);
    }
}