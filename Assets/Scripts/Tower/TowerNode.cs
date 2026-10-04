using UnityEngine;

public class TowerNode : MonoBehaviour
{
    public Tower currentTower;

    private void OnMouseDown()
    {
        if (ShopManager.Instance != null)
        {
            if (currentTower == null)
            {
                ShopManager.Instance.OpenShop(this);
            }
            else if (!currentTower.isMaxLevel) // เปลี่ยนตรงนี้
            {
                ShopManager.Instance.OpenUpgradeShop(this);
            }
        }
    }

    public void BuildTower(GameObject towerPrefab)
    {
        GameObject towerObj = Instantiate(towerPrefab, transform.position, Quaternion.identity);
        currentTower = towerObj.GetComponent<Tower>();
        currentTower.myNode = this;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;
    }

    // --- เพิ่มฟังก์ชันนี้สำหรับทำลายของเก่า แล้ววางของใหม่ ---
    public void ReplaceWithUpgradedTower()
    {
        if (currentTower != null && currentTower.upgradedTowerPrefab != null)
        {
            // 1. จำ Prefab ร่าง 2 เอาไว้ก่อน
            GameObject nextPrefab = currentTower.upgradedTowerPrefab;

            // 2. ระเบิดป้อมร่าง 1 ทิ้ง!
            Destroy(currentTower.gameObject);

            // 3. วางป้อมร่าง 2 ลงไปแทนที่
            GameObject newTowerObj = Instantiate(nextPrefab, transform.position, Quaternion.identity);
            currentTower = newTowerObj.GetComponent<Tower>();
            currentTower.myNode = this; // บอกป้อมใหม่ว่ายืนอยู่บนฐานนี้นะ
        }
    }
    public void SellTower()
    {
        if (currentTower != null)
        {
            // 1. คืนเงินให้ผู้เล่น
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddCoin(currentTower.sellValue);
            }

            // ----------------------------------------------------
            // เอาบรรทัดนี้ออกครับ เอฟเฟกต์อัปเกรดจะได้ไม่โผล่ตอนขาย
            /* 
            if (ParticleManager.Instance != null)
            {
                ParticleManager.Instance.PlayTowerUpgrade(transform.position); 
            }
            */
            // (ถ้าคุณทำเอฟเฟกต์ควันตอนขายป้อมไว้ต่างหาก ค่อยเอามาใส่ตรงนี้แทนครับ)
            // ----------------------------------------------------

            // 2. ทำลายป้อมทิ้ง
            Destroy(currentTower.gameObject);
            currentTower = null;

            // 3. เปิดการแสดงผล Sprite ของฐานให้กลับมามองเห็นอีกครั้ง
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = true;
        }
    }
}