using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLook : MonoBehaviour
{
    public float sensitivity = 3f;
    public float minY = -40f;
    public float maxY = 70f;

    public float smoothSpeed = 10f;

    float rotX;
    float rotY;

    float currentRotX;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        Vector2 delta = Mouse.current.delta.ReadValue();

        rotX -= delta.y * sensitivity * 0.1f;
        rotY += delta.x * sensitivity * 0.1f;

        rotX = Mathf.Clamp(rotX, minY, maxY);

        transform.localRotation = Quaternion.Euler(rotX, rotY, 0f);
    }
    void LateUpdate()
    {
        // Smoothing vertical de la caméra
        currentRotX = Mathf.Lerp(
            currentRotX,
            rotX,
            smoothSpeed * Time.deltaTime
        );

        transform.localRotation = Quaternion.Euler(currentRotX, 0f, 0f);
    }
}
/*
public class CameraLook : MonoBehaviour
{
    [Header("Références")]
    public Transform player; // DRAG le Player ici

    [Header("Sensibilité")]
    public float sensitivity = 3f;

    [Header("Clamp Vertical")]
    public float minY = -40f;
    public float maxY = 70f;

    [Header("Smoothing")]
    public float smoothSpeed = 10f; // plus haut = plus réactif

    float rotX;
    float currentRotX;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        Vector2 delta = Mouse.current.delta.ReadValue();

        // Calcul rotation verticale cible
        rotX -= delta.y * sensitivity * 0.1f;
        rotX = Mathf.Clamp(rotX, minY, maxY);

        // Rotation horizontale directe du joueur
        player.Rotate(Vector3.up * delta.x * sensitivity * 0.1f);
    }

    void LateUpdate()
    {
        // Smoothing vertical de la caméra
        currentRotX = Mathf.Lerp(
            currentRotX,
            rotX,
            smoothSpeed * Time.deltaTime
        );

        transform.localRotation = Quaternion.Euler(currentRotX, 0f, 0f);
    }
}
*/ 