using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    private Dictionary<string, DialogueUI> uiLookup = new Dictionary<string, DialogueUI>();

    private Dictionary<string, DialogueData> dialogueMap;

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
}
