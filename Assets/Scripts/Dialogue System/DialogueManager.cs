
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

        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {

        DialogueUI d = GetUIByTag(currentLines[currentLineIndex].speaker);
        if (d == null) {
            Debug.LogError($"Cannot find UILine with tag {currentLines[currentLineIndex].speaker}");
        }
        dialoguePanel = d.GetPanel();
        dialogueText = d.GetTextBox();
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (continueButton != null)
            continueButton.gameObject.SetActive(false);

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        string line = currentLines[currentLineIndex].text;
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

        if (continueButton != null)
            continueButton.gameObject.SetActive(true);
    }

    private void OnContinueClicked()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

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