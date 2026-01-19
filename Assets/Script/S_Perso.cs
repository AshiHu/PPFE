using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class TPSController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float jumpForce = 5f;

    [Header("Camera Settings")]
    public Transform cameraPivot;       // Empty pivot au centre du joueur
    public Transform playerCamera;      // La caméra attachée au pivot
    public Vector3 cameraOffset = new Vector3(0, 2f, -4f);
    public float mouseSensitivity = 100f;
    public float maxLookAngle = 45f;

    private Rigidbody rb;
    private float pitch = 20f;          // angle vertical initial
    private float yaw = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
        HandleJump();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    // ----- MOUSE LOOK -----
    private void HandleMouseLook()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        float mouseX = mouse.delta.x.ReadValue() * mouseSensitivity * Time.deltaTime;
        float mouseY = mouse.delta.y.ReadValue() * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;                      // rotation horizontale autour du joueur
        pitch -= mouseY;                    // rotation verticale de la caméra
        pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

        // On place le pivot au joueur
        cameraPivot.position = transform.position + Vector3.up * 1.5f;

        // Rotation du pivot (yaw)
        cameraPivot.rotation = Quaternion.Euler(0, yaw, 0);

        // Position de la caméra relative au pivot
        playerCamera.localPosition = cameraOffset;

        // Pitch vertical de la caméra
        playerCamera.localRotation = Quaternion.Euler(pitch, 0, 0);
    }

    // ----- PLAYER MOVEMENT -----
    private void HandleMovement()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        float moveX = 0f;
        float moveZ = 0f;

        if (keyboard.aKey.isPressed) moveX = -1f;
        if (keyboard.dKey.isPressed) moveX = 1f;
        if (keyboard.wKey.isPressed) moveZ = 1f;
        if (keyboard.sKey.isPressed) moveZ = -1f;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move.Normalize();

        Vector3 velocity = move * speed;
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;
    }

    // ----- JUMP -----
    private void HandleJump()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.spaceKey.wasPressedThisFrame && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
