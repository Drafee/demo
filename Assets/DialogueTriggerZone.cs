using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueTriggerZone : MonoBehaviour
{
    public string dialogueKey = "Tutorial_initial_audio";
    private bool hasTriggered = false;
    public GameObject pickupCueUI;
    public GameObject bagUI;
    private Coroutine currentCueCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartDialogue();
        }
    }

    void StartDialogue()
    {
        Debug.Log("Æô¶¯¶Ô»°£º" + dialogueKey);
        DialogueManager.Instance.StartDialogue(dialogueKey, PickupCue);
    }
    public void PickupCue()
    {
        if (currentCueCoroutine != null)
        {
            StopCoroutine(currentCueCoroutine);
        }

        currentCueCoroutine = StartCoroutine(ShowPickupCue());
    }

    private IEnumerator ShowPickupCue()
    {
        pickupCueUI.SetActive(true);
        bagUI.SetActive(true);
        yield return new WaitForSeconds(4f);
        pickupCueUI.SetActive(false);
        currentCueCoroutine = null;

    }
}
