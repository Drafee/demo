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
    public Button continueButotn;

    public GameObject GetPanel() {
        return dialoguePanel;
    }
    public TextMeshProUGUI GetTextBox() {
        return textBox;
    }

    public Button GetContinueButton()
    {
        return continueButotn;
    }
}
