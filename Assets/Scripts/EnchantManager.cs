using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnchantManager : MonoBehaviour
{
    public static EnchantManager Instance { get; private set; }

    [Header("Unlocked Skills")]
    public bool hasSpikeImmune = false;
    public bool hasDoubleJump = false;
    public bool hasDash = false;
    public bool hasTowerOverclock = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 1. ซื้อ Spike Immune (กันหนาม)
    public void BuySpikeImmune(Button btn)
    {
        if (!hasSpikeImmune && GameManager.Instance.SpendShard(1))
        {
            hasSpikeImmune = true;
            btn.interactable = false;
            PlayUpgradeEffect(); // <--- สั่งเล่น Particle
            Debug.Log("ปลดล็อก: Spike Immune!");
        }
    }

    // 2. ซื้อ Double Jump (กระโดดสองชั้น)
    public void BuyDoubleJump(Button btn)
    {
        if (!hasDoubleJump && GameManager.Instance.SpendShard(1))
        {
            hasDoubleJump = true;
            btn.interactable = false;
            PlayUpgradeEffect(); // <--- สั่งเล่น Particle
            Debug.Log("ปลดล็อก: Double Jump!");
        }
    }

    // 3. ซื้อ Dash (พุ่งตัว)
    public void BuyDash(Button btn)
    {
        if (!hasDash && GameManager.Instance.SpendShard(1))
        {
            hasDash = true;
            btn.interactable = false;
            PlayUpgradeEffect(); // <--- สั่งเล่น Particle
            Debug.Log("ปลดล็อก: Dash!");
        }
    }

    // 4. ซื้อ Tower Overclock (บัฟป้อมนานขึ้น)
    public void BuyTowerOverclock(Button btn)
    {
        if (!hasTowerOverclock && GameManager.Instance.SpendShard(1))
        {
            hasTowerOverclock = true;
            btn.interactable = false;
            PlayUpgradeEffect(); // <--- สั่งเล่น Particle
            Debug.Log("ปลดล็อก: Tower Overclock!");
        }
    }

    // 5. ซื้อ Full Heal Base (ฮีลฐาน)
    public void BuyFullHealBase()
    {
        if (GameManager.Instance.SpendShard(1))
        {
            if (UIManager.Instance.baseHealth != null)
            {
                UIManager.Instance.baseHealth.Revive();
                UIManager.Instance.UpdateBaseHealthUI();
                PlayUpgradeEffect(); // <--- สั่งเล่น Particle ให้ผู้เล่นรู้ว่าซื้อสำเร็จแล้ว
                Debug.Log("ฮีลฐานเต็มแล้ว!");
            }
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    // --- ฟังก์ชันเสริมที่เพิ่มเข้ามาใหม่ ---
    private void PlayUpgradeEffect()
    {
        if (ParticleManager.Instance != null)
        {
            // หาตัวผู้เล่นในฉากด้วย Tag แล้วแสดงเอฟเฟกต์ที่ตำแหน่งนั้น
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                ParticleManager.Instance.PlayPlayerUpgrade(player.transform.position);
            }
        }
    }
}