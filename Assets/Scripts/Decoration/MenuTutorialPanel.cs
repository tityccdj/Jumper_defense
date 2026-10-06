using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuTutorialPanel : MonoBehaviour
{
    [Header("Tutorial UI")]
    [Tooltip("ลากตัว Panel หน้าต่าง Tutorial ทั้งก้อนมาใส่ตรงนี้")]
    public GameObject tutorialPanel;

    [Header("Pages Setup")]
    [Tooltip("ลากหน้า Tutorial แต่ละหน้ามาเรียงใส่ในนี้")]
    public GameObject[] pages;

    [Header("Buttons & UI")]
    public Button nextButton;
    public Button prevButton;
    public TextMeshProUGUI pageNumberText;

    private int currentPageIndex = 0;

    private void Start()
    {
        // สั่งปิดหน้า Tutorial ไว้ก่อนตอนเริ่มเกม
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
    }

    // --- ฟังก์ชันสำหรับเปิดหน้า Tutorial (ให้ปุ่มใน Main Menu มาเรียกอันนี้) ---
    public void OpenTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
        }
        currentPageIndex = 0; // บังคับเริ่มที่หน้า 1 เสมอ
        UpdatePageDisplay();
    }

    // --- ฟังก์ชันสำหรับปิดหน้าต่าง (ผูกกับปุ่ม X) ---
    public void CloseTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
    }

    public void ClickNextPage()
    {
        if (currentPageIndex < pages.Length - 1)
        {
            currentPageIndex++;
            UpdatePageDisplay();
        }
    }

    public void ClickPrevPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            UpdatePageDisplay();
        }
    }

    private void UpdatePageDisplay()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] != null)
            {
                pages[i].SetActive(i == currentPageIndex);
            }
        }

        if (prevButton != null)
            prevButton.interactable = (currentPageIndex > 0);

        if (nextButton != null)
            nextButton.interactable = (currentPageIndex < pages.Length - 1);

        if (pageNumberText != null)
        {
            pageNumberText.text = (currentPageIndex + 1) + " / " + pages.Length;
        }
    }
}