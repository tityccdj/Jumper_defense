using UnityEngine;

public class MenuButtonProxy : MonoBehaviour
{
    // ปุ่ม Restart ให้มาเรียกใช้ฟังก์ชันนี้แทน
    public void ClickRestart()
    {
        if (GameSceneManager.Instance != null)
        {
            GameSceneManager.Instance.RestartCurrentScene();
        }
    }

    // ปุ่มกลับหน้าเมนู ให้มาเรียกใช้ฟังก์ชันนี้แทน
    public void ClickMainMenu(string menuName)
    {
        if (GameSceneManager.Instance != null)
        {
            GameSceneManager.Instance.LoadMainMenu(menuName);
        }
    }

    // --- ส่วนที่เพิ่มใหม่: ปุ่มเริ่มเกม/โหลดเข้าด่าน (Play Game) ---
    public void ClickPlayGame(string sceneName)
    {
        if (GameSceneManager.Instance != null)
        {
            GameSceneManager.Instance.LoadGameScene(sceneName);
        }
    }
}