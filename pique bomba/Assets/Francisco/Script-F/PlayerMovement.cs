using Fusion;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Movimento")]
    public float speed = 6f;

    [Header("Mouse")]
    public float mouseSensitivity = 2f;

    [Header("Pulo")]
    public float jumpForce = 7f;

    private Rigidbody rb;

    private float cameraRotationX = 0f;

    public Camera playerCamera;

    private bool isGrounded;

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody>();

        if (Object.HasInputAuthority)
        {
            playerCamera.gameObject.SetActive(true);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            playerCamera.gameObject.SetActive(false);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasInputAuthority)
            return;

        Move();
        Look();
        JumpCheck();
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move =
            transform.forward * v +
            transform.right * h;

        Vector3 velocity = move * speed;

        velocity.y = rb.linearVelocity.y;

        rb.linearVelocity = velocity;
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        cameraRotationX -= mouseY;

        cameraRotationX = Mathf.Clamp(cameraRotationX, -80f, 80f);

        playerCamera.transform.localRotation =
            Quaternion.Euler(cameraRotationX, 0f, 0f);
    }

    void JumpCheck()
    {
        isGrounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            1.2f
        );

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce,
                ForceMode.Impulse);
        }
    }
}