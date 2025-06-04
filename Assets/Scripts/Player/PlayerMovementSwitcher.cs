using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementSwitcher : MonoBehaviour
{
    public static PlayerMovementSwitcher Instance { get; private set; }

    public GameObject fpsController;  // 自由移动
    public GameObject simpleMoveController; // 只能前后

    public GameObject currentController;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        currentController = fpsController;
    }
    private void Start()
    {
        currentController = fpsController;
    }
    public void SwitchToFPS(Transform spawnPoint)
    {
        MovePlayer(fpsController, spawnPoint);
        fpsController.SetActive(true);
        simpleMoveController.SetActive(false);
        currentController = fpsController;

    }

    public void SwitchToSimple(Transform spawnPoint)
    {
        Debug.Log("Switch to sIMPLE");
        Debug.Log(spawnPoint);
        MovePlayer(simpleMoveController, spawnPoint);
        simpleMoveController.SetActive(true);
        fpsController.SetActive(false);
        currentController = simpleMoveController;
    }

    private void MovePlayer(GameObject controller, Transform spawnPoint)
    {
        controller.transform.position = spawnPoint.position;
        controller.transform.rotation = spawnPoint.rotation;
    }

    public Transform GetCurrentPlayerTransform() {
        return currentController.transform;
    }

    public void FreezeMove() {

        if (currentController == fpsController)
        {
            currentController.GetComponent<PlayerDarkController>().DisablePlayerMove();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else {
            currentController.GetComponent<PlayerLightController>().DisablePlayerMove();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ReleaseMove()
    {

        if (currentController == fpsController)
        {
            currentController.GetComponent<PlayerDarkController>().EnablePlayerMove();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            currentController.GetComponent<PlayerLightController>().EnablePlayerMove();
        }
    }
}
