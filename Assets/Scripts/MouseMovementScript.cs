using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovementScript : MonoBehaviour
{
    public float mouseSensitivity = 250f;
    float xRotation = 0f;
    float yRotation = 0f;

    public float topClam = -90f;
    public float bottomClam = 90f;

    // Start is called before the first frame update
    void Start()
    {
        // Lock cursor to the middle of the game and make it invisible
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // Getting the mouse inputs
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;  // Left and right -> mouse X
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;  // Up and down -> mouse Y

        // Rotation around the x axis (look up and down)
        xRotation -= mouseY;

        // Clamp the rotation
        xRotation = Mathf.Clamp(xRotation, topClam, bottomClam);

        // Rotation around the y axis (look left and right)
        yRotation += mouseX;

        // Apply rotations to our transform
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
