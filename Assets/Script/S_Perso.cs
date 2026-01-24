using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementCustomKeys : MonoBehaviour
{
    [Header("Physique")]
    public float moveSpeed = 8f;
    public float jumpHeight = 2.2f;
    public float gravityValue = -20f;

    [Header("Glissade")]
    public float slideSpeed = 15f;
    public float slideDuration = 0.8f;
    private bool isSliding = false;
    private Vector3 slideDirection;

    [Header("Configuration des Touches")]
    [Tooltip("L'axe vertical (Z/S ou W/S)")]
    public string verticalAxis = "Vertical";
    [Tooltip("L'axe horizontal (Q/D ou A/D)")]
    public string horizontalAxis = "Horizontal";

    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode slideKey = KeyCode.LeftControl;

    [Header("Détection Sol")]
    public float groundCheckRadius = 0.4f;
    public LayerMask groundLayer;

    [Header("Références")]
    public GravityManager gravityManager;

    private CharacterController controller;
    [HideInInspector] public Vector3 velocity;
    private bool isGrounded;
    private float jumpTimer = 0f;

    void Start() => controller = GetComponent<CharacterController>();

    void Update()
    {
        if (gravityManager == null) return;

        Vector3 gravityDir = gravityManager.gravityDirection.normalized;
        Vector3 playerUp = -gravityDir;

        // --- DÉTECTION DU SOL ---
        Vector3 spherePosition = transform.position + (gravityDir * (controller.height / 2f));
        if (jumpTimer <= 0)
        {
            isGrounded = Physics.CheckSphere(spherePosition, groundCheckRadius, groundLayer);
        }
        else
        {
            isGrounded = false;
            jumpTimer -= Time.deltaTime;
        }

        // --- GESTION VÉLOCITÉ ---
        if (isGrounded)
        {
            float dot = Vector3.Dot(velocity, gravityDir);
            if (dot > 0) velocity = gravityDir * 2f;
        }
        else
        {
            velocity += gravityDir * -gravityValue * Time.deltaTime;
        }

        // --- INPUTS VIA AXES (Évite le bug AZERTY/QWERTY) ---
        float inputX = Input.GetAxisRaw(horizontalAxis);
        float inputZ = Input.GetAxisRaw(verticalAxis);

        // --- CALCUL DU MOUVEMENT ---
        Vector3 moveForward = Vector3.ProjectOnPlane(transform.forward, gravityDir).normalized;
        Vector3 moveRight = Vector3.ProjectOnPlane(transform.right, gravityDir).normalized;
        Vector3 move = (moveRight * inputX + moveForward * inputZ).normalized;

        // --- LOGIQUE DE DÉPLACEMENT ---
        if (isSliding)
        {
            controller.Move(slideDirection * slideSpeed * Time.deltaTime);
        }
        else
        {
            controller.Move(move * moveSpeed * Time.deltaTime);

            if (Input.GetKeyDown(slideKey) && move.sqrMagnitude > 0.1f && isGrounded)
            {
                slideDirection = move;
                StartCoroutine(SlideRoutine());
            }
        }

        // --- SAUT ---
        if (Input.GetKeyDown(jumpKey) && isGrounded && !isSliding)
        {
            velocity = playerUp * Mathf.Sqrt(jumpHeight * 2f * Mathf.Abs(gravityValue));
            jumpTimer = 0.15f;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    private IEnumerator SlideRoutine()
    {
        isSliding = true;
        float originalHeight = controller.height;
        controller.height = originalHeight / 2f;
        yield return new WaitForSeconds(slideDuration);
        controller.height = originalHeight;
        isSliding = false;
    }
}