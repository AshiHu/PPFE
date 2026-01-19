using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerAdvanced : MonoBehaviour
{
    [Header("Références")]
    public Transform cameraPivot; // <-- DRAG CameraPivot ICI

    [Header("Vitesse")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float slideSpeed = 15f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Glissade")]
    public float slideDuration = 0.8f;
    private bool isSliding = false;

    [Header("Contrôles")]
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode slideKey = KeyCode.LeftControl;
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Animations")]
    public Animator animator;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Sol
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // Inputs
        float moveX = Input.GetAxis("Horizontal"); // Q / D
        float moveZ = Input.GetAxis("Vertical");   // Z / S

        // Directions caméra
        Vector3 camForward = cameraPivot.forward;
        Vector3 camRight = cameraPivot.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // Mouvement relatif à la caméra
        Vector3 move = camForward * moveZ + camRight * moveX;

        // Vitesse
        float currentSpeed = walkSpeed;

        if (!isSliding)
        {
            if (Input.GetKey(sprintKey) && move.magnitude > 0.1f)
                currentSpeed = sprintSpeed;

            if (Input.GetKeyDown(slideKey) && move.magnitude > 0.1f)
                StartCoroutine(Slide());
        }
        else
        {
            currentSpeed = slideSpeed;
        }

        // Déplacement
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Rotation du joueur vers la direction du mouvement (TPS PRO)
        if (move.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                10f * Time.deltaTime
            );
        }

        // Saut
        if (Input.GetKeyDown(jumpKey) && isGrounded && !isSliding)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // Gravité
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Animations
        if (animator)
        {
            animator.SetFloat("Speed", move.magnitude * currentSpeed);
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetBool("IsSliding", isSliding);
            animator.SetFloat("VerticalVelocity", velocity.y);
        }
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
