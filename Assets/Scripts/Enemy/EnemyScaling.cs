using UnityEngine;

public class EnemyScaling : MonoBehaviour
{
    [Header("Scaling Stats")]
    public int baseHP = 12;           // เลือดพื้นฐานของศัตรูตัวนี้
    public int hpScalePerWave = 4;    // เลือดที่เพิ่มขึ้นในแต่ละ Wave ถัดไป

    // ฟังก์ชันนี้จะถูก WaveSpawner เรียกใช้ตอนที่ปล่อยศัตรูออกมา
    public void ApplyScaling(int waveIndex)
    {
        // สูตรคำนวณ (waveIndex เริ่มที่ 0 ดังนั้น Wave 1 จะบวก 0, Wave 2 จะบวกค่า Scale)
        int scaledHP = baseHP + (waveIndex * hpScalePerWave);

        // ดึงสคริปต์ HealthSystem ในตัวมันเองมาแก้ไขเลือด
        HealthSystem health = GetComponent<HealthSystem>();
        if (health != null)
        {
            health.maxHealth = scaledHP;
            health.currentHealth = scaledHP; // อัปเดตเลือดปัจจุบันให้เต็มตาม Max HP ใหม่
            Debug.Log($"ศัตรูเกิดใหม่! Wave {waveIndex + 1} | เลือด: {scaledHP}");
        }
    }
}