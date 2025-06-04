
/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{



}
*/

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;


public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    private Dictionary<string, DialogueUI> uiLookup = new Dictionary<string, DialogueUI>();
    private Dictionary<string, DialogueData> dialogueMap;

    [Header("UI References")]
    private GameObject dialoguePanel;
    private TextMeshProUGUI dialogueText;
    public Button continueButton;

    [Header("Settings")]
    public float textSpeed = 0.2f;

    private Action onDialogueComplete;
    private bool isDialogueActive = false;
    private Coroutine typingCoroutine;

    private List<DialogueLine> currentLines;
    private int currentLineIndex;
    private string currentLineFullText = ""; // 新增字段
    bool isTyping = false; // 添加状态标志

    [Header("White Area UI 设置")]
    public GameObject Exit;
    public GameObject textButtonPrefab;
    public List<Transform> worldCanvas;
    public GameObject textUIPrefab;
    public Transform playerCamera;
    public float forwardSpacing = 2f;
    public float sideSpacing = 2f;
    public float height = 4f;

    [Header("White Area 播放设置")]
    public float wordReadSpeed = 0.2f; // 每个字的“播放时间”
    public float pauseBetweenLines = 1f;

    private List<DialogueLine> lines;
    private int direction = 1; // 用于左右交替
    private GameObject door;

    public void StartDialogueWhiteArea(string dialogueTag) {
        Debug.Log("Start Talking");
        playerCamera = PlayerMovementSwitcher.Instance.GetCurrentPlayerTransform();
        lines = GetDialogueByTag(dialogueTag).lines;
        Debug.Log("Lines count: " + lines.Count);
        StartCoroutine(SpawnTextsOverTime());
    }

    IEnumerator SpawnTextsOverTime()
    {
        Vector3 basePos = playerCamera.position + playerCamera.forward * 15f + Vector3.up * height;
        Vector3 spawnPos = basePos;

        for (int i = 0; i < lines.Count; i++)
        {
            string line = lines[i].text;

            GameObject textObj = Instantiate(textUIPrefab, spawnPos, Quaternion.identity);
            textObj.transform.parent = worldCanvas[LevelFlowExecutor.Instance.currentLevel - 1];
            // textObj.transform.rotation = Quaternion.LookRotation(textObj.transform.position - playerCamera.position);

            TextMeshProUGUI textComp = textObj.GetComponentInChildren<TextMeshProUGUI>();
            if (textComp != null)
                textComp.text = line;

            // 计算播放时长（按字数 * 每字时长）+ 停顿
            float duration = line.Length * wordReadSpeed + pauseBetweenLines;

            // 准备下一句位置（左右交替 + 向前推进）
            direction *= -1;
            spawnPos += playerCamera.forward * forwardSpacing + playerCamera.right * direction * sideSpacing + Vector3.up;

            yield return new WaitForSeconds(duration);
        }
        yield return new WaitForSeconds(1f);
        door = Instantiate(Exit, playerCamera.position + playerCamera.forward * 15f + Vector3.up, Quaternion.identity);
        ShowSelectionButtons();
    }

    void ShowSelectionButtons()
    {
        door.transform.GetChild(0).GetComponent<Canvas>().worldCamera = playerCamera.GetComponentInChildren<Camera>();
        Transform targetParent = door.transform
            .GetChild(0)   // 第1层
            .GetChild(0)   // 第2层
            .GetChild(0)   // 第3层
            .GetChild(0);  // 第4层

        foreach (DialogueLine line in lines)
        {
            GameObject btnObj = Instantiate(textButtonPrefab);
            btnObj.transform.SetParent(targetParent, false);

            TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText == null)
            {
                Debug.LogError("TextMeshProUGUI component not found on prefab.");
                continue;
            }

            btnText.text = line.text;

            Button btn = btnObj.GetComponentInChildren<Button>();
            if (btn == null)
            {
                Debug.LogError("Button component not found on prefab.");
                continue;
            }

            Debug.Log(line.text);
            string capturedLine = line.text;
            btn.onClick.AddListener(() => OnSelectLine(capturedLine, btnObj));
        }
    }

    GameObject currentlySelected;

    void OnSelectLine(string selectedLine, GameObject buttonObj)
    {
        if (currentlySelected != null)
        {
            // 取消上一个高亮
            currentlySelected.GetComponentInChildren<Image>().color = Color.white;
        }

        currentlySelected = buttonObj;
        buttonObj.GetComponentInChildren<Image>().color = Color.yellow;  // 高亮

        if (LevelFlowExecutor.Instance.currentLevel == 1 && selectedLine == lines[lines.Count - 1].text ||
            LevelFlowExecutor.Instance.currentLevel == 2 && selectedLine == lines[lines.Count - 2].text ||
            LevelFlowExecutor.Instance.currentLevel == 3 && selectedLine == lines[lines.Count - 3].text)
        {
            Debug.Log("正确！");

            LevelFlowExecutor.Instance.OnAnimationComplete();
        }
        else
        {
            Debug.Log("这不是你的答案");
            // 可加提示音或震动等反馈
        }
    }

    public void DestroyDoorsObjects() {
        Destroy(door.gameObject);
        foreach (Transform child in worldCanvas[LevelFlowExecutor.Instance.currentLevel - 1])
        {
            Destroy(child.gameObject);
        }

    }
    private void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueClicked);
    }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAllDialogues();

        AutoCollectUIs();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AutoCollectUIs();
    }
    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void AutoCollectUIs()
    {
        uiLookup.Clear();

        DialogueUI[] allUIs = FindObjectsOfType<DialogueUI>();

        foreach (var ui in allUIs)
        {
            if (!string.IsNullOrEmpty(ui.characterTag))
            {
                if (!uiLookup.ContainsKey(ui.characterTag))
                    uiLookup.Add(ui.characterTag, ui);
                else
                    Debug.LogWarning($"重复角色 Tag: {ui.characterTag}");
            }
        }

        Debug.Log($"Dialogue UI 收集完毕，当前共 {uiLookup.Count} 个角色 UI。");
    }

    public DialogueUI GetUIByTag(string tag)
    {
        if (uiLookup.TryGetValue(tag, out var ui))
            return ui;
        Debug.LogWarning($"未找到角色 UI: {tag}");
        return null;
    }
    // Load Dialogue from ScriptableObject


    void LoadAllDialogues()
    {
        dialogueMap = new Dictionary<string, DialogueData>();
        DialogueData[] allDialogues = Resources.LoadAll<DialogueData>("");

        foreach (var data in allDialogues)
        {
            if (!dialogueMap.ContainsKey(data.tag))
                dialogueMap.Add(data.tag, data);
            else
                Debug.LogWarning($"Dialogue duplicate: {data.tag}");
        }

        Debug.Log($"Loaded {dialogueMap.Count} dialogue entries.");
    }

    // GetDialogue
    public DialogueData GetDialogueByTag(string tag)
    {
        if (dialogueMap.TryGetValue(tag, out var data))
            return data;

        Debug.LogWarning($"Dialogue tag not found: {tag}");
        return null;
    }
    public void StartDialogue(string dialogueTag, Action onComplete = null)
    {
        var dialogue = GetDialogueByTag(dialogueTag);
        if (dialogue == null || dialogue.lines == null || dialogue.lines.Count == 0)
        {
            Debug.LogWarning("未找到有效的对话内容。");
            onComplete?.Invoke();
            return;
        }

        onDialogueComplete = onComplete;
        isDialogueActive = true;
        currentLines = dialogue.lines;
        currentLineIndex = 0;

        if (continueButton != null)
            continueButton.gameObject.SetActive(true);
        ShowCurrentLine();
    }
    private void ShowCurrentLine()
    {
        DialogueUI d = GetUIByTag(currentLines[currentLineIndex].speaker);
        if (d == null)
        {
            Debug.LogError($"Cannot find UILine with tag {currentLines[currentLineIndex].speaker}");
        }
        dialoguePanel = d.GetPanel();
        dialogueText = d.GetTextBox();

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        string line = currentLines[currentLineIndex].text;
        currentLineFullText = line;
        isTyping = true; // 设置打字状态
        typingCoroutine = StartCoroutine(TypeText(line));
    }

    private IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        isTyping = false; // 打字完成
    }

    private void OnContinueClicked()
    {
        if (isTyping && typingCoroutine != null)
        {
            // 如果正在打字，快进显示完整文本
            StopCoroutine(typingCoroutine);
            dialogueText.text = currentLineFullText;
            isTyping = false;
            typingCoroutine = null;
            if (continueButton != null)
                continueButton.gameObject.SetActive(true);
            return; // 直接返回，不执行后续逻辑
        }

        // 如果不在打字，继续下一句
        currentLineIndex++;
        if (currentLineIndex < currentLines.Count)
        {
            if (dialoguePanel != null)
                dialoguePanel.SetActive(false);
            ShowCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        if (continueButton != null)
            continueButton.gameObject.SetActive(false);
        isDialogueActive = false;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (continueButton != null)
            continueButton.gameObject.SetActive(false);

        currentLines = null;
        currentLineIndex = 0;

        onDialogueComplete?.Invoke();
        onDialogueComplete = null;
    }

    public bool IsDialogueActive => isDialogueActive;
}