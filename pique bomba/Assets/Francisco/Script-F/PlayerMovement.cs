using Fusion;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Movimento")]
    public float speed = 7f;

    [Header("Mouse")]
    public float mouseSensitivity = 2f;

    [Header("Pulo")]
    public float jumpForce = 7f;

    [Header("Referências")]
    public Transform cameraHolder;
    public Camera playerCamera;

    private Rigidbody rb;

    private float xRotation;

    private bool isGrounded;

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody>();

        Debug.Log("Player spawnou!");

        if (Object.HasInputAuthority)
        {
            Debug.Log("Sou o player local!");

            playerCamera.gameObject.SetActive(true);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            playerCamera.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!Object.HasInputAuthority)
            return;

        MouseLook();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasInputAuthority)
            return;

        Move();
        Jump();
    }

    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move =
            transform.right * x +
            transform.forward * z;

        move.Normalize();

        Vector3 velocity = move * speed;

        // mantém velocidade vertical
        velocity.y = rb.velocity.y;

        rb.velocity = velocity;
    }

    void MouseLook()
    {
        float mouseX =
            Input.GetAxis("Mouse X") * mouseSensitivity;

        float mouseY =
            Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraHolder.localRotation =
            Quaternion.Euler(xRotation, 0f, 0f);
    }

    void Jump()
    {
        isGrounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            1.2f
        );

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(
                Vector3.up * jumpForce,
                ForceMode.Impulse
            );
        }
    }
}