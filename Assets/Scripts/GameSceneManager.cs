using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance { get; private set; }

    [Header("Loading UI")]
    public GameObject loadingScreen;
    public Slider loadingSlider;
    public TextMeshProUGUI progressText;

    [Header("Fake Loading Speeds (ตั้งค่าความเร็วแยกกัน)")]
    [Tooltip("ความเร็วตอนโหลดเข้าด่านเล่นเกม (เช่น 0.5 คือหน่วง 2 วิ)")]
    public float enterGameSpeed = 0.5f;

    [Tooltip("ความเร็วตอนกดรีสตาร์ท เริ่มด่านใหม่ (มักจะให้โหลดไวๆ เช่น 2.0 คือครึ่งวิ)")]
    public float restartSpeed = 2.0f;

    [Tooltip("ความเร็วตอนกลับหน้าเมนูหลัก (เช่น 1.0 คือ 1 วิ)")]
    public float mainMenuSpeed = 1.0f;

    [Tooltip("ความเร็วทั่วไป (ถ้าไม่ได้ระบุประเภท)")]
    public float defaultSpeed = 0.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
    }

    // --- 1. สำหรับปุ่มเข้าเกม (ใช้ enterGameSpeed) ---
    public void LoadGameScene(string sceneName)
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadSceneAsync(sceneName, enterGameSpeed));
    }

    // --- 2. สำหรับปุ่มหน้าเมนู (ใช้ mainMenuSpeed) ---
    public void LoadMainMenu(string menuSceneName)
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadSceneAsync(menuSceneName, mainMenuSpeed));
    }

    // --- 3. สำหรับปุ่ม Restart (ใช้ restartSpeed และดึงฉากปัจจุบันให้อัตโนมัติ) ---
    public void RestartCurrentScene()
    {
        Time.timeScale = 1f;
        string currentScene = SceneManager.GetActiveScene().name;
        StartCoroutine(LoadSceneAsync(currentScene, restartSpeed));
    }

    // --- 4. แบบดั้งเดิม (เผื่อใช้เปลี่ยนฉากอื่นๆ) ---
    public void LoadSceneByName(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName, defaultSpeed));
    }

    public void LoadSceneByIndex(int sceneIndex)
    {
        StartCoroutine(LoadSceneAsync(sceneIndex, defaultSpeed));
    }

    // --- ฟังก์ชันหลักที่เพิ่มการรับค่าความเร็วเข้ามา (speedToUse) ---
    private IEnumerator LoadSceneAsync(object sceneID, float speedToUse)
    {
        if (loadingScreen != null) loadingScreen.SetActive(true);

        if (loadingSlider != null) loadingSlider.value = 0f;
        if (progressText != null) progressText.text = "0%";

        AsyncOperation operation;

        if (sceneID is int index)
        {
            operation = SceneManager.LoadSceneAsync(index);
        }
        else if (sceneID is string name)
        {
            operation = SceneManager.LoadSceneAsync(name);
        }
        else
        {
            yield break;
        }

        operation.allowSceneActivation = false;
        float displayProgress = 0f;

        while (!operation.isDone)
        {
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            // ใช้ความเร็วที่รับค่ามา ตามประเภทของปุ่มที่ผู้เล่นกด
            displayProgress = Mathf.MoveTowards(displayProgress, targetProgress, speedToUse * Time.deltaTime);

            if (loadingSlider != null) loadingSlider.value = displayProgress;
            if (progressText != null) progressText.text = "Loading: "+(displayProgress * 100f).ToString("F0") + "%";

            if (displayProgress >= 1f)
            {
                yield return new WaitForSeconds(0.2f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        if (loadingScreen != null) loadingScreen.SetActive(false);
    }
}