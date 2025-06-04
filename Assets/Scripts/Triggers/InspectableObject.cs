using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectableObject : MonoBehaviour
{

    private bool isPlayerNearby = false;

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            UIManager.Instance.ShowAnswerAreaPanel();
            UIManager.Instance.HidePressFReminder();

            if (!DialogueManager.Instance.hasTriggeredFDialogue)
            {
                DialogueManager.Instance.hasTriggeredFDialogue = true;
                DialogueManager.Instance.StartDialogue("Tutorial_pedastal", ()=> PlayerMovementSwitcher.Instance.FreezeMove());
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;

            if (!UIManager.Instance.IsAnswerAreaPanelVisible())
            {
                UIManager.Instance.ShowPressFReminder();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            UIManager.Instance.HidePressFReminder();
            UIManager.Instance.HideAnswerAreaPanel();
        }
    }

}
