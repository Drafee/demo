using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectableObject : MonoBehaviour
{

    private bool isPlayerNearby = false;
    private bool panelOpen = false;
    private bool hasTriggeredDialogue = false;

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            panelOpen = !panelOpen;

            if (panelOpen)
            {
                UIManager.Instance.ShowAnswerAreaPanel();

                if (!hasTriggeredDialogue)
                {
                    hasTriggeredDialogue = true;
                    DialogueManager.Instance.StartDialogue("Tutorial_pedastal");
                }
            }
            else
            {
                UIManager.Instance.HideAnswerAreaPanel();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            UIManager.Instance.ShowPressFReminder();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            UIManager.Instance.HidePressFReminder();
            UIManager.Instance.HideAnswerAreaPanel();
            panelOpen = false;
        }
    }
}
