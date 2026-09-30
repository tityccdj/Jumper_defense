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
}