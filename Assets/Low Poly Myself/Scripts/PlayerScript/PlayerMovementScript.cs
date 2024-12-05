

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    private CharacterController controller;

    public float speed = 8f;
    public float crouchSpeed = 4f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;

    bool isGrounded;
    bool isCrouching = false;

    private float originalHeight;
    public float crouchHeight = 1f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        originalHeight = controller.height;
    }

    void Update()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Crouch logic
        if (Input.GetKeyDown(KeyCode.LeftControl) && !isCrouching)
        {
            Crouch();
        }
        if (Input.GetKeyUp(KeyCode.LeftControl) && isCrouching)
        {
            StandUp();
        }

        // Move
        float currentSpeed = isCrouching ? crouchSpeed : speed;
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Jump
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded && !isCrouching)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            else if (isCrouching)
            {
                Debug.Log("Không thể nhảy khi đang ngồi");
            }
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Check movement
        if (controller.velocity.magnitude > 0.1f && isGrounded)
        {
            Debug.Log("Nhân vật đang di chuyển");
        }
        else
        {
            Debug.Log("Nhân vật đứng yên");
        }
    }

    void Crouch()
    {
        isCrouching = true;
        controller.height = crouchHeight;
    }

    void StandUp()
    {
        isCrouching = false;
        controller.height = originalHeight;
    }
}
