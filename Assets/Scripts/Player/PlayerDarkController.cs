using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDarkController : MonoBehaviour
{
    [Header("移动参数")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    [Header("鼠标参数")]
    public Transform cameraTransform;
    public float mouseSensitivity = 100f;
    public float maxLookUp = 90f;
    public float maxLookDown = -90f;

    private CharacterController controller;
    private float verticalVelocity;
    private float xRotation = 0f;

    [Header("吸收音源参数")]
    public float absorbDistance = 3f; // 最大吸收距离
    public float absorbAngle = 30f;   // 最大吸收夹角
    public string absorbableTag = "AudioClip"; // 目标Tag

    private bool canMove = true;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (canMove) {
            HandleMouseLook();
            HandleMovement();
            HandleAbsorb();
        }
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, maxLookDown, maxLookUp);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move *= moveSpeed;

        // 判断是否在地面上
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // 稍微向下，保证贴地
            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }

    void HandleAbsorb()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            TryAbsorb();
        }
    }

    void TryAbsorb()
    {
        // 检测范围内所有碰撞器
        Collider[] candidates = Physics.OverlapSphere(transform.position, absorbDistance);

        foreach (var collider in candidates)
        {
            Debug.Log(collider.tag);
            if (!collider.CompareTag(absorbableTag)) continue;

            Vector3 directionToTarget = (collider.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToTarget);

            if (angle <= absorbAngle)
            {
                // 命中可吸收音源
                Debug.Log("吸收音源: " + collider.name);
                collider.gameObject.GetComponent<AudioIndicator>().OnAbsorbed();
                return;
            }
        }

        Debug.Log("附近没有可吸收的音源");
    }

    public void EnablePlayerMove()
    {
        canMove = true;
    }

    public void DisablePlayerMove()
    {
        canMove = false;
    }
}
