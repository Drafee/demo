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
        // MovePlayer(fpsController, spawnPoint);
        fpsController.SetActive(true);
        simpleMoveController.SetActive(false);
        currentController = fpsController;
    }

    public void SwitchToSimple(Transform spawnPoint)
    {
        // MovePlayer(simpleMoveController, spawnPoint);
        simpleMoveController.SetActive(true);
        fpsController.SetActive(false);
        currentController = simpleMoveController;
    }

    private void MovePlayer(GameObject controller, Transform spawnPoint)
    {
        controller.transform.position = spawnPoint.position;
        controller.transform.rotation = spawnPoint.rotation;
    }

    public void FreezeMove() {

        if (currentController == fpsController)
        {
            currentController.GetComponent<PlayerDarkController>().DisablePlayerMove();
        }
        else {
            currentController.GetComponent<PlayerLightController>().DisablePlayerMove();
        }
    }

    public void ReleaseMove()
    {

        if (currentController == fpsController)
        {
            currentController.GetComponent<PlayerDarkController>().EnablePlayerMove();
        }
        else
        {
            currentController.GetComponent<PlayerLightController>().EnablePlayerMove();
        }
    }
}
