using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectableObject : MonoBehaviour
{

    private bool isPlayerNearby = false;
    private bool panelOpen = false;

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            panelOpen = !panelOpen;
            UIManager.Instance.ShowAnswerAreaPanel();
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
            UIManager.Instance.HideAnswerAreaPanel();
            UIManager.Instance.HidePressFReminder();
            panelOpen = false;
        }
    }
}
