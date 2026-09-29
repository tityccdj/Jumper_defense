using UnityEngine;

// ชื่อตรงนี้ต้องตรงกับชื่อไฟล์เป๊ะๆ
public class TowerNode : MonoBehaviour
{
    private void OnMouseDown()
    {
        // เมื่อใช้เมาส์คลิกที่จุดนี้ ให้เรียก GameManager มาเปิด Shop
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OpenShop(this);
        }
    }

    public void BuildTower(GameObject towerPrefab)
    {
        // สร้างป้อมและซ่อนจุดวางป้อมทิ้ง
        Instantiate(towerPrefab, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }
}