using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Microsoft.VisualBasic.FileIO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DialogueCSVLoader : MonoBehaviour
{
    [ContextMenu("Load and Generate Dialogues")]
    void LoadFile()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("DialogueData");
        if (csvFile == null)
        {
            Debug.LogError("cannot find csv file");
            return;
        }

        Dictionary<string, List<DialogueLine>> dialogueGroups = new Dictionary<string, List<DialogueLine>>();

        using (StringReader stringReader = new StringReader(csvFile.text))
        using (TextFieldParser parser = new TextFieldParser(stringReader))
        {
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");

            // Read All filed
            parser.ReadFields();

            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();
                if (fields.Length < 3) continue;

                string tag = fields[0];
                DialogueLine line = new DialogueLine
                {
                    speaker = fields[1],
                    text = fields[2],
                };

                if (!dialogueGroups.ContainsKey(tag))
                    dialogueGroups[tag] = new List<DialogueLine>();

                dialogueGroups[tag].Add(line);
            }
        }

        // Store Data into the scriptableObject
#if UNITY_EDITOR
        string outputPath = "Assets/Resources/Dialogues/";
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        foreach (var pair in dialogueGroups)
        {
            DialogueData asset = ScriptableObject.CreateInstance<DialogueData>();
            asset.tag = pair.Key;
            asset.lines = pair.Value;

            string assetPath = $"{outputPath}{pair.Key}_Dialogue.asset";
            AssetDatabase.CreateAsset(asset, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Dialogue Store successful£¡");
#endif
    }
}