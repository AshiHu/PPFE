using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerSpeedrunnerController : MonoBehaviour
{
    [Header("Vitesse")]
    public float walkSpeed = 8f;
    public float sprintSpeed = 12f;
    public float slideSpeed = 15f;
    public float jumpHeight = 2.5f;
    public float gravity = 20f;
    public float airControl = 0.5f; // contrôle en l'air (0 = aucun, 1 = plein)

    [Header("Glissade")]
    public float slideDuration = 0.8f;
    private bool isSliding = false;
    private Vector3 slideDirection;

    [Header("Contrôles")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode slideKey = KeyCode.LeftControl;
    public KeyCode sprintKey = KeyCode.LeftShift;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // --- Check sol ---
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f; // petit push vers les pieds

        // --- Inputs ---
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        // Mouvement horizontal (ignorer rotation Z/X)
        Vector3 forward = transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = transform.right;
        right.y = 0;
        right.Normalize();

        Vector3 move = right * inputX + forward * inputZ;
        move = Vector3.ClampMagnitude(move, 1f);

        // --- Sprint ---
        float speed = Input.GetKey(sprintKey) ? sprintSpeed : walkSpeed;

        // --- Glissade ---
        if (!isSliding)
        {
            controller.Move(move * speed * Time.deltaTime);

            if (Input.GetKeyDown(slideKey) && isGrounded && move.magnitude > 0.1f)
            {
                slideDirection = move.normalized;
                StartCoroutine(Slide());
            }
        }
        else
        {
            controller.Move(slideDirection * slideSpeed * Time.deltaTime);
        }

        // --- Rotation uniquement sur Y ---
        if (move.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 12f * Time.deltaTime);
        }

        // --- Saut ---
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
        }

        // --- Gravité ---
        velocity.y -= gravity * Time.deltaTime;

        // --- Air Control ---
        Vector3 horizontalVelocity = move * speed * (isGrounded ? 1f : airControl);
        Vector3 totalVelocity = horizontalVelocity + Vector3.up * velocity.y;

        controller.Move(totalVelocity * Time.deltaTime);
    }

    private IEnumerator Slide()
    {
        isSliding = true;
        float originalHeight = controller.height;
        controller.height = originalHeight / 2f;
        controller.center = new Vector3(0, originalHeight / 4f, 0); // recentrer le CharacterController

        yield return new WaitForSeconds(slideDuration);

        controller.height = originalHeight;
        controller.center = new Vector3(0, originalHeight / 2f, 0); // remettre le centre
        isSliding = false;
    }

    void LateUpdate()
    {
        // Bloquer rotation X et Z pour rester debout
        Vector3 euler = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0, euler.y, 0);
    }
}
