using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FPScontroller : NetworkBehaviour
{
    float playerSpeed = 12f;
    Vector2 rotation = new Vector2(0, 0);
    float mouseSensitivity = 200f;
    private CharacterController controller;

    private void Start()
    {
        if (isLocalPlayer)
        {
            Cursor.lockState = CursorLockMode.Locked;
            controller = GetComponent<CharacterController>();

            GameObject cameraMountPoint = gameObject;
            Transform cameraTransform = Camera.main.gameObject.transform;  // find main camera which is part of the scene instead of the prefab
            cameraTransform.parent = cameraMountPoint.transform;  // make the camera a child of the mount point
            cameraTransform.position = cameraMountPoint.transform.position;  // set position/rotation same as the mount point
            cameraTransform.rotation = cameraMountPoint.transform.rotation;
        }
    }
    private void Update()
    {
        if (isLocalPlayer)
        {
            HandleMovement();
            MouseLook();
        }
    }

    void HandleMovement()
    {
        if (isLocalPlayer)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector3 movement = transform.right * moveHorizontal + transform.forward * moveVertical;
            controller.Move(movement * playerSpeed * Time.deltaTime);

        }
    }
    void MouseLook()
    {
        if (isLocalPlayer)
        {
            rotation.y += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            rotation.x += -Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            transform.eulerAngles = (Vector2)rotation;
        }
    }
}
