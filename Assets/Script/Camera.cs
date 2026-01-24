using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform playerBody;      // le corps du joueur
    public Transform cameraTransform;  // la caméra FPS
    public float mouseSensitivity = 2f;
    public float smoothSpeed = 10f;    // vitesse de lissage

    private float xRotation = 0f;
    private Quaternion targetCameraRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        targetCameraRotation = cameraTransform.localRotation;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotation horizontale du joueur
        playerBody.Rotate(Vector3.up * mouseX);

        // Rotation verticale de la caméra
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        targetCameraRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Lissage de la rotation
        cameraTransform.localRotation = Quaternion.Slerp(
            cameraTransform.localRotation,
            targetCameraRotation,
            smoothSpeed * Time.deltaTime
        );
    }
}
