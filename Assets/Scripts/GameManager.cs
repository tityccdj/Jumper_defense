using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Economy")]
    public int coins = 0;
    public int shards = 0;

    [Header("Tower System")]
    public GameObject normalTowerPrefab;
    private TowerNode selectedNode;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UIManager.Instance.UpdateCoinUI(coins); // สั่ง UI ให้อัปเดตเงิน
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
}