using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementCustomKeys : MonoBehaviour
{
    [Header("Vitesse")]
    public float moveSpeed = 8f;
    public float slideSpeed = 15f;
    public float jumpHeight = 2.2f;
    public float gravity = -20f;

    [Header("Glissade")]
    public float slideDuration = 0.8f;
    private bool isSliding = false;
    private Vector3 slideDirection;

    [Header("Contrôles")]
    public KeyCode slideKey = KeyCode.LeftControl;
    public KeyCode jumpKey = KeyCode.Space;

    // **Touches de déplacement personnalisables**
    public KeyCode forwardKey = KeyCode.Z;   // Z ou W
    public KeyCode backwardKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.Q;      // Q ou A
    public KeyCode rightKey = KeyCode.D;

    private CharacterController controller;
    [HideInInspector] public Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        // --- Check sol ---
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // --- INPUTS personnalisables ---
        float inputX = 0f;
        float inputZ = 0f;

        if (Input.GetKey(forwardKey)) inputZ += 1f;
        if (Input.GetKey(backwardKey)) inputZ -= 1f;
        if (Input.GetKey(rightKey)) inputX += 1f;
        if (Input.GetKey(leftKey)) inputX -= 1f;

        // Déplacement relatif au joueur
        Vector3 move = transform.right * inputX + transform.forward * inputZ;
        move = Vector3.ClampMagnitude(move, 1f);

        float currentSpeed = moveSpeed;

        // GLISSADE
        if (isSliding)
        {
            controller.Move(slideDirection * slideSpeed * Time.deltaTime);
        }
        else
        {
            controller.Move(move * currentSpeed * Time.deltaTime);

            if (Input.GetKeyDown(slideKey) && move.magnitude > 0.1f)
            {
                slideDirection = move.normalized;
                StartCoroutine(Slide());
            }
        }

        // SAUT
        if (Input.GetKeyDown(jumpKey) && isGrounded && !isSliding)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // GRAVITÉ
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private IEnumerator Slide()
    {
        isSliding = true;
        float originalHeight = controller.height;
        controller.height = originalHeight / 2f;

        yield return new WaitForSeconds(slideDuration);

        controller.height = originalHeight;
        isSliding = false;
    }
}
