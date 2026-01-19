using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLook : MonoBehaviour
{
    public float sensitivity = 3f;
    float rotX = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x * sensitivity * 0.1f;
        float mouseY = mouseDelta.y * sensitivity * 0.1f;

        // Rotation verticale libre (plus de clamp)
        rotX -= mouseY;
        transform.localRotation = Quaternion.Euler(rotX, 0f, 0f);

        // Rotation horizontale du joueur
        transform.parent.Rotate(Vector3.up * mouseX, Space.Self);
    }
}
