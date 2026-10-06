using UnityEngine;

public class MenuButtonProxy : MonoBehaviour
{
    [Header("Pause UI")]
    [Tooltip("อนุญาตให้ Pause ได้หรือไม่? (เอาติ๊กถูกออกถ้าอยู่ในฉาก Main Menu)")]
    public bool allowPause = true; // <--- เพิ่มสวิตช์ตัวนี้เข้ามา

    [Tooltip("ลากหน้าต่าง Pause Panel (ที่มีปุ่ม Resume, Restart, Main Menu) มาใส่ตรงนี้")]
    public GameObject pausePanel;

    private bool isPaused = false;

    private void Start()
    {
        // ซ่อนหน้าต่าง Pause ไว้ก่อนตอนเริ่มเกม
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    private void Update()
    {
        // เช็คว่าระบบอนุญาตให้ Pause ไหม (allowPause) และเช็คการกดปุ่ม ESC
        if (allowPause && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // --- ฟังก์ชันสำหรับ Pause / Resume เกม ---
    public void TogglePause()
    {
        // ป้องกันกรณีเผลอเอาไปผูกกับปุ่มบนจอในหน้าเมนู
        if (!allowPause) return;

        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; // หยุดเวลาเกม
            if (pausePanel != null) pausePanel.SetActive(true); // เปิดหน้าต่าง Pause
        }
        else
        {
            Time.timeScale = 1f; // ให้เวลาเดินปกติ
            if (pausePanel != null) pausePanel.SetActive(false); // ปิดหน้าต่าง Pause
        }
    }

    // ฟังก์ชันเผื่อแยกปุ่ม Resume ต่างหาก
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    // ----------------------------------------------------

    public void ClickRestart()
    {
        Time.timeScale = 1f; // คืนค่าเวลาก่อนโหลดฉาก ไม่งั้นฉากใหม่จะค้าง
        if (GameSceneManager.Instance != null)
        {
            GameSceneManager.Instance.RestartCurrentScene();
        }
    }

    public void ClickMainMenu(string menuName)
    {
        Time.timeScale = 1f; // คืนค่าเวลาก่อนโหลดฉาก
        if (GameSceneManager.Instance != null)
        {
            GameSceneManager.Instance.LoadMainMenu(menuName);
        }
    }

    public void ClickPlayGame(string sceneName)
    {
        Time.timeScale = 1f; // คืนค่าเวลาก่อนโหลดฉาก
        if (GameSceneManager.Instance != null)
        {
            GameSceneManager.Instance.LoadGameScene(sceneName);
        }
    }
}