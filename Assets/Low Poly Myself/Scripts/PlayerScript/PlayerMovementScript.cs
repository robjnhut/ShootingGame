//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PlayerMovementScript : MonoBehaviour
//{
//    private CharacterController controller;

//    public float speed = 8f;
//    public float crouchSpeed = 4f; // Tốc độ khi ngồi
//    public float gravity = -9.81f ;
//    public float jumpHeight = 3f; // Sửa lỗi chính tả

//    public Transform groundCheck;
//    public float groundDistance = 0.4f;
//    public LayerMask groundMask;

//    Vector3 velocity;

//    bool isGrounded;
//    bool isMoving;
//    bool isCrouching = false; // Biến kiểm tra trạng thái ngồi

//    private Vector3 lastPosition = new Vector3(0f, 0f, 0f);

//    // Chiều cao mặc định của CharacterController
//    private float originalHeight;
//    public float crouchHeight = 1f; // Chiều cao khi ngồi

//    // Start is called before the first frame update
//    void Start()
//    {
//        controller = GetComponent<CharacterController>();
//        originalHeight = controller.height; // Lưu lại chiều cao ban đầu
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        // Ground check 
//        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
//        // Resetting the default velocity

//        if (isGrounded && velocity.y < 0)
//        {
//            velocity.y = -2f;
//        }

//        // Getting the inputs
//        float x = Input.GetAxis("Horizontal");
//        float z = Input.GetAxis("Vertical");

//        // Kiểm tra trạng thái ngồi
//        if (Input.GetKeyDown(KeyCode.LeftControl) && !isCrouching)
//        {
//            Crouch(); // Gọi hàm ngồi
//        }
//        if (Input.GetKeyUp(KeyCode.LeftControl) && isCrouching)
//        {
//            StandUp(); // Gọi hàm đứng dậy
//        }

//        // Tạo vector di chuyển
//        float currentSpeed = isCrouching ? crouchSpeed : speed; // Sử dụng tốc độ phù hợp khi ngồi
//        Vector3 move = transform.right * x + transform.forward * z; // Di chuyển theo trục X và Z

//        // Di chuyển nhân vật
//        controller.Move(move * currentSpeed * Time.deltaTime);

//        // Kiểm tra nếu nhân vật nhảy
//        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching) // Không thể nhảy khi ngồi
//        {
//            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // Thực hiện nhảy
//        }

//        // Rơi xuống
//        velocity.y += gravity * Time.deltaTime;

//        // Thực hiện việc di chuyển theo trục Y (nhảy/rơi)
//        controller.Move(velocity * Time.deltaTime);

//        // Kiểm tra trạng thái di chuyển
//        if (lastPosition != gameObject.transform.position && isGrounded == true)
//        {
//            isMoving = true;
//        }
//        else
//        {
//            isMoving = false;
//        }
//        lastPosition = gameObject.transform.position;
//    }

//    void Crouch()
//    {
//        isCrouching = true;
//        controller.height = crouchHeight; // Giảm chiều cao của nhân vật
//    }

//    void StandUp()
//    {
//        isCrouching = false;
//        controller.height = originalHeight; // Trở lại chiều cao bình thường
//    }
//}


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
