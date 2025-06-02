using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class PlayerLightController : MonoBehaviour
{
    [Header("移动参数")]
    public float moveSpeed = 3f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private float verticalVelocity;

    private bool canMove = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (canMove)
        {

            HandleMovement();
        }

    }

    void HandleMovement()
    {
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.forward * moveZ * moveSpeed;

        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // 保持贴地
        }

        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }

    public void EnablePlayerMove() {
        canMove = true;
    }

    public void DisablePlayerMove()
    {
        canMove = false;
    }
}
