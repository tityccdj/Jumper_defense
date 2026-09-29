using UnityEngine;

public class GameManager : MonoBehaviour
{
    // ทำให้เป็น Singleton เพื่อให้สคริปต์อื่นเรียกใช้ได้ง่ายๆ ผ่าน GameManager.Instance
    public static GameManager Instance { get; private set; }

    [Header("Economy")]
    public int coins = 0;
    public int shards = 0;

    private void Awake()
    {
        // ตรวจสอบว่ามี GameManager ตัวอื่นอยู่ไหม ถ้ามีให้ทำลายทิ้ง (ป้องกันการซ้ำซ้อน)
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCoin(int amount)
    {
        coins += amount;
        Debug.Log("มอนสเตอร์ดรอปเงิน! ตอนนี้มี: " + coins + " เหรียญ");
        // (เดี๋ยวเราจะมาเขียนโค้ดอัปเดตตัวเลขขึ้นหน้าจอ UI ตรงนี้ในอนาคตครับ)
    }

    public bool SpendCoin(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            Debug.Log("ซื้อป้อมสำเร็จ! เหลือเงิน: " + coins + " เหรียญ");
            return true; // จ่ายเงินผ่าน
        }

        Debug.Log("เงินไม่พอ! ขาดอีก: " + (amount - coins) + " เหรียญ");
        return false; // จ่ายไม่ผ่าน (เงินไม่พอ)
    }
}