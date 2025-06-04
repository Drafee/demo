using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject lightUIPanel;
    [SerializeField] private GameObject darkUIPanel;
    [SerializeField] private GameObject otherUIPanel;

    [SerializeField] private GameObject pressFReminderPanel;
    [SerializeField] private GameObject answerAreaPanel;

    private bool isDarkUIModeEnabled = false;
    private bool isDarkUIVisible = false;

    private void Awake()
    {
        // Singleton ��ʼ��
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        HideAllUI();
    }

    private void Update()
    {
        if (isDarkUIModeEnabled && Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleDarkUI();
        }
    }

    // ---- UI ��ʾ���� ----
    public void ShowLightUI()
    {
        HideAllUI();
        if (lightUIPanel != null)
            lightUIPanel.SetActive(true);
    }

    public void ShowDarkUI()
    {
        if (darkUIPanel != null)
            darkUIPanel.SetActive(true);
        isDarkUIVisible = true;
        FreezePlayer();
    }

    public void ShowPressFReminder() {
        HideAllUI();
        if (pressFReminderPanel != null)
            pressFReminderPanel.SetActive(true);
    }

    public void ShowAnswerAreaPanel()
    {
        HideAllUI();
        if (answerAreaPanel != null)
            answerAreaPanel.SetActive(true);
        FreezePlayer();
    }

    public void ShowOtherUI()
    {
        HideAllUI();
        if (otherUIPanel != null)
            otherUIPanel.SetActive(true);
    }

    // ---- UI ���ط��� ----
    public void HideLightUI()
    {
        if (lightUIPanel != null)
            lightUIPanel.SetActive(false);
    }
    public void HidePressFReminder()
    {
        if (pressFReminderPanel != null)
            pressFReminderPanel.SetActive(false);
    }

    public void HideAnswerAreaPanel()
    {
        HideAllUI();
        if (answerAreaPanel != null)
            answerAreaPanel.SetActive(false);

        ReleasePlayer();
    }

    public void HideDarkUI()
    {
        if (darkUIPanel != null)
            darkUIPanel.SetActive(false);
        isDarkUIVisible = false;
        ReleasePlayer();
    }

    public void HideOtherUI()
    {
        if (otherUIPanel != null)
            otherUIPanel.SetActive(false);
    }

    public void HideAllUI()
    {
        HideLightUI();
        HideDarkUI();
        HideOtherUI();
    }

    // ---- Dark ģʽ���� ----
    public void EnableDarkUIMode()
    {
        isDarkUIModeEnabled = true;
    }

    public void DisableDarkUIMode()
    {
        isDarkUIModeEnabled = false;
        HideDarkUI(); // �˳�ģʽʱ�Զ��ر� UI
    }

    // ---- �л� Dark UI ��� ----
    private void ToggleDarkUI()
    {
        if (isDarkUIVisible)
            HideDarkUI();
        else
            ShowDarkUI();
    }

    private void FreezePlayer() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // �޸����״̬�����������ҿ���
        PlayerMovementSwitcher.Instance.FreezeMove();

    }

    private void ReleasePlayer() {
        PlayerMovementSwitcher.Instance.ReleaseMove();

    }

    public bool IsAnswerAreaPanelVisible()
    {
        return answerAreaPanel != null && answerAreaPanel.activeSelf;
    }
}
