using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("Tower Prefabs")]
    public GameObject normalTowerPrefab;
    public GameObject iceTowerPrefab;

    private TowerNode selectedNode;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        // ปิด Shop ถ้ายกเลิกการเลือก
        if (selectedNode != null)
        {
            if (Input.anyKeyDown && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
            {
                CloseShop();
            }
        }
    }

    // --- ระบบเปิด/ปิด หน้าต่าง UI ---
    public void OpenShop(TowerNode node)
    {
        selectedNode = node;
        UIManager.Instance.OpenShopAtNode(node.transform);
        Time.timeScale = 0f;
    }

    public void OpenUpgradeShop(TowerNode node)
    {
        selectedNode = node;
        UIManager.Instance.OpenUpgradeShopAtNode(node.transform);
        Time.timeScale = 0f;
    }

    public void CloseShop()
    {
        Time.timeScale = 1f;
        selectedNode = null;
        UIManager.Instance.CloseShop(); 
    }

    // --- ระบบซื้อและอัปเกรด (คุยกับ GameManager เพื่อหักเงิน) ---
    public void BuyNormalTower()
    {
        if (GameManager.Instance.SpendCoin(10))
        {
            selectedNode.BuildTower(normalTowerPrefab);
            CloseShop();
        }
    }

    public void BuyIceTower()
    {
        if (GameManager.Instance.SpendCoin(15))
        {
            selectedNode.BuildTower(iceTowerPrefab);
            CloseShop();
        }
    }

    public void UpgradeSelectedTower()
    {
        if (selectedNode != null && selectedNode.currentTower != null)
        {
            // เช็คว่าป้อมนี้ตันหรือยัง (ถ้าเปลี่ยนไปใช้ Scriptable Object แล้ว อาจจะต้องแก้เป็น selectedNode.currentTower.towerData.isMaxLevel นะครับ)
            if (selectedNode.currentTower.isMaxLevel)
            {
                Debug.Log("ป้อมนี้ตันแล้วจ้า!");
                return;
            }

            // ถ้ามี Shard พอ ให้สั่ง Node ทำการสลับร่างป้อม!
            if (GameManager.Instance.SpendShard(1))
            {
                // <--- สั่งเล่นเอฟเฟกต์อัปเกรดป้อมตรงนี้ --->
                if (ParticleManager.Instance != null)
                {
                    // แสดงเอฟเฟกต์ที่ตำแหน่งของฐานป้อม
                    ParticleManager.Instance.PlayTowerUpgrade(selectedNode.transform.position);
                }

                selectedNode.ReplaceWithUpgradedTower();
                CloseShop();
            }
            else
            {
                Debug.Log("Shard ไม่พอ!");
            }
        }
    }
    public void SellSelectedTower()
    {
        if (selectedNode != null && selectedNode.currentTower != null)
        {
            // สั่งให้ Node นั้นทำการขายป้อม
            selectedNode.SellTower();

            // ปิดหน้าต่าง Shop และให้เวลาเดินต่อ
            CloseShop();
        }
    }
}