using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Economy UI")]
    public TextMeshProUGUI coinText;

    [Header("Health UI")]
    public TextMeshProUGUI playerHealthText;
    public HealthSystem playerHealth; // โยงไปหาเลือดผู้เล่น

    public TextMeshProUGUI baseHealthText;
    public HealthSystem baseHealth;   // โยงไปหาเลือดฐาน

    [Header("Shop UI")]
    public GameObject shopUI;

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

    public void CloseShop()
    {
        if (shopUI != null) shopUI.SetActive(false);
    }
}