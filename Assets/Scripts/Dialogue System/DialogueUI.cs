using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public string characterTag;
    public GameObject dialoguePanel;
    public TextMeshProUGUI textBox;
    private void Start()
    {
        dialoguePanel.SetActive(false);
    }
    public GameObject GetPanel() {
        return dialoguePanel;
    }
    public TextMeshProUGUI GetTextBox() {
        return textBox;
    }
}
