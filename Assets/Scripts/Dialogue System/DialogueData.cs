using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public string tag;
    public List<DialogueLine> lines;
}

[System.Serializable]
public class DialogueLine
{
    public string speaker;
    [TextArea] public string text;
    public string audioTag;
    public float typingSpeed;
}
